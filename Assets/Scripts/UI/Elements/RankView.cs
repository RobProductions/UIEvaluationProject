using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIDemo.UI
{
	public class RankView : MonoBehaviour
	{
		public RankModel rankModel;
		public TextMeshProUGUI rankText;
		public GameObject rankUpLabel;
		public GameObject rankDownLabel;
		public Image rankFillImage;


		private void Awake()
		{
			if(rankModel != null)
			{
				//Mock set rank
				rankModel.UserRank = 242;
			}
		}

		private void OnEnable()
		{
			if(rankModel != null)
			{
				rankModel.onRankChanged += UserRankUpdated;
			}
		}

		private void OnDisable()
		{
			if(rankModel != null)
			{
				rankModel.onRankChanged -= UserRankUpdated;
			}
		}

		void UserRankUpdated()
		{
			if(rankModel == null)
			{
				return;
			}

			if(rankText)
			{
				rankText.text = rankModel.UserRank.ToString() + " SR";
			}
			if(rankUpLabel != null && rankDownLabel != null)
			{
				//Mock values used
				bool rankUp = (rankModel.UserRank > 230);
				rankUpLabel.SetActive(rankUp);
				rankDownLabel.SetActive(!rankUp);
			}
			if(rankFillImage)
			{
				rankFillImage.fillAmount = (float)rankModel.UserRank / (float)rankModel.MaxUserRank;
			}
		}
	}
}

