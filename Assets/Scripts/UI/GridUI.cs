using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using Game.Interfaces;
using Managers;
using Managers.Interfaces;
using UI.Interaction;
using UI.Interaction.Interfaces;
using UnityEngine;

namespace UI
{
	public class GridUI : MonoBehaviour
	{
		[SerializeField] private Transform _gridParent;
		[SerializeField] private Transform _elementsParent;

		private IGridManager _gridManager;
		private Vector2 _cellSize;
		
		private IMoveController _moveController;
		
		public void Init(IGridManager gridManager, IDataManager dataManager)
		{
			_gridManager = gridManager;
			_gridManager.OnCellCreated += OnCellCreated;
			_gridManager.OnElementAdd += OnElementAdd;
			
			_moveController = new MoveController(this, _gridManager);
			
			_cellSize = _gridParent.GetComponent<RectTransform>().rect.size / dataManager.GridData.GridSize;
		}

		private void OnCellCreated(object sender, EventArgs e)
		{
			if (e is not OnCellCreatedEventArgs eventArgs)
			{
				Debug.LogError("GridUi received wrong EventArgs during cell created");
				return;
			}
			
			var cell = eventArgs.Cell as Cell;

			var cellPosition = GetCellPosition(cell.CellMapPosition);
			cell.UpdatePosition(_gridParent, cellPosition);
			cell.SetupOnMove(_moveController.OnInteraction);
		}

		private Vector2 GetCellPosition(Vector2Int cellMapPosition)
		{
			return new Vector2(_cellSize.x * (cellMapPosition.x + 1), _cellSize.y * (cellMapPosition.y + 1)) - _cellSize/2;
		}
		
		private void OnElementAdd(object sender, EventArgs e)
		{
			if (e is not OnElementAddEventArgs eventArgs)
			{
				Debug.LogError("GridUi received wrong EventArgs during element add");
				return;
			}

			var fruit = eventArgs.GridElement as Piece;
			fruit.UpdatePosition(_elementsParent, GetCellPosition(eventArgs.InitCellElementPosition));
		}
		
		private void OnDestroy()
		{
			_gridManager.OnCellCreated -= OnCellCreated;
		}
		
		public bool TrySwapElements(ICell initCell, ICell neighbourCell)
		{
			if (!CheckIfElementsAreNeighbor(initCell, neighbourCell))
			{
				return false;
			}
			
			SwapElements(initCell, neighbourCell, gameObject.GetCancellationTokenOnDestroy()).Forget();
			return true;
		}
		
		private bool CheckIfElementsAreNeighbor(ICell firstCell, ICell secondCell)
		{
			return (firstCell.CellMapPosition.x == secondCell.CellMapPosition.x
			        && Mathf.Abs(firstCell.CellMapPosition.y - secondCell.CellMapPosition.y) == 1)
			       || (firstCell.CellMapPosition.y == secondCell.CellMapPosition.y
			           && Mathf.Abs(firstCell.CellMapPosition.x - secondCell.CellMapPosition.x) == 1);
		}
		
		//Ideal would be to invoke one method in the GridManager. Then GridManager do all actions at once invoking a events that GridUI listen.
		//Then GridUI create a animations and put them to a animation stack. But for test purpose I think below solution is enough.
		private async UniTaskVoid SwapElements(ICell initCell, ICell neighbourCell, CancellationToken ct)
		{
			_gridManager.BlockSwap(true);
			var initMove = initCell.CurrentElement.Move(neighbourCell.CellPrefabPosition);
			var neighbourMove = neighbourCell.CurrentElement.Move(initCell.CellPrefabPosition);
			
			var isSwapped = _gridManager.TrySwapElements(initCell, neighbourCell, out var elementsToDestroy);

			await UniTask.WhenAll(initMove, neighbourMove).AttachExternalCancellation(ct);
			
			//Check if we can start destroying elements or we need to swap again
			if (!isSwapped)
			{
				initMove = initCell.CurrentElement.Move(initCell.CellPrefabPosition);
				neighbourMove = neighbourCell.CurrentElement.Move(neighbourCell.CellPrefabPosition);
				
				await UniTask.WhenAll(initMove, neighbourMove);

				_gridManager.BlockSwap(false);
				return;
			}

			await RemoveElements(elementsToDestroy);
			List<ICell> updatedGaps = _gridManager.FillGridGaps();
			await UpdateGaps(updatedGaps);
			await CheckMatches(updatedGaps);
			
			_gridManager.BlockSwap(false);
		}

		private async UniTask RemoveElements(List<ICell> elementsToDestroy)
		{
			List<UniTask> destroyTasks = new List<UniTask>();
			foreach (ICell cell in elementsToDestroy)
			{
				destroyTasks.Add(cell.CurrentElement.DestroyElement());
			}
			
			_gridManager.RemoveElementsFromGrid(elementsToDestroy);

			await UniTask.WhenAll(destroyTasks);
		}

		private async UniTask UpdateGaps(List<ICell> updatedCell)
		{
			List<UniTask> updateTasks = new List<UniTask>();
			
			foreach (ICell cell in updatedCell)
			{
				if (cell.CurrentElement != null)
				{
					updateTasks.Add(cell.CurrentElement.Move(cell.CellPrefabPosition));
				}
			}
			
			await UniTask.WhenAll(updateTasks);
		}

		private async UniTask CheckMatches(List<ICell> cells)
		{
			List<ICell> cellsToDestroy = _gridManager.CheckMatches(cells);
			while (cellsToDestroy.Count > 0)
			{
				await RemoveElements(cellsToDestroy);
				List<ICell> updatedCells = _gridManager.FillGridGaps();
				await UpdateGaps(updatedCells);
				cellsToDestroy = _gridManager.CheckMatches(updatedCells);
			}
		}
	}
}