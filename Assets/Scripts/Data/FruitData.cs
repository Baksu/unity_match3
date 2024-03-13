using System;
using Data.Interface;
using UnityEngine;

namespace Data
{
	[Serializable]
	public class FruitData : IFruitData
	{
		[SerializeField] private string _id;
		[SerializeField] private Sprite _sprite;

		public string Id => _id;
		public Sprite Sprite => _sprite;
	}
}