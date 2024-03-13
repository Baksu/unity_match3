using System;
using Managers;
using Managers.Interfaces;
using TMPro;
using UnityEngine;

namespace UI
{
	public class GameplayView : MonoBehaviour
	{
		[SerializeField] private GridUI _gridUi;
		[SerializeField] private TextMeshProUGUI _scoreText;
		[SerializeField] private TextMeshProUGUI _timeText;

		private IScoreManager _scoreManager;
		private ITimeManager _timeManager;
		
		public void Init(IGridManager gridManager, IDataManager dataManager, IScoreManager scoreManager, ITimeManager timeManager)
		{
			_gridUi.Init(gridManager, dataManager);
			
			_scoreManager = scoreManager;
			_scoreManager.OnScoreUpdate += OnScoreUpdate;

			_timeManager = timeManager;
			_timeManager.OnTimeUpdate += OnTimeUpdate;
		}
		
		private void OnDestroy()
		{
			_scoreManager.OnScoreUpdate -= OnScoreUpdate;
			_timeManager.OnTimeUpdate -= OnTimeUpdate;
		}

		private void OnScoreUpdate(object sender, EventArgs e)
		{
			if (e is not OnScoreUpdateEventArgs eventArgs)
			{
				Debug.LogError("GridUi received wrong EventArgs during score update");
				return;
			}
			
			_scoreText.SetText($"SCORE: {eventArgs.CurrentScore}");
		}
		
		private void OnTimeUpdate(object sender, EventArgs e)
		{
			if (e is not OnTimeUpdateEventArgs eventArgs)
			{
				Debug.LogError("GridUi received wrong EventArgs during time update");
				return;
			}
			
			_timeText.SetText($"Time: {eventArgs.TimeToEnd}");
		}
	}
}