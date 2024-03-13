using Managers.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class EndGameView : MonoBehaviour
	{
		[SerializeField] private Button _restartGame;
		[SerializeField] private TextMeshProUGUI _scoreText;

		private UiMainWindow _mainWindow;
		private IScoreManager _scoreManager;
		
		public void Init(UiMainWindow mainWindow, IScoreManager scoreManager)
		{
			_mainWindow = mainWindow;
			_scoreManager = scoreManager;
			
			_restartGame.onClick.AddListener(RestartGame);
		}

		public void GameOver()
		{
			_scoreText.SetText($"YOUR SCORE: {_scoreManager.GetScore()}");
		}

		private void RestartGame()
		{
			_mainWindow.RestartGame();
		}
	}
}