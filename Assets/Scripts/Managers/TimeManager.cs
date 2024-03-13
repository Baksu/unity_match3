using System;
using Cysharp.Threading.Tasks;
using Managers.Interfaces;

namespace Managers
{
	public class OnTimeUpdateEventArgs : EventArgs
	{
		public int TimeToEnd { get; set; }
	}
	
	public class TimeManager : ITimeManager
	{
		public event EventHandler OnTimeUpdate;
		public event EventHandler OnTimesUp;
		
		private int _timeToEnd;

		private int TimeToEnd
		{
			get => _timeToEnd;
			set
			{
				_timeToEnd = value;
				OnTimeUpdate?.Invoke(this, new OnTimeUpdateEventArgs{ TimeToEnd = _timeToEnd});
			}
		}

		private IDataManager _dataManager;

		public TimeManager(IDataManager dataManager)
		{
			_dataManager = dataManager;
		}

		private bool _isRunning;
		
		public void StartGame()
		{
			if (!_isRunning)
			{
				_isRunning = true;
				TimeToEnd = _dataManager.GameplayTimeInSeconds;
				Counting().Forget();
			}
		}

		private async UniTask Counting()
		{
			while (TimeToEnd > 0 && _isRunning)
			{
				await UniTask.Delay(1000); //wait one second
				TimeToEnd -= 1;
			}

			_isRunning = false;
			OnTimesUp?.Invoke(this, EventArgs.Empty);
		}
	}
}