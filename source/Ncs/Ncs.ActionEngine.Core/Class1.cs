using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Runtime.Serialization;

namespace Ncs.ActionEngine.Core
{
	public interface IActionContext
	{

	}

	public interface IAction<TContext>
		where TContext : IActionContext
	{
		public string Logic { get; set; }

		[IgnoreDataMember]
		public Script<TContext>? CompiledScript { get; set; }
	}

	public class Processor
	{
		public void Process()
		{
			var state = CSharpScript.Create("1+1");
		}
	}
}