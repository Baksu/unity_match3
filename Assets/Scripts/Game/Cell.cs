using System;
using System.Collections.Generic;
using System.Linq;
using Game.Interfaces;
using UI.Interaction.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class Cell : MonoBehaviour, ICell, ITapElement
	{
		[SerializeField] private Image _background;
		[SerializeField] private Color _normalColor;
		[SerializeField] private Color _tapedColor;
		
		private Vector2Int _cellMapPosition;
		public Vector2Int CellMapPosition => _cellMapPosition;
		private Vector2 _cellPrefabPosition;
		public Vector2 CellPrefabPosition => _cellPrefabPosition;
		public IGridElement CurrentElement { get; private set; }
		public bool HorizontalChecked { get; set; }
		public bool VerticalChecked { get; set; }

		public ICell GetInteractCell => this;
		
		private List<IMoveComponent> _movableComponents;
		private EventHandler _onMoveEventHandler;
		
		private void Awake()
		{
			_movableComponents = GetComponents<IMoveComponent>().ToList();
			
			foreach (var movableComponent in _movableComponents)
			{
				movableComponent.OnMoveEventHandler += OnMove;
			}
		}
		
		private void OnDestroy()
		{
			foreach (var movableComponent in _movableComponents)
			{
				movableComponent.OnMoveEventHandler -= OnMove;
			}
		}
		
		public void Init(int column, int line)
		{
			_cellMapPosition = new Vector2Int(column, line);
		}

		public void Reset()
		{
			CurrentElement.DestroyElement(true);
			UpdateCurrentElement(null);
			HorizontalChecked = false;
			VerticalChecked = false;
		}

		public void UpdatePosition(Transform parent, Vector2 position)
		{
			transform.SetParent(parent, false);
			transform.localPosition =position;
			_cellPrefabPosition = position;
		}

		public void SetupOnMove(EventHandler onMoveEventHandler)
		{
			_onMoveEventHandler = onMoveEventHandler;
		}
		
		public void UpdateCurrentElement(IGridElement currentElement)
		{
			CurrentElement = currentElement;
		}

		private void OnMove(object sender, EventArgs eventArgs)
		{
			_onMoveEventHandler?.Invoke(this, eventArgs);
		}

		public void TapElement(bool isTapped)
		{
			if (isTapped)
			{
				_background.color = _tapedColor;
			}
			else
			{
				_background.color = _normalColor;
			}
		}
	}

	public interface ITapElement
	{
		public void TapElement(bool isTapped);
	}
}