using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections.Generic;
using System.Linq;


namespace UI
{
	public class TabConsole : MonoBehaviour, TabbedOverlay.Tab
	{
		[HideInInspector]
		private ConsoleLog consoleLog;
		private ConsoleCommandsRepository consoleCommandsRepository;
		private TabbedOverlay overlay = null;

		private GameObject root = null;
		private InputField inputField = null;
		private Text consoleText = null;
		private bool forceSubmit = false;

		public string TabName()
		{
			return "Console";
		}

		public void OnGainFocus()
		{
			enabled = true;
		}

		public void OnLostFocus()
		{
			enabled = false;
		}

		public bool ShowVerticalScroll() { return false; }
		public bool ShowHoritzonalScroll() { return false; }

		public GameObject CreateContent()
		{
			root = Helper.CreateGUIGameObject("TabConsole");
			Helper.SetRectTransform(root, 0, 0, 1, 1, 0.5f, 0.5f, 0, 0, 0, 0);

			GameObject inputdiv = Helper.CreateGUIGameObject("CommandField", root);
			Helper.SetRectTransformDPI(inputdiv, 0, 0, 1, 0, 0.5f, 0, -8.0f, 20.0f, 0, 4.0f);
			GameObject input = Helper.CreateInputField(overlay, inputdiv, "Command", "", ">");
			inputField = input.GetComponent<InputField>();

			GameObject console = Helper.CreateGUIGameObject("Console", root);
			Helper.SetRectTransformDPI(console, 0, 0, 1, 1, 0.5f, 1, -8.0f, -36.0f, 0.0f, -4.0f);
			Image consoleImage = console.AddOrGetComponent<Image>();
			consoleImage.type = Image.Type.Sliced;
			consoleImage.sprite = overlay.GetSpriteField();
			consoleImage.color = new Color(0.9f, 0.9f, 0.9f);

			GameObject innerConsole = Helper.CreateGUIGameObject("Text", console);
			Helper.SetRectTransformDPI(innerConsole, 0, 0, 1, 1, 0.5f, 0.5f, -16.0f, -16.0f, 0.0f, 0.0f);
			consoleText = innerConsole.AddOrGetComponent<Text>();
			consoleText.font = overlay.GetFontContentMonospace();
			consoleText.color = Color.black;
			consoleText.fontSize = overlay.FontSizeDPI();

			return root;
		}

		private void Start()
		{
			enabled = false;
			overlay = gameObject.FindManagerOrCreateIt<TabbedOverlay>();
			overlay.AddTab(this);

			consoleLog = ConsoleLog.Instance;
			consoleCommandsRepository = ConsoleCommandsRepository.Instance;
		}

		public void OnGUI()
		{
			HandleAutocomplete();

			if (KeyDown("[enter]") || KeyDown("return"))
				forceSubmit = true;
		}

		public void Update()
		{
			if (consoleText != null)
				consoleText.text = consoleLog.log;

			Submit();
		}

		private void Submit()
		{
			if (inputField == null)
				return;

			if (forceSubmit)
			{
				string input = inputField.text;

				string[] parts = input.TrimStart().Split(' ');
				string command = parts[0];
				string[] args = parts.Skip(1).ToArray();

				consoleLog.Log("> " + input);
				if (consoleCommandsRepository.HasCommand(command))
				{
					string response = consoleCommandsRepository.ExecuteCommand(command, args);
					if (response.Length > 0)
						consoleLog.Log(response);
				}
				else
				{
					if (command.Length > 0)
						consoleLog.Log("Command \"" + command + "\" not found");
				}

				inputField.text = "";
				forceSubmit = false;
			}
		}

		private void HandleAutocomplete()
		{
			if (inputField == null || !inputField.isFocused)
				return;

			string input = inputField.text;
			if (KeyDown("Tab"))
			{
				string[] parts = input.TrimStart().Split(' ');
				if (parts.Length > 1)
					return;

				int count = 0;
				string completed = "";
				string prefix = parts[0];
				foreach (string command in consoleCommandsRepository.Autocomplete(prefix))
				{
					if (count == 1)
					{
						consoleLog.Log("> " + input);
						consoleLog.Log(completed);
					}

					if (count > 0)
					{
						consoleLog.Log(command);
						completed = CommonPrefix(command, completed);
					}
					else
						completed = command;

					count++;
				}

				if (count > 0)
				{
					input = input.Substring(0, input.Length - prefix.Length);
					input += completed;
				}

				inputField.text = input;
			}
		}

		private bool KeyDown(string key)
		{
			return Event.current.Equals(Event.KeyboardEvent(key));
		}

		private string CommonPrefix(string left, string right)
		{
			string common = "";
			int minLen = System.Math.Min(left.Length, right.Length);
			for (int i = 0; i < minLen; i++)
			{
				if (left[i] == right[i])
					common += left[i];
				else
					break;
			}

			return common;
		}
	}
}

