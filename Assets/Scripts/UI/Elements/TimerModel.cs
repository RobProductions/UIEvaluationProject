using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIDemo.UI
{
	public class TimerModel : MonoBehaviour
	{
		public delegate void ExpirationChanged();
		public event ExpirationChanged onExpirationChanged;

		private DateTime expirationTime { get; set; }

		public DateTime ExpirationTime
		{
			get
			{
				return expirationTime;
			}
			set
			{
				expirationTime = value;
				onExpirationChanged?.Invoke();
			}
		}

		/// <summary>
		/// Add this amount of hours to the expiration
		/// </summary>
		/// <param name="hourValue"></param>
		public void AddTimeHour(int hourValue)
		{
			ExpirationTime = expirationTime.AddHours((float)hourValue);
		}
	}
}

