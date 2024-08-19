namespace Core
{
	public interface IObserver
	{
		void OnObjectChanged(Observable observable);
	}
}