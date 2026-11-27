// Shim de SpriteText sobre SpriteFont para o experimento de bulk port.
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.BellsAndWhistles
{
    public enum ScrollTextAlignment { Left, Center, Right }

    public static class SpriteText
    {
        private static SpriteFont font;

        private static SpriteFont Font
        {
            get
            {
                if (font == null && Game1.game1 != null && Game1.game1.Content != null)
                {
                    try { font = Game1.game1.Content.Load<SpriteFont>("Fonts/SmallFont"); }
                    catch { font = null; }
                }
                return font;
            }
        }

        public static Color getColorFromIndex(int index)
        {
            switch (index)
            {
                case 0: return new Color(34, 17, 34);
                case 1: return new Color(255, 238, 160);
                case 2: return new Color(255, 0, 0);
                case 3: return new Color(180, 180, 180);
                case 4: return new Color(64, 136, 248);
                default: return Color.White;
            }
        }

        public static int getWidthOfString(string s, int widthConstraint = 999999)
        {
            if (s == null) return 0;
            var f = Font;
            return f == null ? s.Length * 8 : (int)f.MeasureString(s).X;
        }

        public static int getHeightOfString(string s, int widthConstraint = 999999)
        {
            var f = Font;
            return f == null ? 16 : (int)f.LineSpacing;
        }

        public static bool IsMissingCharacters(string text) { return false; }

        public static string getSubstringBeyondHeight(string s, int width, int height) { return ""; }
        public static int getIndexOfSubstringBeyondHeight(string s, int width, int height) { return s == null ? 0 : s.Length; }
        public static List<string> getStringBrokenIntoSectionsOfHeight(string s, int width, int height)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(s)) result.Add(s);
            return result;
        }
        public static string getStringPreviousToThisHeightCutoff(string s, int width, int height) { return s; }
        public static int getWidthOffsetForChar(char c) { return 8; }
        public static int positionOfNextSpace(string s, int index, int currentXPosition, int accumulatedHorizontalSpaceBetweenCharacters)
        {
            return s == null ? 0 : s.IndexOf(' ', index);
        }

        public static void drawString(SpriteBatch b, string s, int x, int y,
            int characterPosition = 999999, int width = -1, int height = 999999,
            float alpha = 1f, float layerDepth = 0.88f, bool junimoText = false,
            int drawBGScroll = -1, string placeHolderScrollWidthText = "",
            int color = -1, ScrollTextAlignment scroll_text_alignment = ScrollTextAlignment.Left)
        {
            var f = Font;
            if (b == null || f == null || string.IsNullOrEmpty(s)) return;
            var pos = new Vector2(x, y);
            if (scroll_text_alignment == ScrollTextAlignment.Center)
                pos.X -= f.MeasureString(s).X / 2f;
            else if (scroll_text_alignment == ScrollTextAlignment.Right)
                pos.X -= f.MeasureString(s).X;
            b.DrawString(f, s, pos, getColorFromIndex(color) * alpha,
                0f, Vector2.Zero, 1.5f, SpriteEffects.None, layerDepth);
        }

        public static void drawStringHorizontallyCenteredAt(SpriteBatch b, string s, int x, int y,
            int characterPosition = 999999, int width = -1, int height = 999999,
            float alpha = 1f, float layerDepth = 0.88f, bool junimoText = false,
            int color = -1, int maxWidth = 99999)
        {
            drawString(b, s, x, y, characterPosition, width, height, alpha,
                layerDepth, junimoText, -1, "", color, ScrollTextAlignment.Center);
        }

        public static void drawStringWithScrollCenteredAt(SpriteBatch b, string s, int x, int y,
            int width, float alpha = 1f, int color = -1, int scrollType = 0,
            float layerDepth = 0.88f, bool junimoText = false)
        {
            drawStringHorizontallyCenteredAt(b, s, x, y, 999999, width, 999999,
                alpha, layerDepth, junimoText, color, width);
        }

        public static void drawStringWithScrollCenteredAt(SpriteBatch b, string s, int x, int y,
            string placeHolderWidthText = "", float alpha = 1f, int color = -1,
            int scrollType = 0, float layerDepth = 0.88f, bool junimoText = false)
        {
            drawStringHorizontallyCenteredAt(b, s, x, y, 999999, -1, 999999,
                alpha, layerDepth, junimoText, color, 99999);
        }

        public static void drawStringWithScrollBackground(SpriteBatch b, string s, int x, int y,
            string placeHolderWidthText = "", float alpha = 1f, int color = -1,
            ScrollTextAlignment scroll_text_alignment = ScrollTextAlignment.Left)
        {
            drawString(b, s, x, y, 999999, -1, 999999, alpha, 0.88f, false,
                -1, placeHolderWidthText, color, scroll_text_alignment);
        }
    }
}
