namespace N3.Modell
{
	//
	// En provisionsinställning styr hur många procent av en betalning ombudet tar som provision.
	// Ifall det är en direktbetalning, så ska klienten faktureras.
	// Inställningen styr också vilket bokföringskonto som krediteras, samt
	// om, och hur mycket, moms ska räknas ut.
	public readonly record struct ProvisionsInställning(
		Procent AndelAvBetalning,
		BokföringsKonto BokföringsKonto,
		Moms Moms);
}