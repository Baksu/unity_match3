using System;
using System.Collections.Generic;
using System.Linq;
using Data.Interface;
using Game;
using Game.Interfaces;
using JetBrains.Annotations;
using Managers.Interfaces;
using Pool.Interfaces;
using UI.Interaction;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Managers
{
	public class OnCellCreatedEventArgs : EventArgs
	{
		public ICell Cell { get; set; }
	}
	
	public class OnElementAddEventArgs : EventArgs
	{
		public IGridElement GridElement { get; set; }
		public Vector2Int InitCellElementPosition { get; set; } //We need this parameter to create elements above map when we need spawn new elements
	}
	
	public class GridManager : IGridManager
	{
		public event EventHandler OnCellCreated;
		public event EventHandler OnElementAdd;
		
		private readonly IDataManager _dataManager;
		private readonly IScoreManager _scoreManager;
		private readonly IPoolManager<Piece> _fruitPool;
		
		private ICell[,] _cellData;
		private int _gridSize;
		private bool _isSwapBlocked;

		public bool CanMoveElements => !_isSwapBlocked;
		
		public GridManager(IDataManager dataManager, IScoreManager scoreManager, IPoolManager<Piece> fruitPool)
		{
			_dataManager = dataManager;
			_scoreManager = scoreManager;
			_fruitPool = fruitPool;
		}

		public void InitGame()
		{
			GenerateGrid(_dataManager.GridData.GridSize); 
		}
		
		public void StartGame()
		{
			PopulateGridWithElements();
			FirstCleanGrid();
		}

		public void ResetGame()
		{
			for (int column = 0; column < _cellData.GetLength(0); column++)
			{
				for (int line = 0; line < _cellData.GetLength(1); line++)
				{
					_cellData[column, line].Reset();
				}
			}
		}
		
		private void GenerateGrid(int gridSize)
		{
			_cellData = new ICell[gridSize, gridSize];
			
			for (int column = 0; column < gridSize; column++)
			{
				for (int line = 0; line < gridSize; line++)
				{
					ICell cell = Object.Instantiate(_dataManager.GridData.CellPrefab);
					cell.Init(column, line);
					_cellData[column, line] = cell;
					OnCellCreated?.Invoke(this,new OnCellCreatedEventArgs
					{
						Cell = cell
					});
				}
			}
		}
		
		private void PopulateGridWithElements()
		{
			for (int column = 0; column < _cellData.GetLength(0); column++)
			{
				for (int line = 0; line < _cellData.GetLength(1); line++)
				{
					var cell = _cellData[column, line];
					
					AddElementToCell(cell, cell.CellMapPosition);
				}
			}
		}

		private void FirstCleanGrid() //this could be better optimize
		{
			for (int column = 0; column < _cellData.GetLength(0); column++)
			{
				for (int line = 0; line < _cellData.GetLength(1); line++)
				{
					var startCell = _cellData[column, line];

					List<ICell> cells = CheckMatches(startCell, false);
					
					while (cells.Count > 0) 
					{
						foreach (ICell cell in cells)
						{
							cell.HorizontalChecked = false;
							cell.VerticalChecked = false;
						}

						startCell.CurrentElement.DestroyElement(true);
						
						AddElementToCell(startCell, startCell.CellMapPosition);
						cells = CheckMatches(startCell, false);
					}
				}
			}
		}
		
		private void AddElementToCell(ICell cell, Vector2Int initCellMapPosition)
		{
			var piecesData = _dataManager.GridElementData;

			IGridElement gridElement = CreateGridElement(piecesData);
			cell.UpdateCurrentElement(gridElement);
			
			var eventArgs = new OnElementAddEventArgs
			{
				GridElement = gridElement,
				InitCellElementPosition = initCellMapPosition
			};
			
			OnElementAdd?.Invoke(this, eventArgs);
		}

		private IGridElement CreateGridElement(List<IGridElementData> piecesData)
		{
			var randomPiece = piecesData[Random.Range(0, piecesData.Count)];
			IGridElement gridElement = _fruitPool.GetObject();
			gridElement.Init(randomPiece, OnRemoveGridElement);
			return gridElement;
		}

		private void OnRemoveGridElement(IGridElement gridElement)
		{
			_fruitPool.ReleaseObject((Piece)gridElement);
		}

		public ICell GetNeighbourCell(ICell initCell, DragDirection dragDirection)
		{
			if (!IsDirectionValid(initCell.CellMapPosition, dragDirection))
			{
				return null;
			}
		
			if (GetDestinationCell(initCell.CellMapPosition, dragDirection) is not { } destinationCell)
			{
				return null;
			}

			if (initCell.CurrentElement == null || destinationCell.CurrentElement == null)
			{
				return null;
			}
			
			return destinationCell;
		}

		public void RemoveElementsFromGrid(List<ICell> cells)
		{
			_scoreManager.RemovedCells(cells.Count);
			foreach (ICell cell in cells)
			{
				cell.UpdateCurrentElement(null);
				cell.VerticalChecked = false;
				cell.HorizontalChecked = false;
			}
		}
		
		public bool TrySwapElements(ICell startCell, ICell destinationCell, out List<ICell> elementsToDestroy)
		{
			SwapElements(startCell, destinationCell);
			elementsToDestroy = CheckMatches(new List<ICell>{ startCell, destinationCell });

			if (elementsToDestroy.Count <= 0)
			{
				SwapElements(startCell, destinationCell);
				return false;
			}

			return true;
		}

		private void SwapElements(ICell initCell, ICell destinationCell)
		{
			var initCellElement = initCell.CurrentElement;
			var destinationCellElement = destinationCell.CurrentElement;

			initCell.UpdateCurrentElement(destinationCellElement);
			destinationCell.UpdateCurrentElement(initCellElement);
		}
		
		public List<ICell> FillGridGaps()
		{
			List<ICell> updatedCells = new List<ICell>();
			for (int column = 0; column < _cellData.GetLength(0); column++)
			{
				int lineToFillCounter = 0;
				for (int line = 0; line < _cellData.GetLength(1); line++)
				{
					ICell currentCheckingCell = _cellData[column, line];
					if (currentCheckingCell.CurrentElement == null)
					{
						ICell occupyCell = FindOccupyUpCell(column, line);
						if (occupyCell == null)
						{
							//Thanks this we can spawn elements above map and move them into it
							var newPositionY = _cellData.GetLength(1) + lineToFillCounter;
							var initCellMapPosition = new Vector2Int(currentCheckingCell.CellMapPosition.x, newPositionY);
							AddElementToCell(currentCheckingCell, initCellMapPosition);
							lineToFillCounter++;
						}
						else
						{
							currentCheckingCell.UpdateCurrentElement(occupyCell.CurrentElement);
							occupyCell.UpdateCurrentElement(null);
						}
						updatedCells.Add(currentCheckingCell);
					}
				}
			}

			return updatedCells;
		}

		private ICell FindOccupyUpCell(int column, int line)
		{
			for (int i = line + 1; i < _cellData.GetLength(1); i++)
			{
				ICell cell = _cellData[column, i];
				if (cell.CurrentElement != null)
				{
					return cell;
				}
			}

			return null;
		}

		public List<ICell> CheckMatches(IEnumerable<ICell> cellsToCheck)
		{
			List<ICell> cellsToDestroy = new List<ICell>();
			foreach (var cell in cellsToCheck)
			{
				cellsToDestroy.AddRange(CheckMatches(cell));
			}

			return cellsToDestroy.Distinct().ToList();
		}
		
		private List<ICell> CheckMatches(ICell startCell, bool countMatch = true)
		{
			List<ICell> horizontalCells = new List<ICell>();
			List<ICell> verticalCells = new List<ICell>();

			if (!startCell.HorizontalChecked)
			{
				GetNeighbourCellIfMatch(startCell, DragDirection.Left, horizontalCells);
				GetNeighbourCellIfMatch(startCell, DragDirection.Right, horizontalCells);
			}

			if (!startCell.VerticalChecked)
			{
				GetNeighbourCellIfMatch(startCell, DragDirection.Up, verticalCells);
				GetNeighbourCellIfMatch(startCell, DragDirection.Down, verticalCells);
			}

			List<ICell> cellToDestroy = new List<ICell>();
			
			if (horizontalCells.Count >= _dataManager.GridData.MinElementsToMatch - 1)
			{
				foreach (ICell cell in verticalCells)
				{
					cell.HorizontalChecked = true;
				}

				startCell.HorizontalChecked = true;
				cellToDestroy.AddRange(horizontalCells);
			}

			if (verticalCells.Count >= _dataManager.GridData.MinElementsToMatch - 1)
			{
				foreach (ICell cell in verticalCells)
				{
					cell.VerticalChecked = true;
				}

				startCell.VerticalChecked = true;
				cellToDestroy.AddRange(verticalCells);
			}

			if (cellToDestroy.Count > 0)
			{
				cellToDestroy.Add(startCell);
				if (countMatch)
				{
					_scoreManager.Match(cellToDestroy.Count);
				}
			}

			return cellToDestroy;
		}
		
		private void GetNeighbourCellIfMatch(ICell startCell, DragDirection direction, List<ICell> cellList)
		{
			if (!IsDirectionValid(startCell.CellMapPosition, direction))
			{
				return;
			} 
			
			var nextCell = GetDestinationCell(startCell.CellMapPosition, direction);

			if (nextCell != null && 
			    startCell.CurrentElement != null &&
			    nextCell.CurrentElement != null && 
			    startCell.CurrentElement.Id == nextCell.CurrentElement.Id)
			{
				cellList.Add(nextCell);
				GetNeighbourCellIfMatch(nextCell, direction, cellList); //recursive here was quicker but it would be good to create this iterative. Rider can change this automatic but for readibility I left it like that 
			}
		}


		private bool IsDirectionValid(Vector2Int cellMapPosition, DragDirection dragDirection)
		{
			switch (dragDirection)
			{
				case DragDirection.None:
					break;
				case DragDirection.Left:
					if (cellMapPosition.x > 0)
					{
						return true;
					}
					break;
				case DragDirection.Up:
					if (cellMapPosition.y < _cellData.GetLength(0) - 1)
					{
						return true;
					}
					break;
				case DragDirection.Right:
					if (cellMapPosition.x < _cellData.GetLength(1) - 1)
					{
						return true;
					}
					break;
				case DragDirection.Down:
					if (cellMapPosition.y > 0 )
					{
						return true;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(dragDirection), dragDirection, null);
			}

			return false;
		}

		[CanBeNull]
		private ICell GetDestinationCell(Vector2Int cellMapPosition, DragDirection dragDirection)
		{
			switch (dragDirection)
			{
				case DragDirection.None:
					return null;
				case DragDirection.Left:
					return _cellData[cellMapPosition.x - 1, cellMapPosition.y]; 
				case DragDirection.Up:
					return _cellData[cellMapPosition.x, cellMapPosition.y + 1]; 
				case DragDirection.Right:
					return _cellData[cellMapPosition.x + 1, cellMapPosition.y]; 
				case DragDirection.Down:
					return _cellData[cellMapPosition.x, cellMapPosition.y - 1]; 
				default:
					throw new ArgumentOutOfRangeException(nameof(dragDirection), dragDirection, null);
			}
		}

		public void BlockSwap(bool isBlocked)
		{
			_isSwapBlocked = isBlocked;
		}
	}
}