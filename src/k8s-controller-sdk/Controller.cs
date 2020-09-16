using System;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace K8sControllerSDK
{
	public class Controller<T> where T : BaseCRD
	{
		private readonly OperationHandler<T> m_handler;
		private readonly Kubernetes m_kubernetes;
		private readonly T m_crd;
		private Watcher<T> m_watcher;

		public Controller(T crd, OperationHandler<T> handler)
		{
			m_kubernetes = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());
			m_crd = crd;
			m_handler = handler;
		}

		~Controller()
		{
			DisposeWatcher();
		}

		public Task SatrtAsync(CancellationToken token, string k8sNamespace = "")
		{
			var listResponse = m_kubernetes.ListNamespacedCustomObjectWithHttpMessagesAsync(m_crd.Group, m_crd.Version, k8sNamespace, m_crd.Plural, watch: true);

			return Task.Run(() =>
			{
				//while (!token.IsCancellationRequested)
				{
					m_watcher = listResponse.Watch<T, object>(async (type, item) => await OnTChange(type, item));
				}

				DisposeWatcher();

			});

			//return Task.CompletedTask;
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
						await m_handler.OnAdded(m_kubernetes, item);
					return;
				case WatchEventType.Modified:
					if (m_handler != null)
						await m_handler.OnUpdated(item);
					return;
				case WatchEventType.Deleted:
					if (m_handler != null)
						await m_handler.OnDeleted(m_kubernetes, item);
					return;
				case WatchEventType.Bookmark:
					if (m_handler != null)
						await m_handler.OnBookmarked(item);
					return;
				case WatchEventType.Error:
					if (m_handler != null)
						await m_handler.OnError(item);
					return;
				default:
					Console.WriteLine($"Don't know what to do with {type}");
					break;
			};
		}
	}
}
