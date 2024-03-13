using System;
using Cysharp.Threading.Tasks;
using Data.Interface;
using DG.Tweening;
using Game.Interfaces;
using Pool.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class Piece : MonoBehaviour, IGridElement, IPoolObject
	{
		private static float MoveSpeed = 0.3f;
		private static float FadeSpeed = 0.3f;
		private static Vector3 DestroyScale = new Vector3(2, 2, 2);

		[SerializeField] private Image _image;
		
		private IFruitData _fruitData;
		private Action<IGridElement> _onRemoveElement;
		public string Id => _fruitData.Id;

		public void Init<TGridElementData>(TGridElementData data, Action<IGridElement> onRemoveElement) 
			where TGridElementData : IGridElementData
		{
			_fruitData = (IFruitData)data;
			_onRemoveElement = onRemoveElement;

			if (_fruitData == null)
			{
				Debug.LogError("Wrong data pass to the Piece object");
			}
			
			_image.sprite = _fruitData.Sprite;
		}
		
		//for more type elements we can create abstract class from Fruits and this method will be there
		public void UpdatePosition(Transform parent, Vector2 position)
		{
			transform.SetParent(parent, false);
			transform.localPosition =position;
		}

		public async UniTask Move(Vector2 newPosition)
		{
			var ct = gameObject.GetCancellationTokenOnDestroy();
			await transform.DOLocalMove(newPosition, MoveSpeed).SetEase(Ease.InOutBack).WithCancellation(ct);
		}

		public async UniTask DestroyElement(bool immediate = false)
		{
			if (!immediate)
			{
				var ct = gameObject.GetCancellationTokenOnDestroy();

				Sequence sequence = DOTween.Sequence();

				sequence.Join(_image.DOFade(0, FadeSpeed));
				sequence.Join(transform.DOScale(DestroyScale, FadeSpeed));

				await sequence.Play().WithCancellation(ct);
			}

			_onRemoveElement?.Invoke(this);
		}

		public void AfterGet()
		{
			gameObject.SetActive(true);
		}

		public void BeforeRelease()
		{
			gameObject.SetActive(false);
			_image.DOFade(1, 0);
			transform.localScale = Vector3.one;
		}
	}
}