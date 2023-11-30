using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIDemo.UI
{
	public class TabItemView : MonoBehaviour
	{
		public TabItemModel model;
		public TintOnSelected[] affectTints;

		private void OnEnable()
		{
			if(model)
			{
				model.onSelectedChanged += OnSelectedChanged;
			}
		}

		private void OnDisable()
		{
			if(model)
			{
				model.onSelectedChanged -= OnSelectedChanged;
			}
		}

		void OnSelectedChanged()
		{
			foreach(TintOnSelected tint in affectTints)
			{
				tint.SetSelected(model.IsSelected);
			}
		}
	}
}
