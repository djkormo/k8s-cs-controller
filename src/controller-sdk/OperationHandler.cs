using System.Threading.Tasks;

namespace controller_sdk
{
	public interface OperationHandler<T> where T : BaseCRD
	{
		Task OnAdded(T crd);

		Task OnDeleted(T crd);

		Task OnUpdated(T crd);

		Task OnBookmarked(T crd);

		Task OnError(T crd);
	}
}
