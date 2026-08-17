using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionKit.Extras.FutileExtras
{
	/// <summary>
	/// A take on a general purpose rectangular box made up of several pixel <see cref="FSprite"/>s.
	/// </summary>
	internal class FRectangle
	{
		internal FSprite[] Fsprites { get; }
		internal Vector2 Scale { get => _origScale; }
		private Vector2 _origScale;

		internal FRectangle(Vector2 size)
		{
			_origScale = size;
			Fsprites = [
				new("pixel") { anchorX = 0f }, // Bottom
					new("pixel") { anchorY = 0f }, // Left
					new("pixel") { anchorX = 0f }, // Top
					new("pixel") { anchorY = 0f }, // Right
				];
			Resize(size.x, size.y);
		}

		internal void Resize(float scale)
		{
			_origScale = new(scale, scale);
			for (int i = 0; i < Fsprites.Length; i++)
			{
				if (i % 2 == 0)
				{
					Fsprites[i].scaleX = scale;
				}
				else
				{
					Fsprites[i].scaleY = scale;
				}
			}
		}

		internal void Resize(float scaleX, float scaleY)
		{
			_origScale = new(scaleX, scaleY);
			for (int i = 0; i < Fsprites.Length; i++)
			{
				if (i % 2 == 0)
				{
					Fsprites[i].scaleX = scaleX;
				}
				else
				{
					Fsprites[i].scaleY = scaleY;
				}
			}
		}

		internal void SetPosition(Vector2 pos)
		{
			for (int i = 0; i < Fsprites.Length; i++)
			{
				Fsprites[i].SetPosition(pos
					+ i switch
					{
						2 => new(0f, Scale.y),
						3 => new(Scale.x, 0f),
						_ => new(0f, 0f)
					});
			}
		}

		internal void Show(bool show)
		{
			foreach (FSprite sprite in Fsprites)
			{
				sprite.isVisible = show;
			}
		}

		internal void MoveToFront()
		{
			foreach (FSprite sprite in Fsprites)
			{
				sprite.MoveToFront();
			}
		}
	}
}
