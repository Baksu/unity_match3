using System;
using Game;
using Game.Interfaces;
using Managers.Interfaces;
using UI.Interaction.Interfaces;
using UnityEngine;

namespace UI.Interaction
{
	//The logic for each MoveComponent can be extract to separate classes
	public class MoveController : IMoveController
	{
		private GridUI _gridUI;
		private IGridManager _gridManager;

		private ICell _currentCell;
		
		public MoveController(GridUI gridUI, IGridManager gridManager)
		{
			_gridUI = gridUI;
			_gridManager = gridManager;
		}
		
		public void OnInteraction(object sender, EventArgs eventArgs)
		{
			if (!_gridManager.CanMoveElements)
			{
				return;
			}
			
			if (sender is not ICell cell)
			{
				return;
			}
			
			switch (eventArgs)
			{
				case OnDragDirectionArgs ondragDirectionArgs:
					TurnOffCurrenCell();
					Drag(cell, ondragDirectionArgs);
					return;
				case OnTapEventsArgs:
					Tap(cell);
					return;
			}
		}

		private void Drag(ICell cell, OnDragDirectionArgs ondragDirectionArgs)
		{
			var neighbourCell = _gridManager.GetNeighbourCell(cell, ondragDirectionArgs.DragDirection);

			if (neighbourCell == null)
			{
				return;
			}
			
			_gridUI.TrySwapElements(cell, neighbourCell);
		}

		private void Tap(ICell cell)
		{
			if (cell is not ITapElement tappedCell)
			{
				Debug.LogError("ICell is not tappable");
				return;
			}
					
			if (_currentCell == null)
			{
				_currentCell = cell;
				tappedCell.TapElement(true);
				return;
			}
			
			if (_currentCell == tappedCell)
			{
				tappedCell.TapElement(false);
				_currentCell = null;
				return;
			}

			if (_gridUI.TrySwapElements(_currentCell, cell))
			{
				TurnOffCurrenCell();
				return;
			}

			TurnOffCurrenCell();
			tappedCell.TapElement(true);
			_currentCell = cell;
		}

		private void TurnOffCurrenCell()
		{
			if (_currentCell == null)
			{
				return;
			}
			(_currentCell as ITapElement).TapElement(false);
			_currentCell = null;
		}
	}
}