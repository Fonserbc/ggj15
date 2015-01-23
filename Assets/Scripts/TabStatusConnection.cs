using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
	public class TabStatusConnection : MonoBehaviour, TabbedOverlay.Tab
	{
		private string TextToShow = "This is a text example";
		public float SizeMultiplier = 2;

		private TabbedOverlay parent = null;
		GameObject textObject = null;

		public int ComputeFontSize()
		{
			float multiplier = 1;
			if (SizeMultiplier > 0)
				multiplier = SizeMultiplier;

			return (int)(parent.FontSizeDPI() * multiplier);
		}

		public void Start()
		{
			parent = gameObject.FindManagerOrCreateIt<TabbedOverlay>();
			parent.AddTab(this);
		}

		public string TabName()
		{
			return "Connection";
		}

		public void OnGainFocus()
		{
		}

		public void OnLostFocus()
		{
		}

		public bool ShowVerticalScroll() { return false; }
		public bool ShowHoritzonalScroll() { return false; }

		public GameObject CreateContent()
		{
			textObject = new GameObject("TabText");

			Text text = textObject.AddOrGetComponent<Text>();
			text.font = parent.GetFontContent();
			text.text = TextToShow;
			text.fontSize = ComputeFontSize();
			text.color = Color.black;

			Helper.SetRectTransform(textObject, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0);
			ContentSizeFitter fitter = textObject.AddOrGetComponent<ContentSizeFitter>();
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			return textObject;
		}

		public void Update()
		{
			if (textObject != null)
			{
				TextToShow = "Connecting" + GetConnectingDots() + "\nStatus: " + PhotonNetwork.connectionStateDetailed;
				Text text = textObject.AddOrGetComponent<Text>();
				int computed = ComputeFontSize();
				if (text.text != TextToShow || text.fontSize != computed)
				{
					text.text = TextToShow;
					text.fontSize = computed;
					RectTransform rect = textObject.AddOrGetComponent<RectTransform>();
					rect.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
				}
			}
		}

		string GetConnectingDots()
		{
			string str = "";
			int numberOfDots = Mathf.FloorToInt(Time.timeSinceLevelLoad * 3f % 4);

			for (int i = 0; i < numberOfDots; ++i)
			{
				str += " .";
			}

			return str;
		}
	}
}

