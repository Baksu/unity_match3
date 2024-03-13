using System.Collections.Generic;
using Data;
using Data.Interface;

namespace Managers.Interfaces
{
	public interface IDataManager
	{
		public List<IGridElementData> GridElementData { get; }
		public IGridData GridData { get; }
		public int PointsForOneCell { get; }
		public List<PointsForElementsMatched> PointsForElementsMatched { get; }
		public int GameplayTimeInSeconds { get; }
	}
}