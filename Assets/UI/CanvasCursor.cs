using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UI
{
	[AddComponentMenu("Event/Canvas Cursor")]
	public class CanvasCursor : BaseRaycaster
	{
		/// <summary>
		/// Const to use for clarity when no event mask is set
		/// </summary>
		protected const int kNoEventMaskSet = -1;

		/// <summary>
		/// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
		/// </summary>
		[SerializeField]
		protected LayerMask m_EventMask = kNoEventMaskSet;

		/// <summary>
		/// Event mask used to determine which objects will receive events.
		/// </summary>
		public int finalEventMask
		{
			get { return (eventCamera != null) ? eventCamera.cullingMask & m_EventMask : kNoEventMaskSet; }
		}

		/// <summary>
		/// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
		/// </summary>
		public LayerMask eventMask
		{
			get { return m_EventMask; }
			set { m_EventMask = value; }
		}

		private Canvas m_Canvas = null;
		private Canvas canvas
		{
			get
			{
				if (m_Canvas != null)
					return m_Canvas;

				m_Canvas = GetComponent<Canvas>();
				return m_Canvas;
			}
		}

		private GameObject cursor = null;

		private static Sprite _spriteCursor = null;
		static public Sprite GetCursorSprite()
		{
			if (_spriteCursor == null)
				_spriteCursor = Resources.Load<Sprite>("Cursor");

			return _spriteCursor;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (canvas == null)
				return;

			if (cursor == null)
			{
				cursor = new GameObject("Cursor");
				cursor.layer = LayerMask.NameToLayer("UI");

				RectTransform rect = cursor.AddOrGetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0.0f, 1.0f);
				rect.anchorMax = new Vector2(0.0f, 1.0f);
				rect.pivot = new Vector2(0.3f, 0.9f);
				rect.anchoredPosition = Helper.DPI(20.0f, 20.0f);
				rect.sizeDelta = Helper.DPI(20.0f, 20.0f);

				Image image = cursor.AddOrGetComponent<Image>();
				image.sprite = GetCursorSprite();

				CanvasGroup group = cursor.AddOrGetComponent<CanvasGroup>();
				group.blocksRaycasts = false;
				group.interactable = false;

				cursor.transform.SetParent(transform, false);
				cursor.SetActive(false);
			}

			if (Input.mousePresent)
			{
				cursor.SetActive(true);
				Screen.showCursor = false;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			cursor.SetActive(false);
			Screen.showCursor = true;
		}

		void OnApplicationFocus(bool focus)
		{
			if (cursor == null)
				return;

			if (enabled && focus && Input.mousePresent)
			{
				cursor.SetActive(true);
				Screen.showCursor = false;
			}
			else
			{
				cursor.SetActive(false);
				Screen.showCursor = true;
			}
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			RectTransform canvasRect = GetComponent<RectTransform>();
			RectTransform cursorRect = cursor.AddOrGetComponent<RectTransform>();
			Vector2 position = eventData.position;
			position.x = position.x - 0.5f * (Screen.width - canvasRect.sizeDelta.x);
			position.y = position.y - Screen.height;
			cursorRect.anchoredPosition = position;

			return;
		}

		public override Camera eventCamera
		{
			get
			{
				if (canvas.renderMode == RenderMode.ScreenSpaceOverlay
					|| (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
					return null;

				return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
			}
		}
	}
}