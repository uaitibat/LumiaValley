using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game
{
    /// <summary>
    /// Porte direto de FarmerSprite.AnimationFrame + getAnimationFromIndex e
    /// FarmerRenderer.draw/drawHairAndAccesories da 1.5.6. Sem adivinhacao:
    /// cada offset aqui vem do codigo original.
    /// </summary>
    internal sealed class FarmerSpritePort
    {
        internal sealed class Frame
        {
            public int Number;
            public int Ms;
            public int PositionOffset;
            public int XOffset;
            public bool Flip;
            public bool SecondaryArm;
            public Frame(int n, int ms = 200, int po = 0, int xo = 0, bool flip = false, bool sec = false)
            { Number = n; Ms = ms; PositionOffset = po; XOffset = xo; Flip = flip; SecondaryArm = sec; }
        }

        private Frame[] frames = { new Frame(0) };
        private int index;
        private double timer;

        public Frame Current { get { return frames[index]; } }
        public int CurrentFrame { get { return Current.Number; } }

        public void SetDirection(int facing, bool walking)
        {
            // FarmerSprite.getAnimationFromIndex (linhas 1212-1239).
            Frame[] next;
            switch (facing)
            {
                case 0: next = new[] { new Frame(13), new Frame(12), new Frame(14), new Frame(12) }; break;
                case 1: next = new[] { new Frame(7), new Frame(6), new Frame(8), new Frame(6) }; break;
                case 3: next = new[] { new Frame(7, 200, 0, 0, true), new Frame(6, 200, 0, 0, true),
                                      new Frame(8, 200, 0, 0, true), new Frame(6, 200, 0, 0, true) }; break;
                default: next = new[] { new Frame(1), new Frame(0), new Frame(2), new Frame(0) }; break;
            }
            if (walking)
            {
                frames = next;
                index = 0;
                timer = 0;
            }
            else
            {
                // faceDirection: frame unico (12/6/0/6) com flip na esquerda.
                var idle = facing == 0 ? 12 : facing == 1 || facing == 3 ? 6 : 0;
                frames = new[] { new Frame(idle, 100, 0, 0, facing == 3) };
                index = 0;
                timer = 0;
            }
        }

        public void Update(double ms)
        {
            if (frames.Length <= 1) return;
            timer += ms;
            while (timer >= Current.Ms)
            {
                timer -= Current.Ms;
                index = (index + 1) % frames.Length;
            }
        }
    }

    internal sealed class FarmerRendererPort
    {
        private static readonly int[] Fx =
        {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1,0,0,0,0,0,0,0,0,0,0,-1,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,-1,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,4,0,0,0,0,-1,0,0,0,0,0,0,0,0,0,0,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0
        };
        private static readonly int[] Fy =
        {
            1,2,2,0,5,6,1,2,2,1,0,2,0,1,1,0,2,2,3,3,2,2,1,1,0,0,2,2,4,4,0,0,
            1,2,1,1,1,1,0,0,1,1,1,0,0,-2,-1,1,1,0,-1,-2,-1,-1,5,4,0,0,3,2,-1,0,
            4,2,0,0,2,1,0,-1,1,-2,0,0,1,1,1,1,1,1,0,0,0,0,1,-1,-1,-1,-1,1,1,0,
            0,0,0,4,1,0,1,2,1,0,1,0,1,2,-3,-4,-1,0,0,2,1,-4,-1,0,0,-3,0,0,-1,0,
            0,2,1,1
        };

        public static readonly Color[] HairPalette =
        {
            new Color(110,58,30), new Color(129,66,36), new Color(155,79,43),
            new Color(193,90,50), new Color(224,137,83), new Color(246,191,121),
            new Color(255,222,170), new Color(148,118,92), new Color(94,73,54),
            new Color(75,49,32), new Color(45,35,25), new Color(241,174,73)
        };
        public static readonly Color[] PantsPalette =
        {
            new Color(64,86,150), new Color(46,85,183), new Color(38,64,128),
            new Color(88,110,170), new Color(148,76,70), new Color(118,60,56),
            new Color(80,44,40), new Color(96,72,48), new Color(112,148,96),
            new Color(72,112,80), new Color(48,80,60), new Color(64,64,64),
            new Color(126,92,140), new Color(180,180,180), new Color(192,148,92),
            new Color(164,128,64), new Color(92,64,108), new Color(64,44,76),
            new Color(50,62,120), new Color(211,215,211)
        };
        public static readonly Color[] ShirtPalette =
        {
            new Color(211,215,211), new Color(88,110,170), new Color(64,86,150),
            new Color(50,62,120), new Color(148,76,70), new Color(118,60,56),
            new Color(80,44,40), new Color(192,148,92), new Color(148,106,64),
            new Color(96,72,48), new Color(212,176,92), new Color(164,128,64),
            new Color(112,148,96), new Color(72,112,80), new Color(48,80,60),
            new Color(126,92,140), new Color(92,64,108), new Color(64,44,76),
            new Color(180,180,180), new Color(64,64,64)
        };

        public static readonly Color[] EyePalette =
        {
            new Color(96,64,40), new Color(128,88,56), new Color(160,116,76),
            new Color(80,112,128), new Color(96,140,160), new Color(128,176,188),
            new Color(64,96,72), new Color(96,132,84), new Color(128,164,104),
            new Color(112,72,120), new Color(148,104,148), new Color(180,148,180)
        };

        private readonly Texture2D baseTexture, hairSheet, shirtSheet, pantSheet, accessorySheet;
        private readonly Texture2D skinColors, shoeColors, shirtsTexture;
        private readonly SaveSlot who;
        private Texture2D recolored;

        public FarmerRendererPort(Texture2D b, Texture2D h, Texture2D s, Texture2D p,
            Texture2D a, SaveSlot who, Texture2D skin, Texture2D shoes, Texture2D shirts)
        {
            baseTexture = b; hairSheet = h; shirtSheet = s; pantSheet = p; accessorySheet = a;
            skinColors = skin; shoeColors = shoes; shirtsTexture = shirts;
            this.who = who;
        }

        /// <summary>FarmerRenderer.executeRecolorActions: troca os pixels de
        /// referencia 260..262 (pele), 268..271 (sapato), 276..277 (olho) e
        /// 256..258 (manga) diretamente na textura. So o pixel com valor exato
        /// do indice e substituido (FarmerRenderer._SwapColor).</summary>
        private Texture2D Recolored(GraphicsDevice device)
        {
            if (recolored != null)
                return recolored;
            if (baseTexture == null)
                return null;

            var data = new Color[baseTexture.Width * baseTexture.Height];
            baseTexture.GetData(data);

            if (skinColors != null)
            {
                var pal = new Color[skinColors.Width * skinColors.Height];
                skinColors.GetData(pal);
                var w = Wrap(who.Skin, skinColors.Height) * 3;
                Swap(data, 260, pal[w]); Swap(data, 261, pal[w + 1]); Swap(data, 262, pal[w + 2]);
            }
            if (shoeColors != null)
            {
                var pal = new Color[shoeColors.Width * shoeColors.Height];
                shoeColors.GetData(pal);
                var w = Wrap(who.Shoes, shoeColors.Height) * 4;
                for (var i = 0; i < 4; i++) Swap(data, 268 + i, pal[w + i]);
            }
            var eye = EyePalette[Wrap(who.EyeColor, EyePalette.Length)];
            var dark = new Color(
                (byte)Math.Max(0, eye.R - 75),
                (byte)Math.Max(0, eye.G - 75),
                (byte)Math.Min(255, eye.B * 8 / 7));
            Swap(data, 276, eye);
            Swap(data, 277, dark);

            recolored = new Texture2D(device, baseTexture.Width, baseTexture.Height);
            recolored.SetData(data);
            return recolored;
        }

        private static void Swap(Color[] pixels, int index, Color color)
        {
            if (index < 0 || index >= pixels.Length) return;
            var refColor = pixels[index];
            for (var i = 0; i < pixels.Length; i++)
                if (pixels[i].PackedValue == refColor.PackedValue)
                    pixels[i] = color;
        }

        private static int Wrap(int v, int c) { v %= c; return v < 0 ? v + c : v; }

        private void DrawLayer(SpriteBatch b, Texture2D t, Vector2 pos, Rectangle src,
            Color color, SpriteEffects fx, float layer)
        {
            if (t == null || src.X < 0 || src.Y < 0 || src.Right > t.Width || src.Bottom > t.Height)
                return;
            b.Draw(t, pos, src, color, 0f, Vector2.Zero, 4f, fx, layer);
        }

        /// <summary>FarmerRenderer.draw + drawHairAndAccesories, portados.</summary>
        public void Draw(SpriteBatch b, FarmerSpritePort sprite, Vector2 pos, int facing, Color skin)
        {
            if (baseTexture == null) return;

            //executeRecolorActions: base recolorida, tint branco em todas as camadas.
            var baseTex = Recolored(b.GraphicsDevice) ?? baseTexture;
            skin = Color.White;
            var frame = sprite.Current;
            var currentFrame = frame.Number;
            var flip = frame.Flip;
            if (facing == 3 || facing == 1)
                facing = flip ? 3 : 1;
            var effects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            var off = new Vector2(frame.XOffset * 4f, frame.PositionOffset * 4f);
            var fx = Fx[Wrap(currentFrame, Fx.Length)];
            var fy = Fy[Wrap(currentFrame, Fy.Length)];

            var hairIndex = Wrap(who.Hair, 56);
            var hairColor = HairPalette[Wrap(who.HairColor, HairPalette.Length)];
            var eyeColor = EyePalette[Wrap(who.EyeColor, EyePalette.Length)];
            var male = who.IsMale;

            // Braco: facing 0 (cima) tem layerDepth negativo, vai ATRAS do
            // corpo. Desenhamos antes com depth negativo, como no original.
            if (facing == 0)
                DrawLayer(b, baseTex, pos + off,
                    new Rectangle(96 + currentFrame % 6 * 16, currentFrame / 6 * 32, 16, 32),
                    skin, effects, -0.0001f);

            // 1. Corpo
            var bodySrc = new Rectangle(currentFrame % 6 * 16, currentFrame / 6 * 32, 16, 32);
            DrawLayer(b, baseTex, pos + off, bodySrc, skin, effects, 0f);

            // 2. Calca
            if (pantSheet != null)
            {
                var style = Wrap(who.Pants, 20);
                var psrc = new Rectangle(style % 10 * 192 + (male ? 0 : 96), style / 10 * 688, 16, 32);
                DrawLayer(b, pantSheet, pos + off, psrc, PantsPalette[Wrap(who.PantsColor, 20)], effects, 0.000000092f);
            }

            // 3. Olhos (draw:764-781)
            var eyeX = (5 + fx + (facing == 1 ? 3 : facing == 3 ? 1 : 0)) * 4f;
            var whiteY = fy * 4f + (male && who.Facing != 2 ? 36f : 40f);
            var irisY = fy * 4f + (who.Facing == 1 || who.Facing == 3 ? 40f : 44f);
            DrawLayer(b, baseTex, pos + off + new Vector2(eyeX, whiteY),
                new Rectangle(5, 16, facing == 2 ? 6 : 2, 2), skin, SpriteEffects.None, 0.00000005f);
            DrawLayer(b, baseTex, pos + off + new Vector2(eyeX, irisY),
                new Rectangle(264 + (facing == 3 ? 4 : 0), 2, facing == 2 ? 6 : 2, 2),
                eyeColor, SpriteEffects.None, 0.00000012f);

            // 4. Camisa + acessorio + cabelo (drawHairAndAccesories)
            var shirtSrc = new Rectangle(Wrap(who.Shirt, 112) * 8 % 128,
                Wrap(who.Shirt, 112) * 8 / 128 * 32, 8, 8);
            var hairSrc = new Rectangle(hairIndex % 8 * 16, hairIndex / 8 * 96, 16, 32);
            var accSrc = new Rectangle(Wrap(who.Accessory, 20) * 16 % 128,
                Wrap(who.Accessory, 20) * 16 / 128 * 32, 16, 16);

            var shirtOffY = facing == 0 ? 24 : facing == 1 ? 8 : facing == 3 ? 16 : 0;
            shirtSrc.Y += shirtOffY;
            var hairOffY = facing == 0 ? 64 : facing == 1 || facing == 3 ? 32 : 0;
            hairSrc.Y += hairOffY;
            if (facing == 1 || facing == 3) accSrc.Y += 16;

            var shirtSign = facing == 3 ? -1f : 1f;
            var shirtPos = pos + off + new Vector2(16f + shirtSign * fx * 4f, 56f + fy * 4f);
            if (shirtSheet != null)
            {
                DrawLayer(b, shirtSheet, shirtPos, shirtSrc, Color.White, SpriteEffects.None, 0.00000015f);
                DrawLayer(b, shirtSheet, shirtPos, new Rectangle(shirtSrc.X + 128, shirtSrc.Y, 8, 8),
                    ShirtPalette[Wrap(who.ShirtColor, ShirtPalette.Length)],
                    SpriteEffects.None, 0.000000151f);
            }

            if (who.Accessory >= 0 && facing != 0 && accessorySheet != null)
            {
                var accY = facing == 2 ? 8f : 4f;
                DrawLayer(b, accessorySheet,
                    pos + off + new Vector2(shirtSign * fx * 4f, accY + fy * 4f - (facing == 2 ? 4f : 0f)),
                    accSrc, Color.White, effects, 0.00000019f);
            }

            if (hairSheet != null)
            {
                var gender = male && hairIndex >= 16 ? -4f : !male && hairIndex < 16 ? 4f : 0f;
                if (facing == 0) gender += 4f;
                DrawLayer(b, hairSheet,
                    pos + off + new Vector2(shirtSign * fx * 4f, fy * 4f + gender),
                    hairSrc, hairColor, effects, 0.00000022f);
            }

            // 5. Braco (FarmerRenderer:789) — na direcao 0 ja foi desenhado atras.
            if (facing != 0)
                DrawLayer(b, baseTex, pos + off,
                    new Rectangle(96 + currentFrame % 6 * 16, currentFrame / 6 * 32, 16, 32),
                    skin, effects, 0.000049f);
        }
    }
}
