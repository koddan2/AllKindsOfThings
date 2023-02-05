using N2.Model;

namespace N2.Domain.DebtCollectionCase.Commands
{
    public readonly record struct CreateNewCaseCommand() : ICaseCommand
    {
        public string ClientIdentity { get; init; }
        public ISet<string> DebtorIdentities { get; init; }
        public ISet<string> DebtIdentities { get; init; }
        public CollectionProcess CollectionProcess { get; init; }
    }
}