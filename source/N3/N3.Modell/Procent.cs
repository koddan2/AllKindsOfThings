using System.Text.Json.Serialization;

namespace N3.Modell
{
    public readonly record struct Procent(decimal Enheter)
    {
        public override string ToString() => $"{Enheter}%";

        //
        // svarar med en faktor som kan användas i multiplikation
        [JsonIgnore]
        public decimal Faktor => Enheter / 100m;

        public static implicit operator decimal(Procent u) => u.Enheter;

        public static implicit operator Procent(decimal d) => new(d);
    }
}
