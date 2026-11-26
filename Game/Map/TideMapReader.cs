using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace LumiaValley.Game.Map
{
    /// <summary>
    /// Le o formato binario TIDE (tBIN10) usado pelo xTile nos XNB da 1.5.6.
    /// Registrado com o nome exato do TideReader presente no Farm.xnb.
    /// </summary>
    public sealed class TideMapReader : ContentTypeReader<TileMap>
    {
        protected override TileMap Read(ContentReader input, TileMap existing)
        {
            var length = input.ReadInt32();
            var bytes = input.ReadBytes(length);
            return TbinParser.Parse(bytes);
        }
    }

    internal sealed class TbinParser
    {
        private readonly byte[] data;
        private int pos;

        private TbinParser(byte[] data)
        {
            this.data = data;
        }

        public static TileMap Parse(byte[] data)
        {
            var parser = new TbinParser(data);
            return parser.ParseMap();
        }

        private TileMap ParseMap()
        {
            var map = new TileMap();
            Sequence("tBIN10");
            map.Id = ReadString();
            map.Description = ReadString();
            ReadProperties(map.Properties);

            var sheetCount = ReadInt32();
            for (var i = 0; i < sheetCount; i++)
                map.TileSheets.Add(ReadTileSheet());

            var layerCount = ReadInt32();
            for (var i = 0; i < layerCount; i++)
                map.Layers.Add(ReadLayer(map));

            return map;
        }

        private void Sequence(string expected)
        {
            for (var i = 0; i < expected.Length; i++)
            {
                if (pos >= data.Length || data[pos++] != (byte)expected[i])
                    throw new InvalidDataException("Sequencia tBIN invalida.");
            }
        }

        private int ReadInt32()
        {
            var v = data[pos] | (data[pos + 1] << 8) | (data[pos + 2] << 16) | (data[pos + 3] << 24);
            pos += 4;
            return v;
        }

        private bool ReadBool()
        {
            return data[pos++] > 0;
        }

        private string ReadString()
        {
            var len = ReadInt32();
            var s = Encoding.UTF8.GetString(data, pos, len);
            pos += len;
            return s;
        }

        private void ReadProperties(IDictionary<string, string> target)
        {
            var count = ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var key = ReadString();
                var type = data[pos++];
                switch (type)
                {
                    case 0: target[key] = ReadBool() ? "true" : "false"; break;
                    case 1: target[key] = ReadInt32().ToString(); break;
                    case 2:
                    {
                        var bits = data[pos] | (data[pos + 1] << 8) |
                                   (data[pos + 2] << 16) | (data[pos + 3] << 24);
                        pos += 4;
                        target[key] = BitConverter.ToSingle(BitConverter.GetBytes(bits), 0).ToString();
                        break;
                    }
                    case 3: target[key] = ReadString(); break;
                }
            }
        }

        private TileSheetData ReadTileSheet()
        {
            var sheet = new TileSheetData();
            sheet.Id = ReadString();
            ReadString(); // descricao
            sheet.ImageSource = ReadString();
            sheet.SheetWidth = ReadInt32();
            sheet.SheetHeight = ReadInt32();
            sheet.TileWidth = ReadInt32();
            sheet.TileHeight = ReadInt32();
            sheet.MarginX = ReadInt32();
            sheet.MarginY = ReadInt32();
            sheet.SpacingX = ReadInt32();
            sheet.SpacingY = ReadInt32();
            ReadProperties(new Dictionary<string, string>());
            return sheet;
        }

        private TileLayer ReadLayer(TileMap map)
        {
            var layer = new TileLayer();
            layer.Id = ReadString();
            layer.Visible = ReadBool();
            ReadString(); // descricao
            layer.Width = ReadInt32();
            layer.Height = ReadInt32();
            // xTile Layer.zoom = 4: o tbin guarda 16x16, o mapa usa 64x64.
            layer.TileWidth = ReadInt32() * 4;
            layer.TileHeight = ReadInt32() * 4;
            ReadProperties(new Dictionary<string, string>());

            layer.Tiles = new TileData[layer.Width * layer.Height];
            TileSheetData current = null;
            var x = 0;
            for (var y = 0; y < layer.Height; y++)
            {
                x = 0;
                while (x < layer.Width)
                {
                    var code = (char)data[pos++];
                    switch (code)
                    {
                        case 'S':
                            layer.Tiles[y * layer.Width + x] = ReadStaticTile(current);
                            x++;
                            break;
                        case 'T':
                        {
                            var id = ReadString();
                            current = FindSheet(map, id);
                            break;
                        }
                        case 'N':
                            x += ReadInt32();
                            break;
                        case 'A':
                            layer.Tiles[y * layer.Width + x] = ReadAnimatedTile(map, current);
                            x++;
                            break;
                        default:
                            throw new InvalidDataException("Codigo de tile inesperado: " + code);
                    }
                }
            }
            return layer;
        }

        private TileData ReadStaticTile(TileSheetData sheet)
        {
            var tile = new TileData();
            tile.Sheet = sheet;
            tile.Index = ReadInt32();
            tile.Blend = data[pos++];
            ReadProperties(new Dictionary<string, string>());
            return tile;
        }

        private TileData ReadAnimatedTile(TileMap map, TileSheetData current)
        {
            ReadInt32(); // intervalo entre frames
            var frames = ReadInt32();
            TileData result = null;
            while (frames > 0)
            {
                var code = (char)data[pos++];
                if (code == 'T')
                {
                    current = FindSheet(map, ReadString());
                    continue;
                }
                if (code != 'S')
                    throw new InvalidDataException("Frame animado invalido.");
                var tile = ReadStaticTile(current);
                if (result == null)
                    result = tile;
                frames--;
            }
            ReadProperties(new Dictionary<string, string>());
            return result;
        }

        private static TileSheetData FindSheet(TileMap map, string id)
        {
            foreach (var sheet in map.TileSheets)
                if (sheet.Id == id)
                    return sheet;
            return null;
        }
    }
}
