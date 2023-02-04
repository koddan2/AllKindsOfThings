using System.Globalization;

namespace N2.Model
{
	public readonly record struct Money(decimal Amount, Currency Currency)
	{
		public override string ToString() => this.ToString(CultureInfo.InvariantCulture);
		public string ToString(CultureInfo cultureInfo) => this.Currency.Position switch
		{
			Currency.SymbolPosition.After => $"{Amount.ToString(cultureInfo)}{Currency.Symbol}",
			_ => $"{Currency.Symbol}{Amount.ToString(cultureInfo)}",
		};
	}
	public record Currency
	{
		public enum SymbolPosition
		{
			Before,
			After,
		}
		public static Currency SEK { get; } = new Currency("SEK", " kr", SymbolPosition.After);
		private Currency(string name, string symbol, SymbolPosition symbolPosition)
		{
			Name = name;
			Symbol = symbol;
			Position = symbolPosition;
		}

		public string Name { get; }
		public string Symbol { get; }
		public SymbolPosition Position { get; }
	}
}