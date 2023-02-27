namespace N3.Modell
{
    public readonly record struct PersonNummer(string Värde);
    public readonly record struct Land(string Namn)
    {
        public static Land Sverige => new("Sverige");
    }
}
