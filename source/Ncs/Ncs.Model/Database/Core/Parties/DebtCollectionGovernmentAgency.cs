﻿using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Core
{
    [InitRequired]
    public class DebtCollectionGovernmentAgency : BaseTransactionalDatabaseModelWithIdentifiers
    {
        [ForeignKey(nameof(PartyId))]
        public virtual DebtCollectionParty Party { get; set; }
        public long PartyId { get; set; }
    }
}
