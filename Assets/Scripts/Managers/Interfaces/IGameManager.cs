using System;

namespace Managers.Interfaces
{
	public interface IGameManager
	{
		public event EventHandler OnIdleGame;
		public event EventHandler OnStartGame;
		public event EventHandler OnGameOver;

		public void IdleGame();
		public void StartGame();
		public void ResetGame();
	}
}