namespace N3.Model
{
	public readonly record struct Land(string Namn)
	{
		public static Land Sverige => new("Sverige");
	}
}