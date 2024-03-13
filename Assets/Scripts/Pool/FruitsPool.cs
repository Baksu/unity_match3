using Game;
using Pool.Abstract;
using UnityEngine;
using UnityEngine.Pool;

namespace Pool
{
	public class FruitsPool : APoolObject<Piece>
	{
		private IObjectPool<Piece> _fruitsPool;

		private readonly GameObject _fruitPrefab;
		
		public FruitsPool(GameObject fruitPrefab)
		{
			_fruitPrefab = fruitPrefab;
		}
		
		protected override Piece CreateObject()
		{
			return Object.Instantiate(_fruitPrefab).GetComponent<Piece>();
		}
	}
}