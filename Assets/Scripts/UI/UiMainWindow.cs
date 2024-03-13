using System;
using Managers.Interfaces;
using UnityEngine;

namespace UI
{
	public class UiMainWindow : MonoBehaviour
	{
		[SerializeField] private StartView _startView;
		[SerializeField] private GameplayView _gameplayView;
		[SerializeField] private EndGameView _endGameView;

		private IGameManager _gameManager;
		
		public void Init(IScoreManager scoreManger, 
			IGameManager gameManager, 
			IGridManager gridManager, 
			IDataManager dataManager,
			ITimeManager timeManager)
		{
			_gameManager = gameManager;
			
			_startView.Init(this);
			_gameplayView.Init(gridManager, dataManager, scoreManger, timeManager);
			_endGameView.Init(this, scoreManger);

			_gameManager.OnIdleGame += OnIdleGame;
			_gameManager.OnStartGame += OnStartGame;
			_gameManager.OnGameOver += OnGameOver;
		}

		private void OnDestroy()
		{
			_gameManager.OnStartGame -= OnStartGame;
			_gameManager.OnGameOver -= OnGameOver;
		}

		public void StartGame()
		{
			_gameManager.StartGame();
		}

		public void RestartGame()
		{
			_gameManager.ResetGame();
		}

		private void OnIdleGame(object sender, EventArgs eventArgs)
		{
			_startView.gameObject.SetActive(true);
			_gameplayView.gameObject.SetActive(false);
			_endGameView.gameObject.SetActive(false);
		}
		
		private void OnStartGame(object sender, EventArgs eventArgs)
		{
			_gameplayView.gameObject.SetActive(true);
			_startView.gameObject.SetActive(false);
			_endGameView.gameObject.SetActive(false);
		}

		private void OnGameOver(object sender, EventArgs eventArgs)
		{
			_endGameView.GameOver();
			_endGameView.gameObject.SetActive(true);
			_gameplayView.gameObject.SetActive(false);
		}
	}
}