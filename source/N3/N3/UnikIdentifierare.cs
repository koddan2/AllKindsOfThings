using System.Text.Json.Serialization;

namespace N3
{
    /// <summary>
    /// Ett globalt unikt värde, bestående av alfanumeriska tecken
    /// som är shiftlägeskänsliga.
    /// Förkortat "UID".
    /// Exempel:
    /// 4QDKqvHAPEldy3ijc1HX95
    /// xBIimss7CkAszsCokUzHv
    /// 3hGEwuogUUNYKawTv3eRHj
    /// AE8Ls2SXOKGBvKdtJ8fCT
    /// Tekniskt: En System.Guid uttryckt i Bas-62
    /// bibliotek: https://www.nuget.org/packages/Base62-Net
    /// </summary>
    /// <param name="Värde">Själva värdet.</param>
    public readonly record struct UnikIdentifierare(
        [property:JsonPropertyName("ID")]
        string Värde)
    {
        [JsonPropertyName("$type")]
#pragma warning disable CA1822 // Mark members as static
        public string _ => "N3.UnikIdentifierare";

#pragma warning restore CA1822 // Mark members as static
        public static implicit operator string(UnikIdentifierare u) => u.Värde;

        public static implicit operator UnikIdentifierare(string s)
        {
#if DEBUG // tvinga att Värdet i grunden är en System.Guid
            ValideraSträngVärde(s);
#endif
            return new(s);
        }

        public static implicit operator Guid(UnikIdentifierare u) =>
            new(Base62.EncodingExtensions.FromBase62(u.Värde));

        public static implicit operator UnikIdentifierare(Guid g) =>
            new(Base62.EncodingExtensions.ToBase62(g.ToByteArray()));

        public static UnikIdentifierare Skapa() => Guid.NewGuid();

        public readonly static UnikIdentifierare Ingen = Guid.Empty;

        private static void ValideraSträngVärde(string s)
        {
            Guid? ok = new Guid(Base62.EncodingExtensions.FromBase62(s));
            if (ok is null)
            {
                throw new ArgumentException($"Kunde inte konvertera värdet {s} till en System.Guid!");
            }
        }
    }
}
