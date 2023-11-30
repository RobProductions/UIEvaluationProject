using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIDemo.UI
{
	public class RankModel : MonoBehaviour
	{
		public delegate void RankChanged();
		public event RankChanged onRankChanged;

		private const int minUserRank = 0;
		private const int maxUserRank = 500;

		private int userRank { get; set; }

		public int MinUserRank => minUserRank;
		public int MaxUserRank => maxUserRank;

		public int UserRank
		{
			get
			{
				return userRank;
			}
			set
			{
				userRank = value;
				if(userRank < minUserRank)
				{
					userRank = minUserRank; 
				}
				else if(userRank > maxUserRank)
				{
					userRank = maxUserRank;
				}
				onRankChanged?.Invoke();
			}
		}

		public void DecreaseRank(int value)
		{
			UserRank -= value;
		}

		public void IncreaseRank(int value)
		{
			UserRank += value;
		}
	}
}

