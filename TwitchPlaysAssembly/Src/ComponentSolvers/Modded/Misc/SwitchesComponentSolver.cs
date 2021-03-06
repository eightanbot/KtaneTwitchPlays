﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class SwitchesComponentSolver : ComponentSolver
{
	public SwitchesComponentSolver(TwitchModule module) :
		base(module)
	{
		_component = module.BombComponent.GetComponent(ComponentType);
		ModInfo = ComponentSolverFactory.GetModuleInfo(GetModuleType(), "Flip switches using !{0} flip 1 5 3 2.");
	}

	protected internal override IEnumerator RespondToCommandInternal(string inputCommand)
	{
		string[] commands = inputCommand.ToLowerInvariant().Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

		if (commands.Length <= 1 || !commands[0].EqualsAny("flip", "switch", "press", "toggle")) yield break;

		IEnumerable<int?> switches = commands.Where((_, i) => i > 0).Select(n => n.TryParseInt());
		IEnumerable<int?> switchIndices = switches as int?[] ?? switches.ToArray();
		if (!switchIndices.All(n => n != null && n > 0 && n < 6)) yield break;

		yield return null;
		if (switchIndices.Count() > 20)
		{
			yield return "elevator music";
		}

		foreach (int? switchIndex in switchIndices)
		{
			OnToggleMethod.Invoke(_component, new object[] { switchIndex - 1 });
			yield return "trywaitcancel 0.1";
		}
	}

	static SwitchesComponentSolver()
	{
		ComponentType = ReflectionHelper.FindType("SwitchModule");
		OnToggleMethod = ComponentType.GetMethod("OnToggle", BindingFlags.NonPublic | BindingFlags.Instance);
	}

	private static readonly Type ComponentType;
	private static readonly MethodInfo OnToggleMethod;

	private readonly object _component;
}
