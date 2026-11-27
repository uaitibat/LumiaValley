using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley;

/// <summary>
/// Reusable component makes it easy to check whether your important
/// graphics are positioned inside the title safe area, by superimposing
/// a red border that marks the edges of the safe region.
/// </summary>
public class SafeAreaOverlay : DrawableGameComponent
{
	private SpriteBatch spriteBatch;

	private Texture2D dummyTexture;

	/// <summary>
	/// Constructor.
	/// </summary>
	public SafeAreaOverlay(Game game)
		: base(game)
	{
		base.DrawOrder = 1000;
	}

	/// <summary>
	/// Creates the graphics resources needed to draw the overlay.
	/// </summary>
	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(Game1.graphics.GraphicsDevice);
		dummyTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
		dummyTexture.SetData(new Color[1] { Color.White });
	}

	/// <summary>
	/// Draws the title safe area.
	/// </summary>
	public override void Draw(GameTime gameTime)
	{
		Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
		Rectangle titleSafeArea = viewport.GetTitleSafeArea();
		int num = viewport.X + viewport.Width;
		int num2 = viewport.Y + viewport.Height;
		Rectangle destinationRectangle = new Rectangle(viewport.X, viewport.Y, titleSafeArea.X - viewport.X, viewport.Height);
		Rectangle destinationRectangle2 = new Rectangle(titleSafeArea.Right, viewport.Y, num - titleSafeArea.Right, viewport.Height);
		Rectangle destinationRectangle3 = new Rectangle(titleSafeArea.Left, viewport.Y, titleSafeArea.Width, titleSafeArea.Top - viewport.Y);
		Rectangle destinationRectangle4 = new Rectangle(titleSafeArea.Left, titleSafeArea.Bottom, titleSafeArea.Width, num2 - titleSafeArea.Bottom);
		Color red = Color.Red;
		spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
		spriteBatch.Draw(dummyTexture, destinationRectangle, red);
		spriteBatch.Draw(dummyTexture, destinationRectangle2, red);
		spriteBatch.Draw(dummyTexture, destinationRectangle3, red);
		spriteBatch.Draw(dummyTexture, destinationRectangle4, red);
		spriteBatch.End();
	}
}
