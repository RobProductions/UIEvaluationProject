using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIDemo.UI
{
	public class TintOnSelected : MonoBehaviour
	{
		public Image targetImage;
		public Color tintColor = Color.white;
		public bool defaultIsTransparent = false;
		public float tintSpeed = 5f;

		private bool isSelected = false;
		private Color defaultColor = Color.white;

		void Awake()
		{
			defaultColor = GetImageColor();
			if(defaultIsTransparent)
			{
				defaultColor = Color.white;
				defaultColor.a = 0;
			}
			SetImageColor(defaultColor);
		}

		void Update()
		{
			var changeAmt = Time.deltaTime * tintSpeed;
			var targetColor = (isSelected) ? tintColor : defaultColor;
			var newColor = GetImageColor();
			newColor.r = Mathf.MoveTowards(newColor.r, targetColor.r, changeAmt);
			newColor.g = Mathf.MoveTowards(newColor.g, targetColor.g, changeAmt);
			newColor.b = Mathf.MoveTowards(newColor.b, targetColor.b, changeAmt);
			newColor.a = Mathf.MoveTowards(newColor.a, targetColor.a, changeAmt);
			SetImageColor(newColor);
		}

		public void SetSelected(bool v)
		{
			isSelected = v;
		}

		void SetImageColor(Color color)
		{
			if(targetImage != null)
			{
				targetImage.color = color;
			}
		}

		Color GetImageColor()
		{
			if(targetImage != null)
			{
				return targetImage.color;
			}
			return Color.black;
		}
	}
}

