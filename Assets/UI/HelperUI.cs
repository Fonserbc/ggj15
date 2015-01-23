using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
	public static class Helper
	{
		static private float _dpiFactor = -1.0f;
		static private void InitDpiFactor()
		{
			int dpi = (int)Screen.dpi;
			if (dpi == 0)
				dpi = 96;

			_dpiFactor = dpi / 96.0f;
		}

		static public float DPI(float value)
		{
			if (_dpiFactor < 0)
				InitDpiFactor();

			return value * _dpiFactor;
		}

		static public int DPI(int value)
		{
			if (_dpiFactor < 0)
				InitDpiFactor();

			return (int)(value * _dpiFactor);
		}

		static public Vector2 DPI(float x, float y)
		{
			if (_dpiFactor < 0)
				InitDpiFactor();

			return new Vector2(DPI(x), DPI(y));
		}

		static public Vector2 DPI(Vector2 value)
		{
			if (_dpiFactor < 0)
				InitDpiFactor();

			value.x = DPI(value.x);
			value.y = DPI(value.y);

			return value;
		}

		//-------------------------------------------------------------//

		public static GameObject CreateInputField(TabbedOverlay overlay, GameObject parent, string name, string value, string placeHolder = "")
		{
			GameObject goInputField = CreateGUIGameObject("InputField_" + name, parent);
			InputField inField = goInputField.AddComponent<InputField>();
			{
				Image textImage = goInputField.AddComponent<Image>();
				textImage.sprite = overlay.GetSpriteField();
				textImage.type = Image.Type.Sliced;
				inField.targetGraphic = textImage;

				RectTransform rect = goInputField.AddOrGetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0, 0);
				rect.anchorMax = new Vector2(1, 1);
				rect.pivot = new Vector2(0, 0.5f);
				rect.sizeDelta = Helper.DPI(0, 6);
			}

			GameObject goPlaceholder = CreateGUIGameObject("Placeholder", goInputField);
			Text textPlaceholder = goPlaceholder.AddOrGetComponent<Text>();
			{
				textPlaceholder.fontSize = overlay.FontSizeDPI();
				textPlaceholder.font = overlay.GetFontContent();
				textPlaceholder.alignment = TextAnchor.MiddleLeft;
				textPlaceholder.color = Color.gray;
				textPlaceholder.text = placeHolder;
				textPlaceholder.alignment = TextAnchor.UpperLeft;

				RectTransform rect = goPlaceholder.AddOrGetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0, 1);
				rect.anchorMax = new Vector2(1, 1);
				rect.pivot = new Vector2(0.5f, 1);
				rect.anchoredPosition = Helper.DPI(0, -4.0f);
				rect.sizeDelta = Helper.DPI(-16, overlay.FontSize() * 1.25f);
			}

			GameObject goText = CreateGUIGameObject("Text", goInputField);
			Text textText = goText.AddOrGetComponent<Text>();
			{
				textText.fontSize = overlay.FontSizeDPI();
				textText.font = overlay.GetFontContent();
				textText.alignment = TextAnchor.MiddleLeft;
				textText.color = Color.black;
				textText.supportRichText = false;
				textText.alignment = TextAnchor.UpperLeft;

				RectTransform rect = goText.AddOrGetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0, 1);
				rect.anchorMax = new Vector2(1, 1);
				rect.pivot = new Vector2(0.5f, 1);
				rect.anchoredPosition = Helper.DPI(0, -4.0f);
				rect.sizeDelta = Helper.DPI(-16, overlay.FontSize() * 1.25f);
			}

			inField.placeholder = textPlaceholder;
			inField.textComponent = textText;
			inField.text = value;

			return goInputField;
		}

		static public GameObject CreateGUIGameObject(string name, GameObject parent = null)
		{
			GameObject gameobject = new GameObject(name);
			gameobject.layer = LayerMask.NameToLayer("UI");

			if (parent != null)
				gameobject.transform.SetParent(parent.transform, false);

			return gameobject;
		}

		public static RectTransform SetRectTransformDPI(
			GameObject element,
			float anchorMinX,
			float anchorMinY,
			float anchorMaxX,
			float anchorMaxY,
			float pivotX,
			float pivotY,
			float sizeDeltaX,
			float sizeDeltaY,
			float anchoredPositionX,
			float anchoredPositionY)
		{
			return SetRectTransform(
				element.AddOrGetComponent<RectTransform>(),
				anchorMinX, anchorMinY,
				anchorMaxX, anchorMaxY,
				pivotX, pivotY,
				DPI(sizeDeltaX), DPI(sizeDeltaY),
				DPI(anchoredPositionX), DPI(anchoredPositionY));
		}

		public static RectTransform SetRectTransformDPI(
			RectTransform rect,
			float anchorMinX,
			float anchorMinY,
			float anchorMaxX,
			float anchorMaxY,
			float pivotX,
			float pivotY,
			float sizeDeltaX,
			float sizeDeltaY,
			float anchoredPositionX,
			float anchoredPositionY)
		{
			return SetRectTransform(
				rect,
				anchorMinX, anchorMinY,
				anchorMaxX, anchorMaxY,
				pivotX, pivotY,
				DPI(sizeDeltaX), DPI(sizeDeltaY),
				DPI(anchoredPositionX), DPI(anchoredPositionY));
		}

		public static RectTransform SetRectTransform(
			GameObject element,
			float anchorMinX,
			float anchorMinY,
			float anchorMaxX,
			float anchorMaxY,
			float pivotX,
			float pivotY,
			float sizeDeltaX,
			float sizeDeltaY,
			float anchoredPositionX,
			float anchoredPositionY)
		{
			return SetRectTransform(
				element.AddOrGetComponent<RectTransform>(),
				anchorMinX, anchorMinY, 
				anchorMaxX, anchorMaxY, 
				pivotX, pivotY, 
				sizeDeltaX, sizeDeltaY, 
				anchoredPositionX, anchoredPositionY);
		}

		public static RectTransform SetRectTransform(
			RectTransform rect,
			float anchorMinX,
			float anchorMinY,
			float anchorMaxX,
			float anchorMaxY,
			float pivotX,
			float pivotY,
			float sizeDeltaX,
			float sizeDeltaY,
			float anchoredPositionX,
			float anchoredPositionY)
		{
				
			rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
			rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
			rect.pivot = new Vector2(pivotX, pivotY);
			rect.sizeDelta = new Vector2(sizeDeltaX, sizeDeltaY);
			rect.anchoredPosition = new Vector2(anchoredPositionX, anchoredPositionY);

			return rect;
		}
	}
}