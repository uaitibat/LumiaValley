using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LumiaValley.Game.Map
{
    /// <summary>
    /// Categories de interacao dos moveis da 1.5.6. Servem para que, ao
    /// posicionar um movel em qualquer casa/cenario, a funcao ja venha pronta
    /// em vez de ser reimplementada a mao para cada mapa.
    /// </summary>
    public enum FurnitureInteraction
    {
        None,
        Bed,
        Chair,
        Sittable,
        Table,
        TV,
        Fireplace,
        Torch,
        Lamp,
        Decor,
        Painting,
        Bookcase,
        Rug,
        Chest,
        Door,
        Bin,
        Workbench
    }

    /// <summary>
    /// Definicao completa de uma linha de Data/Furniture + classificacao de
    /// interacao. Campos 0..4 do formato original (name/type/size/bbox/rot).
    /// </summary>
    public sealed class FurnitureInfo
    {
        public int Which;
        public string Name;
        public int Type;
        public int Width;
        public int Height;
        public int BoxW;
        public int BoxH;
        public int Rotations;
        public FurnitureInteraction Interaction;
    }

    /// <summary>
    /// Catalogo carregado de Data/Furniture. E a fonte unica de verdade para
    /// desenho, colisao e interacao — qualquer cenario pode consulta-lo.
    /// </summary>
    public sealed class FurnitureCatalog
    {
        private readonly Dictionary<int, FurnitureInfo> _items = new Dictionary<int, FurnitureInfo>();

        public void Load(ContentManager content)
        {
            var data = content.Load<Dictionary<int, string>>("Data/Furniture");
            foreach (var pair in data)
            {
                var parts = pair.Value.Split('/');
                var info = new FurnitureInfo { Which = pair.Key, Name = parts.Length > 0 ? parts[0] : "" };
                info.Type = FurnitureTypeNumber(info.Name);
                if (parts.Length > 2 && parts[2] != "-1")
                {
                    var d = parts[2].Split(' ');
                    info.Width = int.Parse(d[0]);
                    info.Height = int.Parse(d[1]);
                }
                else
                {
                    info.Width = DefaultWidth(info.Type);
                    info.Height = DefaultHeight(info.Type);
                }
                if (parts.Length > 3 && parts[3] != "-1")
                {
                    var b = parts[3].Split(' ');
                    info.BoxW = int.Parse(b[0]);
                    info.BoxH = int.Parse(b[1]);
                }
                else
                {
                    info.BoxW = DefaultBoxWidth(info.Type);
                    info.BoxH = DefaultBoxHeight(info.Type);
                }
                if (parts.Length > 4) int.TryParse(parts[4], out info.Rotations);
                info.Interaction = InteractionFor(info.Type, info.Name);
                _items[pair.Key] = info;
            }
        }

        public FurnitureInfo Get(int which)
        {
            FurnitureInfo info;
            return _items.TryGetValue(which, out info) ? info : null;
        }

        public IEnumerable<FurnitureInfo> All { get { return _items.Values; } }

        /// <summary>
        /// Mapeamento type -> interacao. Baseado em Furniture.isPassable/
        /// IntersectsForCollision e nos IDs fixos de Game1.loadForNewGame.
        /// </summary>
        public static FurnitureInteraction InteractionFor(int type, string name)
        {
            var lower = (name ?? "").ToLowerInvariant();
            if (type == 15 || lower.StartsWith("bed")) return FurnitureInteraction.Bed;
            if (type == 12) return FurnitureInteraction.Rug;
            if (type == 14) return FurnitureInteraction.Fireplace;
            if (type == 16 || type == 17) return FurnitureInteraction.Torch;
            if (type == 7) return FurnitureInteraction.Lamp;
            if (type == 6) return FurnitureInteraction.Painting;
            if (type == 10) return FurnitureInteraction.Bookcase;
            if (type == 11) return FurnitureInteraction.Table;
            if (lower.Contains("tv")) return FurnitureInteraction.TV;
            if (lower.Contains("chest")) return FurnitureInteraction.Chest;
            if (lower.Contains("bin") || lower.Contains("trash")) return FurnitureInteraction.Bin;
            if (lower.Contains("workbench")) return FurnitureInteraction.Workbench;
            if (lower.Contains("chair") || lower.Contains("bench")) return FurnitureInteraction.Chair;
            if (lower.Contains("couch") || lower.Contains("armchair")) return FurnitureInteraction.Sittable;
            if (lower.Contains("door")) return FurnitureInteraction.Door;
            return FurnitureInteraction.Decor;
        }

        public static int FurnitureTypeNumber(string name)
        {
            var lower = (name ?? "").ToLowerInvariant();
            if (lower.StartsWith("bed")) return 15;
            switch (lower)
            {
                case "chair": return 0;
                case "bench": return 1;
                case "couch": return 2;
                case "armchair": return 3;
                case "dresser": return 4;
                case "long table": return 5;
                case "painting": return 6;
                case "lamp": return 7;
                case "decor": return 8;
                case "bookcase": return 10;
                case "table": return 11;
                case "rug": return 12;
                case "window": return 13;
                case "fireplace": return 14;
                case "torch": return 16;
                case "sconce": return 17;
                default: return 9;
            }
        }

        private static int DefaultWidth(int t)
        {
            switch (t)
            {
                case 1: case 3: case 4: case 6: return 2;
                case 2: case 12: return 3;
                case 5: return 5;
                case 10: case 11: return 2;
                case 7: return 1;
                case 14: return 2;
                default: return 1;
            }
        }

        private static int DefaultHeight(int t)
        {
            switch (t)
            {
                case 2: case 12: return 2;
                case 5: return 3;
                case 10: case 11: return 3;
                case 7: return 3;
                case 14: return 5;
                default: return 2;
            }
        }

        private static int DefaultBoxWidth(int t)
        {
            switch (t)
            {
                case 1: case 3: case 4: case 10: case 14: return 2;
                case 2: case 12: return 3;
                case 5: return 5;
                case 6: case 11: return 2;
                case 17: case 13: return 1;
                default: return 1;
            }
        }

        private static int DefaultBoxHeight(int t)
        {
            switch (t)
            {
                case 2: case 6: case 11: case 12: case 17: case 13: return 2;
                case 5: return 2;
                default: return 1;
            }
        }
    }

    /// <summary>Movel posicionado, com caixa e interacao ja resolvidas.</summary>
    public sealed class PlacedFurniture
    {
        public int Which;
        public Vector2 Tile;
        public FurnitureInfo Info;
        public Rectangle Bounds { get { return new Rectangle((int)Tile.X * 64, (int)Tile.Y * 64, Info.BoxW * 64, Info.BoxH * 64); } }

        public bool Blocks(Rectangle box)
        {
            if (Info == null) return false;
            if (Info.Interaction == FurnitureInteraction.Rug) return false;
            var full = Bounds;
            if (Info.Interaction != FurnitureInteraction.Bed)
                return full.Intersects(box);
            // BedFurniture.IntersectsForCollision: cabeca + pes bloqueiam, meio livre.
            var head = full; head.Height = 64;
            if (head.Intersects(box)) return true;
            var foot = full; foot.Y += 128; foot.Height -= 128;
            return foot.Intersects(box);
        }
    }

    /// <summary>
    /// Scanner de cenario: varre um mapa xTile e devolve tudo que pode ser
    /// interativo — moveis das listas, tiles com Passable/Action/WallID e
    /// portas. Base para adicionar qualquer casa sem codigo especifico.
    /// </summary>
    public static class MapScanner
    {
        public static List<PlacedFurniture> ScanFurniture(FurnitureCatalog catalog,
            IEnumerable<int> ids, IEnumerable<Vector2> tiles)
        {
            var result = new List<PlacedFurniture>();
            var idList = new List<int>(ids);
            var tileList = new List<Vector2>(tiles);
            for (var i = 0; i < idList.Count && i < tileList.Count; i++)
            {
                var info = catalog.Get(idList[i]);
                if (info == null) continue;
                result.Add(new PlacedFurniture { Which = idList[i], Tile = tileList[i], Info = info });
            }
            return result;
        }

        public static List<Vector2> ScanTiles(TileMap map, string property)
        {
            var result = new List<Vector2>();
            if (map == null || map.Layers.Count == 0) return result;
            for (var layerIndex = 0; layerIndex < map.Layers.Count; layerIndex++)
            {
                var layer = map.Layers[layerIndex];
                for (var y = 0; y < layer.Height; y++)
                for (var x = 0; x < layer.Width; x++)
                {
                    var tile = layer.Get(x, y);
                    if (tile != null && tile.Properties.ContainsKey(property))
                        result.Add(new Vector2(x, y));
                }
            }
            return result;
        }
    }
}
