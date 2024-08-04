#region Copyright & License Information
/*
 * Copyright (c) The OpenRA Developers and Contributors
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System.Linq;
using Eluant;
using OpenRA.Mods.Common.Widgets;
using OpenRA.Scripting;

namespace OpenRA.Mods.Common.Scripting
{
	[ScriptGlobal("Utils")]
	public class UtilsGlobal : ScriptGlobal
	{
		public UtilsGlobal(ScriptContext context)
			: base(context) { }

		[Desc("Calls a function on every element in a collection.")]
		public void Do(
			[ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] collection,
			[ScriptEmmyTypeOverride("fun(item: T)", "T")] LuaFunction func)
		{
			foreach (var c in collection)
				func.Call(c).Dispose();
		}

		[Desc("Returns true if func returns true for any element in a collection.")]
		public bool Any(
			[ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] collection,
			[ScriptEmmyTypeOverride("fun(item: T):boolean?", "T")] LuaFunction func)
		{
			foreach (var c in collection)
				using (var ret = func.Call(c))
				using (var result = ret.FirstOrDefault())
					if (result != null && result.ToBoolean())
						return true;

			return false;
		}

		[Desc("Returns true if func returns true for all elements in a collection.")]
		public bool All(
			[ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] collection,
			[ScriptEmmyTypeOverride("fun(item: T):boolean?", "T")] LuaFunction func)
		{
			foreach (var c in collection)
				using (var ret = func.Call(c))
				using (var result = ret.FirstOrDefault())
					if (result == null || !result.ToBoolean())
						return false;

			return true;
		}

		[Desc("Returns the original collection filtered with the func.")]
		[return: ScriptEmmyTypeOverride("T[]", "T")]
		public LuaTable Where(
			[ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] collection,
			[ScriptEmmyTypeOverride("fun(item: T):boolean?", "T")] LuaFunction func)
		{
			var t = Context.CreateTable();

			foreach (var c in collection)
				using (var ret = func.Call(c))
				using (var result = ret.FirstOrDefault())
					if (result != null && result.ToBoolean())
						t.Add(t.Count + 1, c);

			return t;
		}

		[Desc("Returns the first n values from a collection.")]
		[return: ScriptEmmyTypeOverride("T[]", "T")]
		public LuaValue[] Take(
			int n,
			[ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] source)
		{
			return source.Take(n).Select(v => v.CopyReference()).ToArray();
		}

		[Desc("Skips over the first numElements members of a table and return the rest.")]
		[return: ScriptEmmyTypeOverride("T[]", "T")]
		public LuaTable Skip(
			[ScriptEmmyTypeOverride("T[]", "T")] LuaTable table,
			int numElements)
		{
			var t = Context.CreateTable();

			for (var i = numElements; i <= table.Count; i++)
				using (LuaValue key = t.Count + 1, value = table[i])
					t.Add(key, value);

			return t;
		}

		[Desc("Concatenates two Lua tables into a single table.")]
		[return: ScriptEmmyTypeOverride("T[]", "T")]
		public LuaTable Concat(
			[ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] firstCollection,
			[ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] secondCollection)
		{
			var t = Context.CreateTable();

			foreach (var e in firstCollection)
				t.Add(t.Count + 1, e);

			foreach (var e in secondCollection)
				t.Add(t.Count + 1, e);

			return t;
		}

		[Desc("Returns a random value from a collection.")]
		[return: ScriptEmmyTypeOverride("T", "T")]
		public LuaValue Random([ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] collection)
		{
			return collection.Random(Context.World.SharedRandom).CopyReference();
		}

		[Desc("Returns the collection in a random order.")]
		[return: ScriptEmmyTypeOverride("T[]", "T")]
		public LuaValue[] Shuffle([ScriptEmmyTypeOverride("T[]", "T")] LuaValue[] collection)
		{
			return collection.Shuffle(Context.World.SharedRandom).ToArray();
		}

		[Desc("Expands the given footprint one step along the coordinate axes, and (if requested) diagonals.")]
		public CPos[] ExpandFootprint(CPos[] footprint, bool allowDiagonal)
		{
			return Util.ExpandFootprint(footprint, allowDiagonal).ToArray();
		}

		[Desc("Returns a random integer x in the range low &lt;= x &lt; high.")]
		public int RandomInteger(int low, int high)
		{
			if (high <= low)
				return low;

			return Context.World.SharedRandom.Next(low, high);
		}

		[Desc("Returns the ticks formatted to HH:MM:SS.")]
		public string FormatTime(int ticks, bool leadingMinuteZero = true)
		{
			return WidgetUtils.FormatTime(ticks, leadingMinuteZero, 40);
		}
	}
}
