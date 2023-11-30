using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIDemo.UI
{
	public class NavSelectPass : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
		public UnityEvent selectEvent;
		public UnityEvent deselectEvent;

		void ISelectHandler.OnSelect(BaseEventData eventData)
		{
			selectEvent.Invoke();
		}

		void IDeselectHandler.OnDeselect(BaseEventData eventData)
		{
			deselectEvent.Invoke();
		}
	}
}