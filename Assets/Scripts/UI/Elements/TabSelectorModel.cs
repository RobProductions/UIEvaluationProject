using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIDemo.UI
{
	public class TabSelectorModel : MonoBehaviour
	{
		public List<TabItemModel> TabItems = new List<TabItemModel>();

		public void AddTabItem(TabItemModel v)
		{
			TabItems.Add(v);
		}
	}
}

