using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "new piece data", menuName = "Game Data/New Piece Data", order = 1)]
	public class PieceDataSo : ScriptableObject
	{
		[SerializeField] private FruitData _fruitData;

		public FruitData FruitData => _fruitData;
	}
}