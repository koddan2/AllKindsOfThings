using Microsoft.Extensions.Options;
using N3.CqrsEs.Ramverk;
////using SAK;
using static LanguageExt.Prelude;
using System.Text.Json;
using SAK;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using Npgsql;
using Marten;

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

        private readonly IDocumentStore _store;

        public LåtsasAktivitetsBuss(
            IOptions<LåtsasAktivitetsBussKonfiguration> options,
            IDocumentStore store
        )
        {
            Konfiguration = options.Value;
            Initialisera();
            _store = store;
        }

        private LåtsasAktivitetsBussKonfiguration Konfiguration { get; }

        public async Task<IEnumerable<AktivitetsStatus>> HämtaStatus(
            AktivitetsKategori kategori,
            AktivitetsStatusFiltrering filtrering = AktivitetsStatusFiltrering.EjAvslutade
        )
        {
            await using var session = _store.LightweightSession();
            var poster = await session
                .Query<LåtsasAktivitetsPost>()
                .Where(x => x.Kategori == kategori.Namn)
                .ToListAsync();
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
            await using var session = _store.LightweightSession();
            var parametrar = new { AktivitetsIdentifierare = aktivitetsIdentifierare.ToString(), };
            var post = await session
                .Query<LåtsasAktivitetsPost>()
                .Where(x => x.AktivitetsIdentifierare == (string)aktivitetsIdentifierare)
                .SingleAsync();

            return new AktivitetsStatus(
                aktivitetsIdentifierare,
                new AktivitetsKategori(post.Kategori.OrFail()),
                post.ReservationsIdentifierare is not null
            );
        }

        public async Task Kölägg<T>(T data)
            where T : IAktivitet
        {
            await using var session = _store.LightweightSession();
            var post = new LåtsasAktivitetsPost
            {
                AktivitetsIdentifierare = (string)data.AktivitetsIdentifierare,
                Kategori = data.AktivitetsKategori.Namn,
                TypNamn = typeof(T).Name,
                JsonData = JsonSerializer.Serialize((object)data, _JsonSerializerOptions),
            };

            session.Store(post);
            await session.SaveChangesAsync();
        }

        public async Task<AktivitetsKvitto> Reservera(UnikIdentifierare aktivitetsIdentifierare)
        {
            await using var session = _store.LightweightSession();
            var parametrar = new
            {
                AktivitetsIdentifierare = aktivitetsIdentifierare.ToString(),
                ReservationsIdentifierare = UnikIdentifierare.Skapa().ToString(),
            };
            var post = await session
                .Query<LåtsasAktivitetsPost>()
                .FirstOrDefaultAsync(
                    x => (string)x.AktivitetsIdentifierare! == (string)aktivitetsIdentifierare
                );
            if (post is not null)
            {
                post.ReservationsIdentifierare = UnikIdentifierare.Skapa();
                post.ReservationsTidsstämpel = DateTimeOffset.Now;
                session.Update(post);
                await session.SaveChangesAsync();
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
                using var conn = new NpgsqlConnection(
                    Konfiguration.PostgresConnectionString.OrFail()
                );
                conn.Open();
                var exists =
                    conn.ExecuteScalar(
                        @"
                SELECT EXISTS (
                    SELECT FROM 
                        pg_tables
                    WHERE 
                        schemaname = 'public' AND 
                        tablename  = 'n3schemaversion'
                );"
                    ) as bool?;
                if (exists is false) { }
            }
        }
    }
}
