using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIDemo.UI
{
	public class TabItemModel : MonoBehaviour
	{
		public delegate void SelectedChanged();
		public event SelectedChanged onSelectedChanged;

		private bool isSelected { get; set; }

		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
			set
			{
				isSelected = value;
				onSelectedChanged?.Invoke();
			}
		}

	}

}
