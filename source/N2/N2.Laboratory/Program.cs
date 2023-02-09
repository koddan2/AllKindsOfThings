using N2.Story;
using Base62;
using Stateless.Graph;
using System.Diagnostics;

//var story = new Story(null!);
//var info = Story.FsmTest();
//var dot = UmlDotGraph.Format(info);
//Console.WriteLine(dot);

for (int i = 0; i < 1000; i++)
{
	var guid = Guid.NewGuid();
	var ba = guid.ToByteArray();
	var b62 = ba.ToBase62();
	Console.WriteLine(b62);
	var ba2 = b62.FromBase62();
	var guid2 = new Guid(ba2);
	Debug.Assert(guid2 == guid, "must be reversible");
}
