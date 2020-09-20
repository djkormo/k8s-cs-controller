using System;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace K8sControllerSDK
{
	public class Controller<T> where T : BaseCRD
	{
		public Kubernetes Kubernetes { get; private set; }

		private readonly IOperationHandler<T> m_handler;
		private readonly T m_crd;
		private Watcher<T> m_watcher;

		public Controller(T crd, IOperationHandler<T> handler)
		{
			Kubernetes = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());
			m_crd = crd;
			m_handler = handler;
		}

		~Controller()
		{
			DisposeWatcher();
		}

		public Task SatrtAsync(CancellationToken token, string k8sNamespace = "")
		{
			var listResponse = Kubernetes.ListNamespacedCustomObjectWithHttpMessagesAsync(m_crd.Group, m_crd.Version, k8sNamespace, m_crd.Plural, watch: true);

			return Task.Run(() =>
			{
				//while (!token.IsCancellationRequested)
				{
					m_watcher = listResponse.Watch<T, object>(async (type, item) => await OnTChange(type, item));
				}

				DisposeWatcher();

			});
		}

		void DisposeWatcher()
		{
			if (m_watcher != null && m_watcher.Watching)
				m_watcher.Dispose();
		}

		private async Task OnTChange(WatchEventType type, T item)
		{
			Console.WriteLine($"{typeof(T)} {item.Name()} {type} on Namespace {item.Namespace()}");

			switch (type)
			{
				case WatchEventType.Added:
					if (m_handler != null)
						await m_handler.OnAdded(Kubernetes, item);
					return;
				case WatchEventType.Modified:
					if (m_handler != null)
						await m_handler.OnUpdated(Kubernetes, item);
					return;
				case WatchEventType.Deleted:
					if (m_handler != null)
						await m_handler.OnDeleted(Kubernetes, item);
					return;
				case WatchEventType.Bookmark:
					if (m_handler != null)
						await m_handler.OnBookmarked(Kubernetes, item);
					return;
				case WatchEventType.Error:
					if (m_handler != null)
						await m_handler.OnError(Kubernetes, item);
					return;
				default:
					Console.WriteLine($"Don't know what to do with {type}");
					break;
			};
		}
	}
}
