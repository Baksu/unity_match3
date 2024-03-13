using System;
using UI.Interaction.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Interaction
{
	public class OnTapEventsArgs : EventArgs
	{
	}
	
	public class TapComponent : MonoBehaviour, IMoveComponent, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
	{
		public event EventHandler OnMoveEventHandler;

		private bool _pointerAboveElement;
		
		public void OnPointerDown(PointerEventData eventData)
		{
			_pointerAboveElement = true;
		}
		
		public void OnPointerUp(PointerEventData eventData)
		{
			if (_pointerAboveElement)
			{
				OnMoveEventHandler?.Invoke(this, new OnTapEventsArgs());
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_pointerAboveElement = false;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_pointerAboveElement = true;
		}
	}
}