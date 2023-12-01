using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIDemo.UI
{
	public class TabItemView : MonoBehaviour
	{
		public TabItemModel model;
		public TintOnSelected[] affectTints;
		public Animator[] affectAnims;

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
			foreach(Animator anim in affectAnims)
			{
				anim.SetInteger("state", (model.IsSelected) ? 1 : 0);
			}
		}
	}
}
