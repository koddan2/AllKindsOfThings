using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Domain.Model
{
	[InitRequired]
	public class DebtCollectionClientCreateModel_V1
	{
		[Required]
		[DisplayName("Client name")]
		public string Name { get; set; }

		[Required]
		[DisplayName("Client ID number")]
		public string PersonalIdentificationNumber { get; set; }
	}
}
