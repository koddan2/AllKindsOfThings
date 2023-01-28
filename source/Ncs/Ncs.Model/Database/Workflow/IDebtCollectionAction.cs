namespace Ncs.Model.Database.Workflow
{
    public enum DebtCollectionActionBase
    {
        None = 0,
        Case,
        Debtor,
        Legal,
    }
    public interface IDebtCollectionAction
    {

    }
}