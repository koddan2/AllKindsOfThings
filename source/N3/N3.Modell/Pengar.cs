using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace N3.Modell
{
    ////public readonly record struct Pengar(decimal Belopp, string ValutaKod = "") : IPengar;
    /// <summary>
    /// Motsvarar ett belopp av pengar i någon valuta
    /// </summary>
    [DataContract]
    public record Pengar
    {
        [JsonConstructor]
        public Pengar(decimal belopp, string valutaKod)
        {
            Belopp = belopp;
            ValutaKod = valutaKod;
        }

        [Required]
        [DataMember]
        [JsonPropertyName("belopp")]
        public decimal Belopp { get; init; }

        [DataMember]
        [Required]
        [JsonPropertyName("valutaKod")]
        public virtual string ValutaKod { get; init; }
    }
}
