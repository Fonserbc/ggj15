using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace UI
{
	public class TabbedOverlay : MonoBehaviour
	{
		public interface Tab
		{
			string TabName();
			GameObject CreateContent();

			bool ShowVerticalScroll();
			bool ShowHoritzonalScroll();

			void OnGainFocus();
			void OnLostFocus();
		}

		//----------------------------//

		static private Font _fontTitle = null;
		static private Font _fontTabButton = null;
		static private Font _fontContent = null;
		static private Font _fontContentMonospace = null;

		private static Sprite _spriteBackground = null;
		private static Sprite _spriteTabButton = null;
		private static Sprite _spriteButton = null;
		private static Sprite _spriteCross = null;
		private static Sprite _spriteCheckmark = null;
		private static Sprite _spriteField = null;

		static private T GetStaticResource<T>(ref T content, string filename) where T : UnityEngine.Object
		{
			if (content == null)
				content = Resources.Load<T>(filename);

			return content;
		}

		static public Font GetStaticFontTilte() { return GetStaticResource<Font>(ref _fontTitle, "Ubuntu-B"); }
		static public Font GetStaticFontTabs() { return GetStaticResource<Font>(ref _fontTabButton, "Ubuntu-R"); }
		static public Font GetStaticFontContent() { return GetStaticResource<Font>(ref _fontContent, "Ubuntu-L"); }
		static public Font GetStaticFontContentMonospace() { return GetStaticResource<Font>(ref _fontContentMonospace, "UbuntuMono-R"); }

		static public Sprite GetStaticSpriteBackground() { return GetStaticResource<Sprite>(ref _spriteBackground, "Background"); }
		static public Sprite GetStaticSpriteTabButton() { return GetStaticResource<Sprite>(ref _spriteTabButton, "TabButton"); }
		static public Sprite GetStaticSpriteButton() { return GetStaticResource<Sprite>(ref _spriteButton, "Button"); }
		static public Sprite GetStaticSpriteCross() { return GetStaticResource<Sprite>(ref _spriteCross, "Cross"); }
		static public Sprite GetStaticSpriteCheckmark() { return GetStaticResource<Sprite>(ref _spriteCheckmark, "Checkmark"); }
		static public Sprite GetStaticSpriteField() { return GetStaticResource<Sprite>(ref _spriteField, "Field"); }

		//----------------------------//

		public Font FontTitle = null;
		public Font FontTabButton = null;
		public Font FontContent = null;
		public Font FontContentMonospace = null;

		public Sprite SpriteBackground = null;
		public Sprite SpriteTabButton = null;
		public Sprite SpriteButton = null;
		public Sprite SpriteCheckmark = null;
		public Sprite SpriteField = null;

		private T GetResource<T>(ref T content, ref T buildIn, string filename) where T : UnityEngine.Object
		{
			if (content == null)
				content = GetStaticResource<T>(ref buildIn, filename);

			return content;
		}

		public Font GetFontTilte() { return GetResource<Font>(ref FontTitle, ref _fontTitle, "Ubuntu-B"); }
		public Font GetFontTabs() { return GetResource<Font>(ref FontTabButton, ref _fontTabButton, "Ubuntu-R"); }
		public Font GetFontContent() { return GetResource<Font>(ref FontContent, ref _fontContent, "Ubuntu-L"); }
		public Font GetFontContentMonospace() { return GetResource<Font>(ref FontContentMonospace, ref _fontContentMonospace, "UbuntuMono-R"); }

		public Sprite GetSpriteBackground() { return GetResource<Sprite>(ref SpriteBackground, ref _spriteBackground, "Background"); }
		public Sprite GetSpriteTabButton() { return GetResource<Sprite>(ref SpriteTabButton, ref _spriteTabButton, "TabButton"); }
		public Sprite GetSpriteButton() { return GetResource<Sprite>(ref SpriteButton, ref _spriteButton, "Button"); }
		public Sprite GetSpriteCheckmark() { return GetResource<Sprite>(ref SpriteCheckmark, ref _spriteCheckmark, "Checkmark"); }
		public Sprite GetSpriteField() { return GetResource<Sprite>(ref SpriteField, ref _spriteField, "Field"); }

		public int FontSize(int fontSize = 16) { return fontSize; }
		public float FontSizeWithMargins(int fontSize = 16) { return FontSize(fontSize) * (7f / 4f); }

		public int FontSizeDPI(int fontSize = 16) { return Helper.DPI(fontSize); }
		public float FontSizeDPIWithMargins(int fontSize = 16) { return FontSizeDPI(fontSize) * (7f / 4f); }

		//----------------------------//

		[SerializeField]
		private bool show = false;
		public bool Show
		{
			get
			{
				return show;
			}
			set
			{
				show = value;
				if (_vrcanvas != null)
					_vrcanvas.show = value;
			}
		}

		private bool _lastIsVisible = false;
		public bool IsVisible
		{
			get
			{
				if (_vrcanvas != null)
					return _vrcanvas.IsVisible;

				return true;
			}
		}

		public GameObject UIParent
		{
			set
			{
				if (_vrcanvas != null)
					_vrcanvas.SetParent(value);
			}
			get
			{
				if (_vrcanvas != null)
					return _vrcanvas.EventCamera.gameObject;

				return null;
			}
		}

		public Camera UIEventCamera
		{
			set
			{
				if (_vrcanvas != null)
					_vrcanvas.EventCamera = value;
			}
			get
			{
				if (_vrcanvas != null)
					return _vrcanvas.EventCamera;

				return null;
			}
		}

		//----------------------------//

		private bool swipeProgress = false;

		private Vector2 touch0Begin = Vector2.zero;
		private Vector2 touch1Begin = Vector2.zero;

		private Vector2 touch0End = Vector2.zero;
		private Vector2 touch1End = Vector2.zero;

		//----------------------------//

		private class TabHandler
		{
			public Tab tab = null;
			public GameObject button = null;
			public GameObject root = null;
		}

		private List<TabHandler> _tabs = new List<TabHandler>();
		private Tab _selectedTab = null;
		private bool _dirtyTabs = true;

		//----------------------------//

		private GameObject _root = null;
		private ScreenSpaceCanvasVR _vrcanvas = null;

		private GameObject _objTitle = null;
		private GameObject _objTabView = null;
		private GameObject _objTabContent = null;
		private GameObject _objInnerView = null;
		private GameObject _objInnerScroll = null;
		private GameObject _objInnerDummyContent = null;
		private GameObject _objVertScrollbar = null;
		private GameObject _objHoriScrollbar = null;

		private Scrollbar AddScrollbar(GameObject goScrollbar)
		{
			Scrollbar scrollbar = goScrollbar.AddOrGetComponent<Scrollbar>();
			{
				Image image = goScrollbar.AddOrGetComponent<Image>();
				image.sprite = GetSpriteField();
				image.type = Image.Type.Sliced;
			}

			GameObject sliding = new GameObject("Sliding Area");
			sliding.layer = LayerMask.NameToLayer("UI");
			sliding.transform.SetParent(goScrollbar.transform, false);
			{
				RectTransform rect = sliding.AddOrGetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0, 0);
				rect.anchorMax = new Vector2(1, 1);
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.sizeDelta = new Vector2(0, 0);
				rect.anchoredPosition = new Vector2(0, 0);
			}

			GameObject handle = new GameObject("Handle");
			handle.layer = LayerMask.NameToLayer("UI");
			handle.transform.SetParent(sliding.transform, false);
			{
				RectTransform rect = handle.AddOrGetComponent<RectTransform>();
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.sizeDelta = new Vector2(0, 0);
				rect.anchoredPosition = new Vector2(0, 0);
				scrollbar.handleRect = rect;

				Image image = handle.AddOrGetComponent<Image>();
				image.sprite = GetSpriteButton();
				image.type = Image.Type.Sliced;
			}

			return scrollbar;
		}

		private static GameObject CreateCanvas(GameObject parent)
		{
			GameObject root = Helper.CreateGUIGameObject("TabbedOverlayCanvas", parent);
			root.AddOrGetComponent<Canvas>();
			root.AddOrGetComponent<CanvasScaler>();
			root.AddOrGetComponent<GraphicRaycaster>();
			root.AddOrGetComponent<CanvasRenderer>();

			return root;
		}

		private void CreateWindow(GameObject parent)
		{
			GameObject window = Helper.CreateGUIGameObject("Window", parent);
			GameObject bar = Helper.CreateGUIGameObject("WindowBar", window);
			GameObject color = Helper.CreateGUIGameObject("WindowBarColor", bar);
			GameObject title = _objTitle = Helper.CreateGUIGameObject("WindowTitle", bar);
			GameObject close = Helper.CreateGUIGameObject("CloseButton", bar);
			GameObject cimg = Helper.CreateGUIGameObject("CloseButton", close);

			GameObject tabview = _objTabView = Helper.CreateGUIGameObject("TabScrollView", bar);
			GameObject tabscroll = Helper.CreateGUIGameObject("TabScrollRect", tabview);
			GameObject tabcontent = _objTabContent = Helper.CreateGUIGameObject("TabContent", tabscroll);

			GameObject innerview = _objInnerView = Helper.CreateGUIGameObject("InnerScrollView", window);
			GameObject innerscroll = _objInnerScroll = Helper.CreateGUIGameObject("InnerScrollRect", innerview);
			GameObject innercontent = _objInnerDummyContent = Helper.CreateGUIGameObject("InnerScrollRect", innerscroll);
			GameObject vertscrollbar = _objVertScrollbar = Helper.CreateGUIGameObject("InnerVerticalScrollbar", innerview);
			GameObject horiscrollbar = _objHoriScrollbar = Helper.CreateGUIGameObject("InnerHorizontalScrollbar", innerview);

			// Window
			{
				Helper.SetRectTransformDPI(window, 0, 0, 1, 1, 0.5f, 0.5f, 0, 0, 0, 0);
				Image img = window.AddOrGetComponent<Image>();
				img.type = Image.Type.Sliced;
				img.sprite = GetSpriteBackground();

				window.AddOrGetComponent<Mask>();
			}

			// WindowBar
			{
				Helper.SetRectTransformDPI(bar, 0, 1, 1, 1, 0.5f, 1, -4.0f, 32.0f, 0.0f, -2.0f);
				bar.AddOrGetComponent<CanvasRenderer>();
			}

			// WindowTitle
			{
				Helper.SetRectTransformDPI(title, 0, 0, 1, 1, 0.5f, 0.5f, 0, 0, 0, 0);
				Text text = title.AddOrGetComponent<Text>();
				text.text = "";
				text.alignment = TextAnchor.MiddleCenter;
				text.color = Color.white;
				text.font = GetFontTilte();
				text.fontSize = FontSizeDPI();
			}

			// WindowBarColor
			{
				Helper.SetRectTransformDPI(color, 0, 0, 1, 1, 0.5f, 0.5f, 0, 0, 0, 0);
				Image img = color.AddOrGetComponent<Image>();
				img.color = new Color(0.25f, 0.5f, 1.0f);
			}

			// CloseButton
			{
				Helper.SetRectTransformDPI(close, 1, 0, 1, 1, 1, 0.5f, 32.0f - 4.0f, -4.0f, -2.0f, 0);
				Image img = close.AddOrGetComponent<Image>();
				img.sprite = GetSpriteButton();
				img.type = Image.Type.Sliced;

				Button but = close.AddOrGetComponent<Button>();
				but.onClick.AddListener(() =>
				{
					Show = false;
				});
			}

			// CloseCross
			{
				Helper.SetRectTransformDPI(cimg, 0.25f, 0.25f, 0.75f, 0.75f, 0.5f, 0.5f, 0, 0, 0, 0);
				Image img = cimg.AddOrGetComponent<Image>();
				img.sprite = GetStaticSpriteCross();
				img.type = Image.Type.Simple;
			}

			// TabScrollView
			{
				Helper.SetRectTransformDPI(tabview, 0, 0, 1, 0, 0.5f, 1, 0, 0, 0, -2.0f);
				tabview.AddOrGetComponent<Image>();
				Mask mask = tabview.AddOrGetComponent<Mask>();
				mask.showMaskGraphic = false;
			}

			// TabScrollRect
			{
				Helper.SetRectTransformDPI(tabscroll, 0, 0, 1, 1, 0.5f, 1, 0, 0, 0, -2.0f);
				ScrollRect scroll = tabscroll.AddOrGetComponent<ScrollRect>();
				scroll.content = tabcontent.AddOrGetComponent<RectTransform>();
				scroll.vertical = false;
			}

			// TabContent
			{
				Helper.SetRectTransformDPI(tabcontent, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
				tabcontent.AddOrGetComponent<CanvasRenderer>();
			}

			// InnerScrollView
			{
				Helper.SetRectTransformDPI(innerview, 0, 0, 1, 1, 0.5f, 0, -4.0f, 0.0f, 0, 2.0f);
				innerview.AddOrGetComponent<Image>();
				Mask mask = innerview.AddOrGetComponent<Mask>();
				mask.showMaskGraphic = false;
			}

			// InnerVerticalScrollbarRect
			{
				Helper.SetRectTransformDPI(vertscrollbar, 1, 0, 1, 1, 1, 1, 20.0f, -24.0f, -2.0f, -2.0f);
				Scrollbar scrollbar = AddScrollbar(vertscrollbar);
				scrollbar.direction = Scrollbar.Direction.BottomToTop;
			}

			// InnerHorizontalScrollbarRect
			{
				Helper.SetRectTransformDPI(horiscrollbar, 0, 0, 1, 0, 0, 0, -24.0f, 20.0f, 2.0f, 2.0f);
				AddScrollbar(horiscrollbar);
			}

			// InnerScrollRect
			{
				Helper.SetRectTransformDPI(innerscroll, 0, 0, 1, 1, 0, 1, -24.0f, -24.0f, 0, 0);
				ScrollRect scroll = innerscroll.AddOrGetComponent<ScrollRect>();
				scroll.verticalScrollbar = vertscrollbar.GetComponent<Scrollbar>();
				scroll.horizontalScrollbar = horiscrollbar.GetComponent<Scrollbar>();
				scroll.content = Helper.SetRectTransform(innercontent, 0, 0, 1, 1, 0, 1, 0, 0, 0, 0);
			}
		}

		private GameObject CreateTabButton(TabHandler handler)
		{
			string name = handler.tab.TabName();
			GameObject button = new GameObject("TabButton_" + name);
			GameObject text = new GameObject("Text");

			button.layer =
				text.layer = LayerMask.NameToLayer("UI");

			text.transform.SetParent(button.transform, false);
			Text txt = text.AddOrGetComponent<Text>();
			txt.text = name;
			txt.fontSize = FontSizeDPI();
			txt.font = GetFontTabs();
			txt.alignment = TextAnchor.MiddleCenter;
			txt.color = Color.black;

			{ //Text
				RectTransform rect = text.AddOrGetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0, 0);
				rect.anchorMax = new Vector2(1, 1);
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.anchoredPosition = new Vector2(0, 0);
				rect.sizeDelta = new Vector2(0, 0);
			}

			{ // Tab
				Image img = button.AddOrGetComponent<Image>();
				img.sprite = GetSpriteTabButton();
				img.type = Image.Type.Sliced;

				RectTransform rect = button.AddOrGetComponent<RectTransform>();
				try
				{
					rect.sizeDelta = new Vector2(txt.preferredWidth + Helper.DPI(16.0f), Helper.DPI(32.0f));
				}
				catch
				{

				}

				Tab i = handler.tab;
				Button but = button.AddOrGetComponent<Button>();
				but.onClick.AddListener(() =>
				{
					SetTabActive(i);
				});

				ColorBlock block = but.colors;
				Color normal = block.normalColor;
				block.normalColor = block.disabledColor;
				block.disabledColor = normal;
				but.colors = block;
			}

			return button;
		}

		private void InitTab(TabHandler handler)
		{
			handler.root = handler.tab.CreateContent();
			if (handler.root == null)
			{
				handler.root = new GameObject("TabNull" + handler.tab.TabName());
				handler.root.AddOrGetComponent<CanvasRenderer>();
			}

			handler.root.layer = LayerMask.NameToLayer("UI");
			handler.root.transform.SetParent(_objInnerScroll.transform, false);
			handler.root.SetActive(false);
			handler.button = CreateTabButton(handler);
		}

		public void AddTab(Tab tab)
		{
			TabHandler handler = new TabHandler();
			handler.tab = tab;
			_tabs.Add(handler);
			_dirtyTabs = true;
		}

		public void SetTabActive(Tab tab)
		{
			_selectedTab = tab;
			_dirtyTabs = true;
		}

		private void SetTilte(string title)
		{
			if (_objTitle == null)
				return;

			Text text = _objTitle.AddOrGetComponent<Text>();
			text.text = title;
		}

		void OnEnable()
		{
			if (_root != null)
			{
				_root.SetActive(true);
				return;
			}

			EventSystem es = gameObject.FindObjectOrCreateIt<EventSystem>("EventSystem");
			es.gameObject.AddOrGetComponent<StandaloneInputModule>();
			es.gameObject.AddOrGetComponent<TouchInputModule>();

			_root = CreateCanvas(gameObject);
			CreateWindow(_root);

			_vrcanvas = _root.AddOrGetComponent<ScreenSpaceCanvasVR>();
			_vrcanvas.show = show;
			_lastIsVisible = _vrcanvas.IsVisible;
		}

		void OnDisable()
		{
			if (_root != null)
			{
				_root.SetActive(false);
				return;
			}
		}

		void CheckTabsChanges()
		{
			if (!_dirtyTabs)
				return;

			{
				RectTransform recttab = _objTabView.AddOrGetComponent<RectTransform>();
				RectTransform rectinner = _objInnerView.AddOrGetComponent<RectTransform>();
				if (_tabs.Count > 1)
				{
					recttab.sizeDelta = Helper.DPI(0, 32.0f);
					Vector2 innerdelta = rectinner.sizeDelta;
					innerdelta.y = Helper.DPI(-64.0f - 8.0f) + innerdelta.x;
					rectinner.sizeDelta = innerdelta;
				}
				else
				{
					recttab.sizeDelta = new Vector2(0, 0);
					Vector2 innerdelta = rectinner.sizeDelta;
					innerdelta.y = Helper.DPI(-32.0f - 8.0f) + innerdelta.x;
					rectinner.sizeDelta = innerdelta;
				}

				if (_tabs.Count > 0)
				{
					if (_selectedTab == null)
						_selectedTab = _tabs[0].tab;
				}
				else
				{
					if (_selectedTab != null)
					{
						_selectedTab = null;
						_objInnerScroll.AddOrGetComponent<ScrollRect>().content = 
							_objInnerDummyContent.AddOrGetComponent<RectTransform>();
					}
				}
			}

			float accumX = 0;
			for (int i = 0; i < _tabs.Count; i++)
			{
				TabHandler handler = _tabs[i];
				if (handler.root == null)
					InitTab(handler);

				if (handler.tab == _selectedTab)
				{
					if (!handler.root.activeSelf)
					{
						handler.button.AddOrGetComponent<Button>().interactable = false;
						handler.root.SetActive(true);
						handler.tab.OnGainFocus();
						_objInnerScroll.AddOrGetComponent<ScrollRect>().content = handler.root.AddOrGetComponent<RectTransform>();
					}
				}
				else
				{
					if (handler.root.activeSelf)
					{
						handler.button.AddOrGetComponent<Button>().interactable = true;
						handler.root.SetActive(false);
						handler.tab.OnLostFocus();
					}
				}

				handler.button.transform.SetParent(_objTabContent.transform, false);
				RectTransform rect = handler.button.AddOrGetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0, 0);
				rect.anchorMax = new Vector2(0, 0);
				rect.pivot = new Vector2(0, 0);
				rect.anchoredPosition = new Vector2(accumX, 0);
				accumX += rect.sizeDelta.x;
			}

			if (_selectedTab == null)
			{
				SetTilte("Tabbed Overlay");
			}
			else
			{
				if (_tabs.Count == 1)
					SetTilte(_selectedTab.TabName());
				else
					SetTilte("Tab: " + _selectedTab.TabName());
			}

			{
				RectTransform rect = _objTabContent.AddOrGetComponent<RectTransform>();
				rect.sizeDelta = new Vector2(accumX, 0);
			}

			_dirtyTabs = false;
		}

		private bool KeyDown(string key)
		{
			return Event.current.Equals(Event.KeyboardEvent(key));
		}

		void OnSwipe(Vector2 swipe)
		{
			float sw = Mathf.Abs(swipe.x) / swipe.magnitude;
			if (sw > 0.5f)
				return;

			if (Show && swipe.y > 0.4f)
				Show = false;

			if (!Show && swipe.y < -0.4f)
				Show = true;
		}

		void OnGUI()
		{
			// For phone devices
			if (Input.touchCount == 2)
			{
				if (!swipeProgress)
				{
					touch0Begin = Input.GetTouch(0).position;
					touch1Begin = Input.GetTouch(1).position;
					swipeProgress = true;
				}

				touch0End = Input.GetTouch(0).position;
				touch1End = Input.GetTouch(1).position;
			}
			else if (swipeProgress)
			{
				swipeProgress = false;

				float ipd = 0;
				if (Screen.dpi != 0)
					ipd = 1.0f / Screen.dpi;

				Vector2 touchBegin = (touch0Begin + touch1Begin) * 0.5f * ipd;
				Vector2 touchEnd = (touch0End + touch1End) * 0.5f * ipd;
				Vector2 swipe = touchEnd - touchBegin;
				OnSwipe(swipe);
			}

			if (KeyDown("F9"))
				Show = !Show;
		}

		void Update()
		{
			if (!_lastIsVisible && IsVisible)
			{
				if (_selectedTab != null)
					_selectedTab.OnGainFocus();
			}
			else if (_lastIsVisible && !IsVisible)
			{
				if (_selectedTab != null)
					_selectedTab.OnLostFocus();
			}

			if (IsVisible)
			{
				CheckTabsChanges();

				if (_selectedTab != null)
				{
					RectTransform rect = _objInnerScroll.AddOrGetComponent<RectTransform>();
					Vector2 sizeDelta = rect.sizeDelta;
					sizeDelta.x = _selectedTab.ShowVerticalScroll() ? Helper.DPI(-24.0f) : Helper.DPI(-4.0f);
					sizeDelta.y = _selectedTab.ShowHoritzonalScroll() ? Helper.DPI(-24.0f) : Helper.DPI(-4.0f);
					rect.sizeDelta = sizeDelta;

					if (_objVertScrollbar.activeSelf != _selectedTab.ShowVerticalScroll())
						_objVertScrollbar.SetActive(_selectedTab.ShowVerticalScroll());

					if (_objHoriScrollbar.activeSelf != _selectedTab.ShowHoritzonalScroll())
						_objHoriScrollbar.SetActive(_selectedTab.ShowHoritzonalScroll());
				}
			}

			_lastIsVisible = IsVisible;
		}
	}
}
