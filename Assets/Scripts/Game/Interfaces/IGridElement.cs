using System;
using Cysharp.Threading.Tasks;
using Data.Interface;
using UnityEngine;

namespace Game.Interfaces
{
	public interface IGridElement  
	{
		public string Id { get; }

		public void Init<TGridElementData>(TGridElementData data, Action<IGridElement> onRemoveElement)
			where TGridElementData : IGridElementData;
		public UniTask Move(Vector2 newPosition);
		public UniTask DestroyElement(bool immediate = false);
	}
}