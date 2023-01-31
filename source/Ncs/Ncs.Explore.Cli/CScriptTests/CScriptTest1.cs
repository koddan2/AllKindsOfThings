using Microsoft.CodeAnalysis.CSharp.Scripting;
using Serilog;
using System.Diagnostics;

namespace Ncs.Explore.Cli.CScriptTests
{
	internal class CScriptTest1
	{
		public void Test1()
		{
			//var a = 1 + new Random().NextDouble();

			var outersw = Stopwatch.StartNew();
			for (int i = 0; i < 100; i++)
			{
				var innersw = Stopwatch.StartNew();
				_ = CSharpScript.RunAsync("1 + new System.Random().NextDouble()");
				Log.Information("Iteration {i} {time}", i, innersw.Elapsed);
			}
			Log.Information("Total {time}", outersw.Elapsed);

			outersw = Stopwatch.StartNew();
			var script = CSharpScript.Create("1 + new System.Random().NextDouble()");
			script.Compile();
			var scriptDel = script.CreateDelegate();
			for (int i = 0; i < 100; i++)
			{
				var innersw = Stopwatch.StartNew();
				_ = scriptDel();
				Log.Information("Iteration {i} {time}", i, innersw.Elapsed);
			}
			Log.Information("Total {time}", outersw.Elapsed);
		}
	}
}
