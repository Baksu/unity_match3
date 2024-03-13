using System;

namespace Managers.Interfaces
{
	public interface ITimeManager
	{
		public event EventHandler OnTimeUpdate;
		public event EventHandler OnTimesUp;

		public void StartGame();
	}
}