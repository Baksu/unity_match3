using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "new grid data", menuName = "Game Data/New Grid Data", order = 1)]
	public class GridDataSo : ScriptableObject
	{
		[SerializeField] private GridData _gridData;

		public GridData GridData => _gridData;
	}
}