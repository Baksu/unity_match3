using System;

namespace Managers.Interfaces
{
	public interface IScoreManager
	{
		public event EventHandler OnScoreUpdate;
		public void RemovedCells(int destroyedElements);
		public void Match(int elementsInOneMatch);
		public int GetScore();
		public void ResetGame();
	}
}