using UnityEngine;

namespace Game.Interfaces
{
	public interface ICell
	{
		public Vector2Int CellMapPosition { get; }
		public Vector2 CellPrefabPosition { get; }
		public IGridElement CurrentElement { get; }
		
		public bool HorizontalChecked { get; set; }
		public bool VerticalChecked { get; set; }

		public void Init(int column, int line);
		public void Reset();
		
		public void UpdateCurrentElement(IGridElement currentElement);
	}
}