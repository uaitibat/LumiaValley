using System.Collections.Generic;

namespace LumiaValley.Game.Map
{
    /// <summary>Representacao leve do mapa xTile, sem dependencia da dll xTile.</summary>
    public sealed class TileMap
    {
        public string Id;
        public string Description;
        public readonly List<TileSheetData> TileSheets = new List<TileSheetData>();
        public readonly List<TileLayer> Layers = new List<TileLayer>();
        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();

        public int Width
        {
            get { return Layers.Count > 0 ? Layers[0].Width : 0; }
        }

        public int Height
        {
            get { return Layers.Count > 0 ? Layers[0].Height : 0; }
        }
    }

    public sealed class TileSheetData
    {
        public string Id;
        public string ImageSource;
        public int SheetWidth;
        public int SheetHeight;
        public int TileWidth;
        public int TileHeight;
        public int MarginX;
        public int MarginY;
        public int SpacingX;
        public int SpacingY;
    }

    public sealed class TileLayer
    {
        public string Id;
        public bool Visible;
        public int Width;
        public int Height;
        public int TileWidth;
        public int TileHeight;
        public TileData[] Tiles;

        public TileData Get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height || Tiles == null)
                return null;
            return Tiles[y * Width + x];
        }
    }

    public sealed class TileData
    {
        public TileSheetData Sheet;
        public int Index;
        public int Blend;
        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();
    }
}
