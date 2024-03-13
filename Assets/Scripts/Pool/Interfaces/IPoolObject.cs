namespace Pool.Interfaces
{
	public interface IPoolObject
	{
		public void AfterGet();
		public void BeforeRelease();
	}
}