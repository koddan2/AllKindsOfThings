namespace N2.Model
{
	public readonly record struct CollectionProcess(
		string Name,
		AllocationKey AllocationKey,
		CommissionModel CommissionModel,
		PriceList PriceList);
}