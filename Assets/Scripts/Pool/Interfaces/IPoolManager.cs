namespace Pool.Interfaces
{
	public interface IPoolManager<T> where T: class, IPoolObject
	{
		public T GetObject();
		public void ReleaseObject(T poolObject);
	}
}