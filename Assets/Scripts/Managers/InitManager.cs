using Managers.Interfaces;
using Pool;
using UI;
using UnityEngine;

namespace Managers
{
	public class InitManager : MonoBehaviour
	{
		[SerializeField] private UiMainWindow _mainWindow;
		[SerializeField] private DataManager _dataManager;

		private IGameManager _gameManager;
		
		private void Start()
		{
			LoadGame();
		}

		private void LoadGame()
		{
			CreateManagers();
			EndLoad();
		}

		private void CreateManagers() //Here I'm using simple injection because it's small project but it can be done by some plugin like zenject
		{
			_dataManager.Init();
			var fruitsPool = new FruitsPool(_dataManager.GridData.FruitPrefab);
			
			var timeManager = new TimeManager(_dataManager);
			var scoreManager = new ScoreManager(_dataManager);
			var gridManager = new GridManager(_dataManager, scoreManager, fruitsPool);
			
			_gameManager = new GameManager(timeManager, gridManager, scoreManager);
			
			_mainWindow.Init(scoreManager, _gameManager, gridManager, _dataManager, timeManager);
		}

		private void EndLoad()
		{
			_gameManager.IdleGame();
		}
	}
}