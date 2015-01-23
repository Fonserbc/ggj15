using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate string ConsoleCommandCallback(params string[] args);

public class ConsoleCommandsRepository
{
	private static ConsoleCommandsRepository instance;
	private Dictionary<string, ConsoleCommandCallback> repository;

	public static ConsoleCommandsRepository Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ConsoleCommandsRepository();
			}

			return instance;
		}
	}

	public ConsoleCommandsRepository()
	{
		repository = new Dictionary<string, ConsoleCommandCallback>();
	}

	public void RegisterCommand(string command, ConsoleCommandCallback callback)
	{
		repository[command] = new ConsoleCommandCallback(callback);
	}

	public IEnumerable<string> Autocomplete(string prefix)
	{
		foreach (KeyValuePair<string, ConsoleCommandCallback> pair in repository)
		{
			if (pair.Key.StartsWith(prefix))
			{
				yield return pair.Key.ToString();
			}
		}
	}

	public bool HasCommand(string command)
	{
		return repository.ContainsKey(command);
	}

	public string ExecuteCommand(string command, string[] args)
	{
		if (HasCommand(command))
		{
			return repository[command](args);
		}
		else
		{
			return "Command not found";
		}
	}
}

