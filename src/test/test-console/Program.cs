using System;
using controller_sdk;
using foo;
using k8s;

namespace test_console
{
	class Program
	{
		static void Main(string[] args)
		{
			//Kubernetes k8s = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());

			Controller<Foo> controller = new Controller<Foo>(new Foo())
			{
				//OnAdded = Foo.FooAdded,
				//OnDeleted = Foo.FooDeleted,
				//OnUpdated = Foo.FooModified
			};
			controller.SatrtAsync(new System.Threading.CancellationToken());

			Console.WriteLine("Press <enter> to quit...");
			Console.ReadLine();
		}
	}
}
