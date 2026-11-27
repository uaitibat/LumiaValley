using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley;

internal class ColorChanger
{
	[InstancedStatic]
	private static Color[] _buffer;

	public static Texture2D swapColor(Texture2D texture, int targetColorIndex, int red, int green, int blue)
	{
		return swapColor(texture, targetColorIndex, red, green, blue, 0, texture.Width * texture.Height);
	}

	private static Color[] getBuffer(int len)
	{
		if (_buffer == null || _buffer.Length < len)
		{
			_buffer = new Color[len];
		}
		return _buffer;
	}

	public unsafe static Texture2D swapColor(Texture2D texture, int targetColorIndex1, int r1, int g1, int b1, int startPixelIndex, int endPixelIndex)
	{
		r1 = Math.Min(Math.Max(1, r1), 255);
		g1 = Math.Min(Math.Max(1, g1), 255);
		b1 = Math.Min(Math.Max(1, b1), 255);
		uint packedValue = new Color(r1, g1, b1).PackedValue;
		int num = texture.Width * texture.Height;
		Color[] buffer = getBuffer(num);
		texture.GetData(buffer, 0, num);
		Color color = buffer[targetColorIndex1];
		uint packedValue2 = color.PackedValue;
		fixed (Color* ptr = buffer)
		{
			Color* num2 = ptr + startPixelIndex;
			Color* ptr2 = ptr + endPixelIndex;
			for (Color* ptr3 = num2; ptr3 <= ptr2; ptr3++)
			{
				if (ptr3->PackedValue == packedValue2)
				{
					ptr3->PackedValue = packedValue;
				}
			}
		}
		texture.SetData(buffer, 0, num);
		return texture;
	}

	public unsafe static void swapColors(Texture2D texture, int targetColorIndex1, byte r1, byte g1, byte b1, int targetColorIndex2, byte r2, byte g2, byte b2)
	{
		r1 = Math.Min(Math.Max((byte)1, r1), byte.MaxValue);
		g1 = Math.Min(Math.Max((byte)1, g1), byte.MaxValue);
		b1 = Math.Min(Math.Max((byte)1, b1), byte.MaxValue);
		r2 = Math.Min(Math.Max((byte)1, r2), byte.MaxValue);
		g2 = Math.Min(Math.Max((byte)1, g2), byte.MaxValue);
		b2 = Math.Min(Math.Max((byte)1, b2), byte.MaxValue);
		Color color = new Color(r1, g1, b1);
		Color color2 = new Color(r2, g2, b2);
		uint packedValue = color.PackedValue;
		uint packedValue2 = color2.PackedValue;
		int num = texture.Width * texture.Height;
		Color[] buffer = getBuffer(num);
		texture.GetData(buffer, 0, num);
		Color color3 = buffer[targetColorIndex1];
		Color color4 = buffer[targetColorIndex2];
		uint packedValue3 = color3.PackedValue;
		uint packedValue4 = color4.PackedValue;
		int num2 = 0;
		int num3 = num;
		fixed (Color* ptr = buffer)
		{
			Color* num4 = ptr + num2;
			Color* ptr2 = ptr + num3;
			for (Color* ptr3 = num4; ptr3 <= ptr2; ptr3++)
			{
				if (ptr3->PackedValue == packedValue3)
				{
					ptr3->PackedValue = packedValue;
				}
				else if (ptr3->PackedValue == packedValue4)
				{
					ptr3->PackedValue = packedValue2;
				}
			}
		}
		texture.SetData(buffer, 0, num);
	}
}
