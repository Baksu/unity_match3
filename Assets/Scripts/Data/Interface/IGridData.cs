using Game;
using UnityEngine;

namespace Data.Interface
{
	public interface IGridData
	{
		public int GridSize { get; }
		public Cell CellPrefab { get; }
		public GameObject FruitPrefab { get; }
		public int MinElementsToMatch { get; }
	}
}