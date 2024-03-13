using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "new game data", menuName = "Game Data/New Game Data", order = 1)]
	public class GameDataSo : ScriptableObject
	{
		[SerializeField] private List<PieceDataSo> _pieceData;
		[SerializeField] private GridDataSo _gridData;
		[SerializeField] private int _pointsForCell;
		[SerializeField] private List<PointsForElementsMatched> _pointsForElementsMatched;
		[SerializeField] private int _gameplayTimeInSeconds;

		public List<PieceDataSo> PieceData => _pieceData;
		public GridDataSo GridData => _gridData;
		public int PointsForCell => _pointsForCell;
		public List<PointsForElementsMatched> PointsForElementsMatched => _pointsForElementsMatched;
		public int GameplayTimeInSeconds => _gameplayTimeInSeconds;
	}

	[Serializable]
	public class PointsForElementsMatched
	{
		[SerializeField] private int _elements;
		[SerializeField] private int _points;

		public int Elements => _elements;
		public int Points => _points;
	}
}