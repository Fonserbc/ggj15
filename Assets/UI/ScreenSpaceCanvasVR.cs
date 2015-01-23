using UnityEngine;
using System.Collections;

namespace UI
{
	public class ScreenSpaceCanvasVR : MonoBehaviour
	{
		private RectTransform rect = null;
		private Canvas canvas = null;
		private CanvasCursor cursor = null;
		private Camera lastWorldCamera = null;

		private float widthMult = 1.0f;
		public float WidthMultiplier
		{
			get { return widthMult; }
			set { widthMult = value; }
		}

		[SerializeField]
		private Camera eventCamera = null;
		public Camera EventCamera
		{
			get { return eventCamera; }
			set { SetEventCamera(value); }
		}

		public void SetParent(GameObject parent)
		{
			if (parent == null || parent == gameObject)
				return;

			Camera parentCamera = parent.GetComponent<Camera>();
			if (parentCamera == null)
			{
				parentCamera = parent.AddComponent<Camera>();
				parentCamera.enabled = false;
			}

			SetEventCamera(parentCamera);
		}

		public bool IsVisible
		{
			get 
			{
				return showTransition > 0.0f;
			}
		}

		public float distance = 0.5f;
		public bool show = false;
		private float showTransition = 0.0f;

		private void SetEventCamera(Camera cameraToAttach = null)
		{
			eventCamera = cameraToAttach;

			if (eventCamera == null && (transform.parent == null || (eventCamera = transform.parent.gameObject.GetComponent<Camera>()) == null))
			{
				if (Camera.main == null || Camera.main.gameObject == gameObject)
				{
					GameObject camera = null;
					if (transform.parent == null)
						camera = new GameObject("EventCamera");
					else
						camera = transform.parent.gameObject;

					eventCamera = camera.AddComponent<Camera>();
					eventCamera.enabled = false;
				}
				else
				{
					eventCamera = Camera.main;
				}
			}

			if (transform.parent != eventCamera.transform)
				transform.SetParent(eventCamera.transform, false);
		}

		void OnEnable()
		{
			rect = GetComponent<RectTransform>();
			canvas = GetComponent<Canvas>();
			cursor = gameObject.AddOrGetComponent<UI.CanvasCursor>();

			if (canvas != null)
			{

				cursor.enabled = true;
				SetEventCamera(null);

				lastWorldCamera = canvas.worldCamera;
				canvas.renderMode = RenderMode.WorldSpace;
				canvas.worldCamera = eventCamera;
			}
		}

		void OnDisable()
		{
			if (canvas != null)
			{
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				canvas.worldCamera = lastWorldCamera;
				cursor.enabled = false;
			}
		}

		private void UpdateTransition()
		{
			float timeToClose = 0.5f;
			if (show && showTransition < 1.0f)
			{
				showTransition += Time.deltaTime / timeToClose;
				if (showTransition > 1.0f)
					showTransition = 1.0f;
			}
			else if (!show && showTransition > 0.0f)
			{
				showTransition -= Time.deltaTime / timeToClose;
				if (showTransition < 0.0f)
					showTransition = 0.0f;
			}
		}

		void Update()
		{
			UpdateTransition();
		}

		void LateUpdate()
		{
			if (canvas != null)
			{
				if (IsVisible)
				{
					canvas.enabled = true;
					if (cursor != null)
						cursor.enabled = true;
				}
				else
				{
					canvas.enabled = false;
					if (cursor != null)
						cursor.enabled = false;
				}
			}

			if (IsVisible && rect != null && eventCamera != null)
			{
				rect.sizeDelta = new Vector2(Screen.width * widthMult, Screen.height * showTransition);
				double plane = distance;
				if (distance < eventCamera.nearClipPlane)
					distance = eventCamera.nearClipPlane;

				float fovRad = eventCamera.fieldOfView * Mathf.PI * (1.0f / 360f);
				float tan = Mathf.Tan(fovRad);
				double z = (0.5f * Screen.height) / tan;
				double scale = plane / z;

				Vector3 foo = rect.localPosition;
				foo.x = 0;
				foo.y = (float)(Screen.height * scale * 0.5);
				foo.z = (float)plane;
				rect.localPosition = foo;

				foo = rect.localScale;
				foo.x = (float)scale;
				foo.y = (float)scale;
				rect.localScale = foo;

				rect.pivot = new Vector2(0.5f, 1.0f);
			}
		}
	}
}

