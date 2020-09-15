using System;
using System.Threading.Tasks;
using controller_sdk;

namespace foo
{
	public class Foo : BaseCRD
	{
		public Foo(string group, string version, string plural, string singular) :
			base(group, version, plural, singular)
		{ }

		public Foo() :
			this("demos.fearofoblivion.com", "v1", "foos", "foo")
		{ }

		public FooSpec Spec { get; set; }

		public static async Task FooAdded(Foo f)
		{
			Console.WriteLine($"Foo added!{f.Spec}");
		}

		public static async Task FooDeleted(Foo f)
		{
			Console.WriteLine($"Foo deleted!{f.Spec}");
		}

		public static async Task FooModified(Foo f)
		{
			Console.WriteLine($"Foo modified!{f.Spec}");
		}

		public class FooSpec
		{
			public string Value1 { get; set; }
			public string Value2 { get; set; }
			public int Value3 { get; set; }
			public int Value4 { get; set; }

			public override string ToString()
			{
				return $"Value1:{Value1}{Environment.NewLine}Value2:{Value2}{Environment.NewLine}Value3:{Value3}{Environment.NewLine}Value4:{Value4}";
			}
		}


	}
}
