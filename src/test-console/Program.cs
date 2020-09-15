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
			Kubernetes k8s = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());

			Controller<Foo> controller = new Controller<Foo>(k8s, new Foo());
			controller.OnAdded = Foo.FooAdded;
			controller.OnDeleted = Foo.FooDeleted;
			controller.OnUpdated = Foo.FooModified;
			controller.SatrtAsync(new System.Threading.CancellationToken());

			Console.WriteLine("Press <enter> to quit...");
			Console.ReadLine();
		}
	}
}
