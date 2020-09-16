using System.Threading.Tasks;
using k8s;

namespace K8sControllerSDK
{
	public interface OperationHandler<T> where T : BaseCRD
	{
		Task OnAdded(Kubernetes k8s, T crd);

		Task OnDeleted(Kubernetes k8s, T crd);

		Task OnUpdated(T crd);

		Task OnBookmarked(T crd);

		Task OnError(T crd);
	}
}
