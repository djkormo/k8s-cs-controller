using System;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace controller_sdk
{
	public class Controller<T> where T : BaseCRD
	{
		public Func<T, Task> OnAdded { get; set; }
		public Func<T, Task> OnDeleted { get; set; }
		public Func<T, Task> OnUpdated { get; set; }

		private readonly Kubernetes m_kubernetes;
		private readonly T m_crd;
		private Watcher<T> m_watcher;
		public Controller(Kubernetes kubernetes, T crd)
		{
			m_kubernetes = kubernetes;
			m_crd = crd;
		}

		public Task SatrtAsync(CancellationToken token)
		{
			var listResponse = m_kubernetes.ListNamespacedCustomObjectWithHttpMessagesAsync(m_crd.Group, m_crd.Version, "", m_crd.Plural, watch: true);

			Task.Run(() =>
			{
				while (!token.IsCancellationRequested)
				{
					m_watcher = listResponse.Watch<T, object>(async (type, item) => await OnTChange(type, item));
				}
			});

			return Task.CompletedTask;
		}

		private async Task OnTChange(WatchEventType type, T item)
		{
			Console.WriteLine($"{typeof(T)} {item.Name()} {type} on Namespace {item.Namespace()}");

			switch (type)
			{
				case WatchEventType.Added:
					if (OnAdded != null)
						await OnAdded(item);
					return;
				case WatchEventType.Modified:
					if (OnUpdated != null)
						await OnUpdated(item);
					return;
				case WatchEventType.Deleted:
					if (OnDeleted != null)
						await OnDeleted(item);
					return;
				default:
					Console.WriteLine($"Don't know what to do with {type}");
					break;
			};
		}
	}
}
