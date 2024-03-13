using System;
using System.Collections.Generic;
using Game;
using Game.Interfaces;
using UI.Interaction;

namespace Managers.Interfaces
{
	public interface IGridManager
	{
		public bool CanMoveElements { get; }

		public event EventHandler OnCellCreated;
		public event EventHandler OnElementAdd;

		public void InitGame();
		public void StartGame();
		public void ResetGame();
		public List<ICell> CheckMatches(IEnumerable<ICell> cellsToCheck);
		public ICell GetNeighbourCell(ICell initElement, DragDirection dragDirection);
		public bool TrySwapElements(ICell startCell, ICell destinationElement, out List<ICell> elementsToDestroy);
		public void RemoveElementsFromGrid(List<ICell> cells);
		public List<ICell> FillGridGaps();
		public void BlockSwap(bool isBlocked);
	}
}