using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class StartView : MonoBehaviour
	{
		[SerializeField] private Button _startButton;

		private UiMainWindow _mainWindow;
		
		public void Init(UiMainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			
			_startButton.onClick.AddListener(StartGame);
		}

		private void StartGame()
		{
			_mainWindow.StartGame();
		}
	}
}