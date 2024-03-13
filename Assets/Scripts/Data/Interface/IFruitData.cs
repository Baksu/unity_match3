using UnityEngine;

namespace Data.Interface
{
	public interface IFruitData : IGridElementData
	{
		public Sprite Sprite { get; }
	}
}