using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Managers.Interfaces;

namespace Managers
{
	
	public class GameManager : IGameManager, IDisposable
	{
		public event EventHandler OnIdleGame;
		public event EventHandler OnStartGame;
		public event EventHandler OnGameOver;
		
		private readonly IGridManager _gridManager;
		private readonly ITimeManager _timeManager;
		private readonly IScoreManager _scoreManager;
		
		private CancellationTokenSource _cancellationToken;
		
		public GameManager(
			ITimeManager timeManager,
			IGridManager gridManager,
			IScoreManager scoreManager)
		{
			_timeManager = timeManager;
			_timeManager.OnTimesUp += OnTimesUp;

			_gridManager = gridManager;
			_scoreManager = scoreManager;
		}
		
		public void IdleGame()
		{
			_gridManager.InitGame();
			OnIdleGame?.Invoke(this, EventArgs.Empty);
		}
		
		public void StartGame()
		{
			_gridManager.StartGame();
			_timeManager.StartGame();
			OnStartGame?.Invoke(this, EventArgs.Empty);
		}
		
		public void ResetGame()
		{
			_gridManager.ResetGame();
			_scoreManager.ResetGame();
			StartGame();
		}

		private void OnTimesUp(object sender, EventArgs eventArgs)
		{
			WaitForFinishMoving().Forget();
		}

		private async UniTaskVoid WaitForFinishMoving()
		{
			_cancellationToken = new CancellationTokenSource();
			await UniTask.WaitUntil(() => _gridManager.CanMoveElements, cancellationToken: _cancellationToken.Token);
			OnGameOver?.Invoke(this, EventArgs.Empty);
		}
		
		public void Dispose()
		{
			_cancellationToken?.Cancel();
			_timeManager.OnTimesUp -= OnTimesUp;
		}
	}
}