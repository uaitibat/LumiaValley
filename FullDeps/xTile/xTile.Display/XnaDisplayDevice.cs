using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace xTile.Display;

public class XnaDisplayDevice : IDisplayDevice
{
	private ContentManager m_contentManager;

	private GraphicsDevice m_graphicsDevice;

	private SpriteBatch m_spriteBatchAlpha;

	private SpriteBatch m_spriteBatchAdditive;

	private Dictionary<TileSheet, Texture2D> m_tileSheetTextures;

	private Vector2 m_tilePosition;

	private Microsoft.Xna.Framework.Rectangle m_sourceRectangle;

	private Color m_modulationColour;

	public Color ModulationColour
	{
		get
		{
			return m_modulationColour;
		}
		set
		{
			m_modulationColour = value;
		}
	}

	public SpriteBatch SpriteBatchAlpha => m_spriteBatchAlpha;

	public SpriteBatch SpriteBatchAdditive => m_spriteBatchAdditive;

	public XnaDisplayDevice(ContentManager contentManager, GraphicsDevice graphicsDevice)
	{
		m_contentManager = contentManager;
		m_graphicsDevice = graphicsDevice;
		m_spriteBatchAlpha = new SpriteBatch(graphicsDevice);
		m_spriteBatchAdditive = new SpriteBatch(graphicsDevice);
		m_tileSheetTextures = new Dictionary<TileSheet, Texture2D>();
		m_tilePosition = default(Vector2);
		m_sourceRectangle = default(Microsoft.Xna.Framework.Rectangle);
		m_modulationColour = Color.White;
	}

	public void LoadTileSheet(TileSheet tileSheet)
	{
		Texture2D value = m_contentManager.Load<Texture2D>(tileSheet.ImageSource);
		m_tileSheetTextures[tileSheet] = value;
	}

	public void DisposeTileSheet(TileSheet tileSheet)
	{
		m_tileSheetTextures.Remove(tileSheet);
	}

	public void BeginScene(SpriteBatch b)
	{
		m_spriteBatchAlpha = b;
	}

	public void SetClippingRegion(xTile.Dimensions.Rectangle clippingRegion)
	{
		int backBufferWidth = m_graphicsDevice.PresentationParameters.BackBufferWidth;
		int backBufferHeight = m_graphicsDevice.PresentationParameters.BackBufferHeight;
		int num = Clamp(clippingRegion.X, 0, backBufferWidth);
		int num2 = Clamp(clippingRegion.Y, 0, backBufferHeight);
		int num3 = Clamp(clippingRegion.X + clippingRegion.Width, 0, backBufferWidth);
		int num4 = Clamp(clippingRegion.Y + clippingRegion.Height, 0, backBufferHeight);
		int width = num3 - num;
		int height = num4 - num2;
		m_graphicsDevice.Viewport = new Viewport(num, num2, width, height);
	}

	public void DrawTile(Tile tile, Location location, float layerDepth)
	{
		if (tile != null)
		{
			xTile.Dimensions.Rectangle tileImageBounds = tile.TileSheet.GetTileImageBounds(tile.TileIndex);
			Texture2D texture2D = m_tileSheetTextures[tile.TileSheet];
			if (!texture2D.IsDisposed)
			{
				m_tilePosition.X = location.X;
				m_tilePosition.Y = location.Y;
				m_sourceRectangle.X = tileImageBounds.X;
				m_sourceRectangle.Y = tileImageBounds.Y;
				m_sourceRectangle.Width = tileImageBounds.Width;
				m_sourceRectangle.Height = tileImageBounds.Height;
				m_spriteBatchAlpha.Draw(texture2D, m_tilePosition, m_sourceRectangle, m_modulationColour, 0f, Vector2.Zero, Layer.zoom, SpriteEffects.None, layerDepth);
			}
		}
	}

	public void EndScene()
	{
	}

	private int Clamp(int nValue, int nMin, int nMax)
	{
		return Math.Min(Math.Max(nValue, nMin), nMax);
	}
}
