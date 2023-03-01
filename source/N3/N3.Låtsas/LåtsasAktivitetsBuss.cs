using Microsoft.Extensions.Options;
using N3.CqrsEs.Ramverk;
////using SAK;
using static LanguageExt.Prelude;
using System.Text.Json;
using SAK;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Text;
using System.Text.Unicode;
using System.Text.Encodings.Web;

namespace N3.Låtsas
{
    public class LåtsasAktivitetsBuss : IAktivitetsBuss
    {
        private static readonly object _Lock = new();

        private readonly static JsonSerializerOptions _JsonSerializerOptions =
            new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(
                    UnicodeRanges.BasicLatin,
                    UnicodeRanges.Latin1Supplement
                ),
            };
        private readonly static string _DatabasNamn = "LåtsasAktivitetsBuss.db";

        public LåtsasAktivitetsBuss(IOptions<LåtsasAktivitetsBussKonfiguration> options)
        {
            Konfiguration = options.Value;
            Initialisera();
        }

        private LåtsasAktivitetsBussKonfiguration Konfiguration { get; }

        private string Sökväg => Path.Combine(Konfiguration.Katalog.OrFail(), _DatabasNamn);

        public async Task<IEnumerable<AktivitetsStatus>> HämtaStatus(
            AktivitetsKategori kategori,
            AktivitetsStatusFiltrering filtrering = AktivitetsStatusFiltrering.EjAvslutade
        )
        {
            await ValueTask.CompletedTask;
            using var koppling = TaDatabasKoppling();
            var parametrar = new { Kategori = kategori.Namn, };
            var poster = koppling.Query<LåtsasAktivitetsPost>(
                "select * from aktivitet where kategori=@Kategori",
                parametrar
            );

            return poster.Select(
                post =>
                    new AktivitetsStatus(
                        post.AktivitetsIdentifierare.OrFail(),
                        new AktivitetsKategori(post.Kategori.OrFail()),
                        post.ReservationsIdentifierare is not null
                    )
            );
        }

        public async Task<AktivitetsStatus> HämtaStatus(UnikIdentifierare aktivitetsIdentifierare)
        {
            await ValueTask.CompletedTask;
            using var koppling = TaDatabasKoppling();
            var parametrar = new { AktivitetsIdentifierare = aktivitetsIdentifierare.ToString(), };
            var post = koppling.QueryFirst<LåtsasAktivitetsPost>(
                "select * from aktivitet where aktivitetsidentifierare=@AktivitetsIdentifierare",
                parametrar
            );

            return new AktivitetsStatus(
                aktivitetsIdentifierare,
                new AktivitetsKategori(post.Kategori.OrFail()),
                post.ReservationsIdentifierare is not null
            );
        }

        public async Task Kölägg<T>(T data)
            where T : IAktivitet
        {
            await ValueTask.CompletedTask;
            using var koppling = TaDatabasKoppling();
            var post = new LåtsasAktivitetsPost
            {
                AktivitetsIdentifierare = (string)data.AktivitetsIdentifierare,
                Kategori = data.AktivitetsKategori.Namn,
                TypNamn = typeof(T).Name,
                JsonData = JsonSerializer.Serialize((object)data, _JsonSerializerOptions),
            };
            var sql =
                @"insert into aktivitet( aktivitetsidentifierare,  kategori,  typnamn,  jsondata)
                                      values (@AktivitetsIdentifierare, @Kategori, @TypNamn, @JsonData)";
            _ = koppling.Execute(sql, post);
        }

        public async Task<AktivitetsKvitto> Reservera(UnikIdentifierare aktivitetsIdentifierare)
        {
            await ValueTask.CompletedTask;
            using var koppling = TaDatabasKoppling();
            var parametrar = new
            {
                AktivitetsIdentifierare = aktivitetsIdentifierare.ToString(),
                ReservationsIdentifierare = UnikIdentifierare.Skapa().ToString(),
            };
            var post = koppling.QueryFirstOrDefault<LåtsasAktivitetsPost>(
                "select * from aktivitet where aktivitetsidentifierare=@AktivitetsIdentifierare",
                parametrar
            );
            if (
                post is not null && post.AktivitetsIdentifierare.OrFail() == aktivitetsIdentifierare
            )
            {
                var antal = koppling.Execute(
                    @"update aktivitet
                    set reservationstidsstämpel=@ReservationsTidsstämpel,
                        reservationsidentifierare=@ReservationsIdentifierare
                    where aktivitetsidentifierare=@AktivitetsIdentifierare",
                    parametrar
                );
                if (antal != 1)
                {
                    throw new Exception();
                }
                return new AktivitetsKvitto(
                    aktivitetsIdentifierare,
                    parametrar.ReservationsIdentifierare
                );
            }

            return new AktivitetsKvitto(aktivitetsIdentifierare, null);
        }

        private void Initialisera()
        {
            lock (_Lock)
            {
                var katalog = Konfiguration.Katalog.OrFail();
                if (!Directory.Exists(katalog))
                {
                    _ = Directory.CreateDirectory(katalog);
                }

                using var koppling = TaDatabasKoppling();
                long? result = koppling.ExecuteScalar("PRAGMA user_version;") as long?;
                if (result is 0)
                {
                    var ddl =
                        @"create table aktivitet(
                        id integer primary key
                        , aktivitetsidentifierare text not null unique
                        , kategori text not null
                        , typnamn text not null
                        , jsondata text not null
                        , reservationstidsstämpel text null
                        , reservationsidentifierare text null
                    );";
                    _ = koppling.Execute(ddl);
                    _ = koppling.Execute("PRAGMA user_version = 1;");
                }
            }
        }

        private IDbConnection TaDatabasKoppling()
        {
            var connection = new SqliteConnection($"Data Source={this.Sökväg}");
            connection.Open();
            return connection;
        }
    }
}
