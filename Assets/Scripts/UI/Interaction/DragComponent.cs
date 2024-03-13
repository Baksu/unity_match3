using System;
using UI.Interaction.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Interaction
{
	public enum DragDirection
	{
		None,
		Left,
		Up,
		Right,
		Down
	}

	public class OnDragDirectionArgs : EventArgs
	{
		public DragDirection DragDirection { get; set; }
	}
	
	public class DragComponent : MonoBehaviour, IMoveComponent, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		[SerializeField] private int _dragOffset = 14; 
		[SerializeField] private float _distanceToDrag = 15;
		
		public event EventHandler OnDragDirection;
		private Vector2 _startDragPosition;
		private bool _canDrag;
		
		public event EventHandler OnMoveEventHandler;
		
		public void OnBeginDrag(PointerEventData eventData)
		{
			_canDrag = true;
			_startDragPosition = eventData.position;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!_canDrag)
			{
				return;
			}
			
			var endDragPosition = _startDragPosition - eventData.position;
			
			if (endDragPosition.magnitude >= _distanceToDrag)
			{
				_canDrag = false;
				CheckDirection(endDragPosition);
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_canDrag = false;
		}
		
		private void CheckDirection(Vector2 endDragPosition)
		{
			DragDirection dragDirection = DragDirection.None;
			
			if (Mathf.Abs(endDragPosition.x) > Mathf.Abs(endDragPosition.y))
			{
				//left or right
				if (endDragPosition.x < _dragOffset * -1)
				{
					Debug.Log("DragRight");
					dragDirection = DragDirection.Right;
				}
				else if(endDragPosition.x > _dragOffset)
				{
					Debug.Log("DragLeft");
					dragDirection = DragDirection.Left;
				}
			}
			else
			{
				//up or down
				if (endDragPosition.y < _dragOffset * -1)
				{
					Debug.Log("Drag up");
					dragDirection = DragDirection.Up;
				}
				else if(endDragPosition.y > _dragOffset)
				{
					Debug.Log("Drag down");
					dragDirection = DragDirection.Down;
				}
			}
			
			Debug.Log("Drag x: " + endDragPosition.x + " y: " + endDragPosition.y);
			OnMoveEventHandler?.Invoke(this, new OnDragDirectionArgs
			{
				DragDirection = dragDirection
			});
		}
	}
}