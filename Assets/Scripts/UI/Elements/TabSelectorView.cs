using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UIDemo.UI
{
	public class TabSelectorView : MonoBehaviour
	{
		public TabSelectorModel model;
		public int defaultSelected = 0;

		public void Awake()
		{
			SelectTabItem(defaultSelected);
		}

		public void SelectTabItem(int index)
		{
			if (model != null)
			{
				for (int i = 0; i < model.TabItems.Count; i++)
				{
					model.TabItems[i].IsSelected = (index == i);
				}
			}
		}
	}
}
