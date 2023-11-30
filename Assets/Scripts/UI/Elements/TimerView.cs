using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace UIDemo.UI
{
	public class TimerView : MonoBehaviour
	{

		public TimerModel timerModel;
		public TextMeshProUGUI timerText;

		private void Awake()
		{
			if(timerModel)
			{
				//Test set initial time
				timerModel.ExpirationTime = DateTime.Now.AddHours(16).AddMinutes(42).AddSeconds(32);
			}
		}

		private void OnEnable()
		{
			if(timerModel)
			{
				timerModel.onExpirationChanged += UpdateTimeDisplay;
			}
		}

		private void OnDisable()
		{
			if(timerModel)
			{
				timerModel.onExpirationChanged -= UpdateTimeDisplay;
			}
		}

		// Update is called once per frame
		void Update()
		{
			UpdateTimeDisplay();
		}

		void UpdateTimeDisplay()
		{
			if(timerModel != null)
			{
				var currentTime = DateTime.Now;
				TimeSpan diff = timerModel.ExpirationTime - currentTime;
				int totalHours = Mathf.FloorToInt((float)diff.TotalHours);
				UpdateTimerText(totalHours + ":" + diff.Minutes + ":" + diff.Seconds);
			}
		}

		void UpdateTimerText(string text)
		{
			if(timerText)
			{
				timerText.text = text;
			}
		}
	}
}

