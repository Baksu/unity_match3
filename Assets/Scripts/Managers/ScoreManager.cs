using System;
using System.Linq;
using Managers.Interfaces;
using UnityEngine;

namespace Managers
{
	public class OnScoreUpdateEventArgs : EventArgs
	{
		public int CurrentScore { get; set; }
	}
	
	public class ScoreManager : IScoreManager
	{
		public event EventHandler OnScoreUpdate;
		
		private int _currentScore;

		public int GetScore() => _currentScore;

		private int CurrentScore
		{
			get => _currentScore;
			set
			{
				_currentScore = value;
				OnScoreUpdate?.Invoke(this, new OnScoreUpdateEventArgs{ CurrentScore = _currentScore});
			}
		}

		private readonly IDataManager _dataManager;

		public ScoreManager(IDataManager dataManager)
		{
			_dataManager = dataManager;
		}
		
		public void RemovedCells(int destroyedElements)
		{
			CurrentScore += _dataManager.PointsForOneCell * destroyedElements;
		}
		
		public void Match(int elementsInOneMatch)
		{
			CurrentScore += _dataManager.PointsForElementsMatched.Find(
				p => p.Elements == Mathf.Min(elementsInOneMatch, _dataManager.PointsForElementsMatched.Last().Elements)).Points;
		}
		
		public void ResetGame()
		{
			CurrentScore = 0;
		}
	}
}