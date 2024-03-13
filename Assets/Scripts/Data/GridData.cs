using System;
using Data.Interface;
using Game;
using UnityEngine;

namespace Data
{
	[Serializable]
	public class GridData : IGridData
	{
		[SerializeField] private int _gridSize = 9;
		[SerializeField] private Cell _cellPrefab;
		[SerializeField] private GameObject _elementPrefab;
		[SerializeField] private int _minElementsToMatch;

		public int GridSize => _gridSize;
		public Cell CellPrefab => _cellPrefab;
		public GameObject FruitPrefab => _elementPrefab;
		public int MinElementsToMatch => _minElementsToMatch;
	}
}