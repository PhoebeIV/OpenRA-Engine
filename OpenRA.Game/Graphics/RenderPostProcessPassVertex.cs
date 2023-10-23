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

using System.Runtime.InteropServices;

namespace OpenRA.Graphics
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct RenderPostProcessPassVertex
	{
		public readonly float X, Y;

		public RenderPostProcessPassVertex(float x, float y)
		{
			X = x; Y = y;
		}
	}

	public sealed class RenderPostProcessPassShaderBindings : ShaderBindings
	{
		public RenderPostProcessPassShaderBindings(string name)
			: base("postprocess", "postprocess_" + name) { }

		public override ShaderVertexAttribute[] Attributes { get; } = new[]
		{
			new ShaderVertexAttribute("aVertexPosition", 2, 0)
		};
	}
}
