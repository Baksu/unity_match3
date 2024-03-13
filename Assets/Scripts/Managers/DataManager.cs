using System.Collections.Generic;
using Data;
using Data.Interface;
using Managers.Interfaces;
using UnityEngine;

namespace Managers
{
	public class DataManager : MonoBehaviour, IDataManager
	{
		[SerializeField] private GameDataSo _gameData;

		private List<IGridElementData> _pieceData;
		public List<IGridElementData> GridElementData => _pieceData;
		public IGridData GridData => _gameData.GridData.GridData;
		public int PointsForOneCell => _gameData.PointsForCell;
		public List<PointsForElementsMatched> PointsForElementsMatched => _gameData.PointsForElementsMatched;
		public int GameplayTimeInSeconds => _gameData.GameplayTimeInSeconds;

		public void Init()
		{
			LoadPieceData();
		}

		private void LoadPieceData()
		{
			_pieceData = new List<IGridElementData>();
			foreach (var pieceDataSo in _gameData.PieceData)
			{
				_pieceData.Add(pieceDataSo.FruitData);
			}
		}
	}
}