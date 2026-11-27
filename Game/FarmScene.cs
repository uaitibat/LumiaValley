using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LumiaValley.Game.Map;

namespace LumiaValley.Game
{
    /// <summary>
    /// Cena jogavel da fazenda: mapa xTile real da 1.5.6, camera seguindo o
    /// jogador, D-pad virtual e HUD basico. Tudo em coordenadas virtuais.
    /// </summary>
    public sealed class FarmScene
    {
        // FarmerRenderer.cs:41-73 — deslocamento de cada feature por frame.
        private static readonly int[] FeatureY =
        {
            1, 2, 2, 0, 5, 6, 1, 2, 2, 1, 0, 2, 0, 1, 1, 0, 2, 2, 3, 3,
            2, 2, 1, 1, 0, 0, 2, 2, 4, 4, 0, 0, 1, 2, 1, 1, 1, 1, 0, 0,
            1, 1, 1, 0, 0, -2, -1, 1, 1, 0, -1, -2, -1, -1, 5, 4, 0, 0, 3, 2,
            -1, 0, 4, 2, 0, 0, 2, 1, 0, -1, 1, -2, 0, 0, 1, 1, 1, 1, 1, 1,
            0, 0, 0, 0, 1, -1, -1, -1, -1, 1, 1, 0, 0, 0, 0, 4, 1, 0, 1, 2,
            1, 0, 1, 0, 1, 2, -3, -4, -1, 0, 0, 2, 1, -4, -1, 0, 0, -3, 0, 0,
            -1, 0, 0, 2, 1, 1
        };

        private static readonly int[] FeatureX =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 4, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0,
            0, 0, 0, 0, 0, 0
        };

        private Texture2D pixel;
        private Texture2D cursors;
        private Texture2D shadow;
        private Texture2D windowLight;
        private Texture2D farmerBase;
        private Texture2D hairSheet;
        private Texture2D shirtSheet;
        private Texture2D pantSheet;
        private Texture2D accessorySheet;
        private Texture2D skinSheet;
        private Texture2D shoeSheet;
        private SpriteFont font;
        private readonly DayTimeMoneyBoxPort clock = new DayTimeMoneyBoxPort();
        private readonly ToolbarPort toolbar = new ToolbarPort();
        private int selectedSlot;
        private TileMap map;
        private readonly Dictionary<string, Texture2D> sheets = new Dictionary<string, Texture2D>();
        private readonly FurnitureRenderer furniture = new FurnitureRenderer();

        // Moveis iniciais (Game1.loadForNewGame). O indice 0=e' o da cadeira.
        private static readonly int[] FurnitureIds =
            { 2048, 1120, 1376, 0, 1466, 1614, 1618, 1602, 1792 };
        private static readonly int[] FurnitureX = { 9, 5, 1, 4, 1, 3, 6, 5, 8 };
        private static readonly int[] FurnitureY = { 8, 4, 10, 4, 4, 1, 8, 1, 4 };

        private Vector2 player;
        private Vector2 target;
        private bool hasTarget;
        private Vector2 direction;
        private Vector2 camera;

        private readonly SaveSlot slot;
        private double elapsedMs;
        private int gameMinutes = 360;
        private int energy = 270;
        private int facing = 2;
        private int walkFrame;
        private double walkMs;
        private bool fireplaceOn; // FarmHouse.fireplaceOn (default false)
        private bool giftboxOpened;
        private bool isInBed;
        // GameLocation.UpdateWhenCurrentLocation: lastTouchActionLocation e o
        // "switch" que dispara o TouchAction uma vez por entrada no tile.
        private Vector2 lastTouchActionLocation = new Vector2(-1, -1);
        // Farmer.behaviorOnMovement: so dispara TouchAction se o jogador andou.
        private bool playerHasMoved;
        // Dialogo simples (prompt de cama / menu da TV / baú / porta).
        private string dialogText;
        private string[] dialogOptions;
        private Action<int> dialogAction;
        private Rectangle[] dialogRects;

        public FarmScene(SaveSlot slot = null)
        {
            this.slot = slot;
        }

        public void Load(ContentManager content, GraphicsDevice device)
        {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });

            try { font = content.Load<SpriteFont>("Fonts/SmallFont.pt-BR"); }
            catch { font = null; }
            try { clock.Load(content); toolbar.Load(content); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("HUD: " + ex.Message); }
            try { cursors = content.Load<Texture2D>("LooseSprites/Cursors.pt-BR"); }
            catch { cursors = null; }
            try { shadow = content.Load<Texture2D>("LooseSprites/shadow"); }
            catch { shadow = null; }
            try { windowLight = content.Load<Texture2D>("LooseSprites/Lighting/indoorWindowLight"); }
            catch { windowLight = null; }
            try { furniture.Load(content); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Furniture: " + ex.Message); }

            try
            {
                farmerBase = content.Load<Texture2D>("Characters/Farmer/farmer_base");
                hairSheet = content.Load<Texture2D>("Characters/Farmer/hairstyles");
                shirtSheet = content.Load<Texture2D>("Characters/Farmer/shirts");
                pantSheet = content.Load<Texture2D>("Characters/Farmer/pants");
                accessorySheet = content.Load<Texture2D>("Characters/Farmer/accessories");
                skinSheet = content.Load<Texture2D>("Characters/Farmer/skinColors");
                shoeSheet = content.Load<Texture2D>("Characters/Farmer/shoeColors");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sprites do fazendeiro: " + ex.Message);
            }

            try { map = content.Load<TileMap>("Maps/FarmHouse"); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao carregar Maps/FarmHouse: " + ex.Message);
                map = null;
            }

            if (map != null)
                ApplyDefaultDecorations();

            if (map != null)
            {
                foreach (var sheet in map.TileSheets)
                {
                    if (string.IsNullOrEmpty(sheet.ImageSource))
                        continue;
                    var path = "Maps/" + sheet.ImageSource;
                    try
                    {
                        sheets[sheet.Id] = content.Load<Texture2D>(path + ".pt-BR");
                    }
                    catch
                    {
                        try { sheets[sheet.Id] = content.Load<Texture2D>(path); }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Tilesheet " + path + ": " + ex.Message);
                        }
                    }
                }
            }

            Reset();
        }

        /// <summary>
        /// Porte do DecoratableLocation.ReadWallpaperAndFloorTileData +
        /// UpdateWallpaper/UpdateFloor com o papel/piso padrao (0), que o
        /// original aplica ao entrar na casa pela primeira vez.
        /// </summary>
        private void ApplyDefaultDecorations()
        {
            var back = map.Layers.Count > 0 ? map.Layers[0] : null;
            var buildings = map.Layers.Count > 1 ? map.Layers[1] : null;
            if (back == null)
                return;

            const int sheetWidth = 16; // walls_and_floors

            for (var y = 0; y < back.Height; y++)
            {
                for (var x = 0; x < back.Width; x++)
                {
                    var tile = back.Get(x, y);
                    if (tile == null || !tile.Properties.ContainsKey("WallID"))
                        continue;

                    // Padrao 0: value = indice na tilesheet; o original faz
                    // value/sheetWidth*sheetWidth*3 + value%sheetWidth + linha.
                    var value = 0;
                    for (var row = 0; row < 3; row++)
                    {
                        var layer = row == 2 && buildings != null ? buildings : back;
                        var ty = y + row;
                        var target = layer.Get(x, ty);
                        if (target == null || target.Sheet == null ||
                            target.Sheet.Id != "walls_and_floors")
                            continue;
                        target.Index = value / sheetWidth * sheetWidth * 3 + value % sheetWidth + row * sheetWidth;
                    }
                }
            }

            // ReadWallpaperAndFloorTileData chama setFloors()/setWallpapers()
            // no construtor: appliedFloor/appliedWallpaper = "0". O piso
            // tambem e aplicado (DecoratableLocation.cs:273-274, 365-408).
            const int firstFlooringTile = 336; // GetFirstFlooringTile()
            for (var y = 0; y < back.Height; y++)
            {
                for (var x = 0; x < back.Width; x++)
                {
                    var tile = back.Get(x, y);
                    if (tile == null || !tile.Properties.ContainsKey("FloorID"))
                        continue;
                    if (tile.Sheet == null || !IsWallAndFloorSheet(tile.Sheet.Id))
                        continue;

                    var num = tile.Index;
                    var sw = tile.Sheet.SheetWidth;
                    if (tile.Sheet.Id == "walls_and_floors")
                        num -= firstFlooringTile;
                    var idx = num % 2 + sw * (num % (sw * 2) / sw);

                    // UpdateFloor: value=0 -> 0*2 + 0/(sw/2)*sw = 0;
                    // walls_and_floors -> +GetFirstFlooringTile() = 336.
                    var targetTile = back.Get(x, y);
                    if (targetTile != null)
                        targetTile.Index = firstFlooringTile + idx;
                }
            }
        }

        private static bool IsWallAndFloorSheet(string id)
        {
            return id == "walls_and_floors" || id.StartsWith("x_WallsAndFloors_");
        }

        public void Reset()
        {
            // Original: TitleMenu.createdNewCharacter seta (9,9)*64 com
            // isInBed=true; NewDay -> BedFurniture.ApplyWakeUpPosition ->
            // GetPlayerBedSpot (10,9), ShiftPositionForBed (-64 X se o tile a
            // esquerda da cama estiver livre) e +32 em Y. Resultado tipico:
            // (576, 608), virado para a direita (facing 1).
            player = new Vector2(9 * 64f, 9 * 64f + 32f);
            target = player;
            hasTarget = false;
            facing = 1;
            direction = Vector2.Zero;

            // GameLocation.resetForPlayerEntry: o TemporaryPassableTiles da
            // cama e equivalente a regra 1:1 usada em IsBlocked (se o jogador
            // ja intersecta a cama, ela nao bloqueia). Nada a inicializar.
            lastTouchActionLocation = new Vector2(-1, -1);
            playerHasMoved = false;

            UpdateCamera();
        }

        /// <summary>Transformacao aplicada pelo FarmScreen antes do desenho.</summary>
        public Matrix Transform
        {
            get { return Matrix.CreateTranslation(-camera.X, -camera.Y, 0f); }
        }

        public bool HasDialog { get { return dialogText != null; } }

        public void OnTap(Vector2 screenPoint)
        {
            var world = screenPoint + camera;
            var tx = (int)(world.X / 64f);
            var ty = (int)(world.Y / 64f);

            // Ordem do original: Buildings.Action -> TouchAction -> moveis.
            if (TryTileAction(tx, ty))
                return;
            if (TryFurnitureAction(tx, ty))
                return;

            // Sem D-pad: apenas toque no mundo, como no original em desktop.
            target = world;
            hasTarget = true;
        }

        /// <summary>Trata o toque numa opcao do dialogo (ponto virtual).</summary>
        public void HandleDialogTap(Vector2 point)
        {
            if (dialogRects == null)
                return;
            for (var i = 0; i < dialogRects.Length; i++)
            {
                if (dialogRects[i].Contains(point))
                {
                    var action = dialogAction;
                    dialogText = null;
                    dialogOptions = null;
                    dialogRects = null;
                    if (action != null) action(i);
                    return;
                }
            }
            // Toque fora fecha o dialogo sem acao.
            dialogText = null;
            dialogOptions = null;
            dialogRects = null;
        }

        private void ShowDialog(string text, string[] options, Action<int> action)
        {
            dialogText = text;
            dialogOptions = options;
            dialogAction = action;
            hasTarget = false;
            walkFrame = 0;
            walkMs = 0;
        }

        private bool TryTileAction(int tx, int ty)
        {
            if (map == null || map.Layers.Count < 2)
                return false;
            var buildings = map.Layers[1];
            var tile = buildings.Get(tx, ty);
            if (tile == null)
                return false;
            string action;
            if (!tile.Properties.TryGetValue("Action", out action))
                return false;
            if (!action.StartsWith("Warp"))
                return false;
            // GameLocation.performAction "Warp": troca de mapa. Ate a cena da
            // fazenda existir, mostramos a acao resolvida em vez de engasgar.
            var parts = action.Split(' ');
            var destino = parts.Length > 1 ? parts[1] : "Farm";
            ShowDialog("Ir para " + destino + "?", new[] { "Agora", "Depois" },
                i => { if (i == 0) { /* troca de cena futura */ } });
            return true;
        }

        private bool TryFurnitureAction(int tx, int ty)
        {
            // Cama: BedFurniture.DoesTileHaveProperty injeta TouchAction="Sleep"
            // no tile (bed.X + bedTileOffset, bed.Y + 1). O offset e 0 se o
            // jogador estiver a direita do centro da cama, 1 se a esquerda.
            if (ty == 9 && (tx == 9 || tx == 10))
            {
                var dormir = Localization.Location("FarmHouse_Bed_GoToSleep", "Dormir agora?");
                ShowDialog(dormir, new[] { "Sim", "Nao" }, i =>
                {
                    if (i != 0) return;
                    // Game1.NewDay observavel: hora 6:00, energia cheia.
                    gameMinutes = 360;
                    energy = 270;
                    hasTarget = false;
                    player = new Vector2(9 * 64f, 9 * 64f + 32f);
                    facing = 1;
                    UpdateCamera();
                });
                return true;
            }

            // TV: Furniture.checkForAction (TV.cs:41-75).
            if (tx == 1 && ty == 4)
            {
                ShowDialog(Localization.CS("TV.cs.13120", "O que voc\\u00ea quer assistir?"),
                    new[] { Localization.CS("TV.cs.13105", "Previs\\u00e3o do tempo"),
                            Localization.CS("TV.cs.13107", "Hor\\u00f3scopo"),
                            Localization.CS("TV.cs.13118", "Sair") },
                    i => { });
                return true;
            }

            // Lareira: FarmHouse.checkAction alterna fireplaceOn (tiles 794-797).
            if (tx == 8 && ty == 4)
            {
                fireplaceOn = !fireplaceOn;
                return true;
            }

            // Baú presente: Chest.dumpContents (3,7).
            if (tx == 3 && ty == 7 && !giftboxOpened)
            {
                ShowDialog("Você recebeu 15 sementes!", new[] { "OK" }, i =>
                {
                    giftboxOpened = true;
                });
                return true;
            }
            return false;
        }

        public void ReleaseDirection()
        {
            direction = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, 0.05);
            elapsedMs += dt * 1000;
            gameMinutes += (int)(dt * 12); // um dia de 20 minutos

            const float speed = 192f;
            if (hasTarget)
            {
                var delta = target - player;
                var distance = delta.Length();
                var step = speed * dt;
                if (distance <= step)
                {
                    MovePlayer(delta);
                    hasTarget = false;
                }
                else
                {
                    var moved = MovePlayer(delta / distance * step);
                    if (!moved)
                    {
                        // Bateu em colisao: cancela o alvo e a animacao, em vez
                        // de ficar "andando" parado contra a parede/movel.
                        hasTarget = false;
                        walkFrame = 0;
                        walkMs = 0;
                    }
                    else
                    {
                        // Farmer.updateMovementAnimation: direcao pelo movimento.
                        facing = Math.Abs(delta.X) > Math.Abs(delta.Y)
                            ? (delta.X > 0 ? 1 : 3)
                            : (delta.Y > 0 ? 2 : 0);

                        // FarmerSprite.getAnimationFromIndex: 4 frames de 200ms.
                        walkMs += dt * 1000;
                        if (walkMs >= 200)
                        {
                            walkMs -= 200;
                            walkFrame = (walkFrame + 1) % 4;
                        }
                    }
                }
            }
            else
            {
                walkFrame = 0;
                walkMs = 0;
            }

            farmerSprite.Update(dt * 1000);
            CheckBedState();

            ClampPlayer();
            UpdateCamera();
        }

        /// <summary>
        /// Farmer.Update: isInBed vem da propriedade "Bed" do tile pisado
        /// (Farmer.cs:7162). BedFurniture.DoesTileHaveProperty injeta
        /// TouchAction="Sleep" no tile lateral e o prompt dispara ao pisar.
        /// </summary>
        private void CheckBedState()
        {
            // Farmer.cs:7162 -> isInBed = currentLocation.doesTileHaveProperty
            // (getTileLocation().X, getTileLocation().Y, "Bed", "Back") != null.
            // BedFurniture.DoesTileHaveProperty devolve "Bed" para os tiles
            // (bed.X .. bed.X + getTilesWide()-1, bed.Y + 1) = (9..10, 9).
            // Usamos o TILE (nao a bounding box) para nao oscilar com o
            // arredondamento do movimento.
            // Character.cs:1177-1187 -> getStandingX/Y = centro da boundingBox.
            // PlayerBounds e (X-24, Y-16, 48, 32), logo standing = (X, Y).
            var tx = (int)(player.X / 64f);
            var ty = (int)(player.Y / 64f);
            var bedBox = furniture.GetBoundingBox(2048, 9, 8);
            isInBed = bedBox.Intersects(PlayerBounds(player)) ||
                      (ty == 9 && (tx == 9 || tx == 10));

            // GameLocation.cs:2818-2828 — o switch do TouchAction.
            // BedFurniture.DoesTileHaveProperty: TouchAction="Sleep" no tile
            // (bed.X + bedTileOffset, bed.Y+1). bedTileOffset = 0 se o jogador
            // esta a direita do centro da cama, 1 se a esquerda.
            var standing = new Vector2(tx, ty);
            var bed = bedBox;
            var offset = player.X > bed.Center.X ? 0 : 1;
            var touchTile = new Vector2(9 + offset, 9);

            if (lastTouchActionLocation.Equals(new Vector2(-1, -1)) ||
                lastTouchActionLocation.Equals(standing))
            {
                if (standing.Equals(touchTile) && playerHasMoved && !HasDialog)
                {
                    lastTouchActionLocation = standing;
                    var dormir = Localization.Location("FarmHouse_Bed_GoToSleep", "Dormir agora?");
                    ShowTouchDialog(dormir, new[] { "Sim", "Nao" }, i =>
                    {
                        if (i != 0) return;
                        gameMinutes = 360;
                        energy = 270;
                        player = new Vector2(9 * 64f, 9 * 64f + 32f);
                        facing = 1;
                        UpdateCamera();
                    });
                }
            }
            else
            {
                lastTouchActionLocation = new Vector2(-1, -1);
            }
        }

        /// <summary>
        /// TouchAction nao interrompe o movimento (ao contrario de ShowDialog,
        /// usado pelos cliques). Game1.player.Halt() so ocorre em respostas
        /// especificas, nao no prompt de dormir.
        /// </summary>
        private void ShowTouchDialog(string text, string[] options, Action<int> action)
        {
            dialogText = text;
            dialogOptions = options;
            dialogAction = action;
        }

        /// <summary>
        /// Caixa de colisao do Farmer (GetBoundingBox): 48x32 centrada no pe,
        /// 8px para a direita como no original (posicao e o canto do sprite).
        /// </summary>
        private Rectangle PlayerBounds(Vector2 pos)
        {
            return new Rectangle((int)pos.X - 24, (int)pos.Y - 16, 48, 32);
        }

        /// <summary>
        /// Porte de GameLocation.isPointPassable/isCollidingPosition:
        /// - Back com propriedade "Passable" (os rodapes/paredes) bloqueia;
        /// - Buildings com tile e sem "Shadow" bloqueia;
        /// - moveis com boundingBox (exceto tapetes) bloqueiam.
        /// </summary>
        private bool IsBlocked(Vector2 pos)
        {
            if (map == null || map.Layers.Count == 0)
                return false;
            var box = PlayerBounds(pos);
            var back = map.Layers[0];
            var buildings = map.Layers.Count > 1 ? map.Layers[1] : null;

            int[] xs = { box.Left, box.Right - 1, box.Center.X };
            int[] ys = { box.Top, box.Bottom - 1 };
            foreach (var x in xs)
            {
                foreach (var y in ys)
                {
                    var tx = x / 64;
                    var ty = y / 64;
                    var t = back.Get(tx, ty);
                    if (t != null && t.Properties.ContainsKey("Passable"))
                        return true;
                    var b = buildings == null ? null : buildings.Get(tx, ty);
                    if (b != null && !b.Properties.ContainsKey("Shadow")
                        && !b.Properties.ContainsKey("Passable"))
                        return true;
                }
            }

            for (var i = 0; i < FurnitureIds.Length; i++)
            {
                if (furniture.IsPassable(FurnitureIds[i]))
                    continue;
                // BedFurniture.IntersectsForCollision: cabeca (64px) e pes
                // (Y+128) bloqueiam; o meio e livre. Ao entrar no save o
                // jogador esta dentro da cama, entao o original adiciona a
                // bounding box dela em TemporaryPassableTiles ate ele sair.
                // GameLocation.cs:1652-1657 (1:1): se o jogador JA intersecta
                // a cama, ela nao bloqueia em nenhuma direcao. E isso que
                // permite levantar e sair de dentro dela ao acordar.
                if (FurnitureIds[i] == 2048 &&
                    furniture.IntersectsForCollision(2048, 9, 8, PlayerBounds(player)))
                    continue;

                var fb = furniture.GetBoundingBox(FurnitureIds[i], FurnitureX[i], FurnitureY[i]);
                if (fb == Rectangle.Empty ||
                    !furniture.IntersectsForCollision(FurnitureIds[i], FurnitureX[i], FurnitureY[i], box))
                    continue;
                // Original: se o jogador ja esta dentro do movel, ele nao
                // bloqueia (!item.IntersectsForCollision(player.GetBoundingBox)).
                if (furniture.IsPassable(FurnitureIds[i]) ||
                    furniture.IsInsidePassableBand(FurnitureIds[i], FurnitureX[i], FurnitureY[i],
                        PlayerBounds(player)))
                    continue;
                return true;
            }

            // Bau de sementes (giftbox) e um Object: isCollidingPosition tambem
            // testa objects[^].getBoundingBox() e nao e passavel.
            if (!giftboxOpened)
            {
                var chest = new Rectangle(3 * 64, 7 * 64, 64, 64);
                if (chest.Intersects(box) && !chest.Intersects(PlayerBounds(player)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Movimento por eixos: se um eixo colide, mantem o outro.
        /// Retorna false se nenhum eixo andou (usado para cancelar o toque).
        /// </summary>
        private bool MovePlayer(Vector2 delta)
        {
            var before = player;
            var tryX = new Vector2(player.X + delta.X, player.Y);
            if (!IsBlocked(tryX))
                player = tryX;
            var tryY = new Vector2(player.X, player.Y + delta.Y);
            if (!IsBlocked(tryY))
                player = tryY;
            if (player != before)
                playerHasMoved = true; // Farmer.behaviorOnMovement()
            return player != before;
        }

        private void ClampPlayer()
        {
            if (map == null)
                return;
            var maxX = map.Width * 64f;
            var maxY = map.Height * 64f;
            player.X = MathHelper.Clamp(player.X, 32f, maxX - 32f);
            player.Y = MathHelper.Clamp(player.Y, 48f, maxY - 16f);
        }

        private void UpdateCamera()
        {
            var worldWidth = map != null ? map.Width * 64 : ViewportConfig.VirtualWidth;
            var worldHeight = map != null ? map.Height * 64 : ViewportConfig.VirtualHeight;

            // Mapas menores que a viewport ficam centralizados, como no original.
            camera.X = worldWidth <= ViewportConfig.VirtualWidth
                ? -(ViewportConfig.VirtualWidth - worldWidth) / 2f
                : MathHelper.Clamp(player.X - ViewportConfig.VirtualWidth / 2f,
                    0f, worldWidth - ViewportConfig.VirtualWidth);
            camera.Y = worldHeight <= ViewportConfig.VirtualHeight
                ? -(ViewportConfig.VirtualHeight - worldHeight) / 2f
                : MathHelper.Clamp(player.Y - ViewportConfig.VirtualHeight / 2f,
                    0f, worldHeight - ViewportConfig.VirtualHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Ordem do Game1.draw: fundo -> objetos/jogador -> Front/AlwaysFront.
            DrawMap(spriteBatch, false);
            DrawLights(spriteBatch);
            // BedFurniture.draw desenha a frente do lençol com depth maior que
            // o do jogador: se ele esta na cama, a frente vai DEPOIS dele.
            DrawFurniture(spriteBatch);
            DrawFarmer(spriteBatch);
            if (isInBed)
                furniture.DrawBedFront(spriteBatch, 2048, 9, 8);
            DrawMap(spriteBatch, true);
            DrawHud(spriteBatch);
            DrawDialog(spriteBatch);
        }

        /// <summary>
        /// Caixa de dialogo em coordenadas de tela (compensa a camera como o HUD).
        /// </summary>
        private void DrawDialog(SpriteBatch batch)
        {
            if (dialogText == null || font == null)
                return;
            var ox = (int)camera.X;
            var oy = (int)camera.Y;
            var x = 96 + ox;
            var y = ViewportConfig.VirtualHeight - 208 + oy;
            var w = 448;
            var h = 176;
            batch.Draw(pixel, new Rectangle(x, y, w, h), new Color(18, 16, 28) * 0.96f);
            batch.Draw(pixel, new Rectangle(x + 4, y + 4, w - 8, 2), new Color(220, 196, 128));
            batch.DrawString(font, dialogText, new Vector2(x + 16, y + 16), Color.White);

            dialogRects = new Rectangle[dialogOptions.Length];
            for (var i = 0; i < dialogOptions.Length; i++)
            {
                var r = new Rectangle(x + 16, y + 56 + i * 32, w - 32, 28);
                dialogRects[i] = r;
                batch.Draw(pixel, r, i == 0 ? new Color(58, 92, 52) : new Color(36, 32, 48));
                batch.DrawString(font, dialogOptions[i], new Vector2(r.X + 8, r.Y + 6), Color.White);
            }
        }

        private void DrawMap(SpriteBatch batch, bool front)
        {
            if (map == null)
            {
                if (!front)
                    batch.Draw(pixel, new Rectangle(0, 0, ViewportConfig.VirtualWidth,
                        ViewportConfig.VirtualHeight), new Color(74, 132, 62));
                return;
            }

            foreach (var layer in map.Layers)
            {
                if (layer == null || !layer.Visible || layer.Tiles == null)
                    continue;

                // Back/Buildings antes dos objetos; Front/AlwaysFront depois.
                var isFront = layer.Id == "Front" || layer.Id == "AlwaysFront";
                if (isFront != front)
                    continue;

                for (var i = 0; i < layer.Tiles.Length; i++)
                {
                    var tile = layer.Tiles[i];
                    if (tile == null || tile.Sheet == null)
                        continue;
                    Texture2D texture;
                    if (!sheets.TryGetValue(tile.Sheet.Id, out texture) || texture == null)
                        continue;

                    var col = tile.Index % tile.Sheet.SheetWidth;
                    var row = tile.Index / tile.Sheet.SheetWidth;
                    var src = new Rectangle(
                        col * tile.Sheet.TileWidth + tile.Sheet.MarginX,
                        row * tile.Sheet.TileHeight + tile.Sheet.MarginY,
                        tile.Sheet.TileWidth, tile.Sheet.TileHeight);
                    if (src.X < 0 || src.Y < 0 || src.Right > texture.Width || src.Bottom > texture.Height)
                        continue;

                    var x = i % layer.Width * 64;
                    var y = i / layer.Width * 64;
                    batch.Draw(texture, new Rectangle(x, y, 64, 64), src, Color.White);
                }
            }
        }

        /// <summary>Luz da janela: Game1.drawLighting com indoorWindowLight.</summary>
        private void DrawLights(SpriteBatch batch)
        {
            if (windowLight == null)
                return;

            // Game1.drawLighting usa BlendState.Additive para as luzes.
            batch.End();
            batch.Begin(blendState: BlendState.Additive,
                samplerState: SamplerState.PointClamp,
                transformMatrix: Game1.SceneTransform);

            // Janela em (3,1): centro do tile, escala do radius do LightSource.
            var center = new Vector2(3 * 64f + 32f, 1 * 64f + 32f);
            batch.Draw(windowLight, center, windowLight.Bounds,
                new Color(255, 244, 190) * 0.28f, 0f,
                new Vector2(windowLight.Width / 2f, windowLight.Height / 2f),
                1.1f, SpriteEffects.None, 0f);

            // Lareira acesa: GameLocation.setFireplace cria duas luzes
            // (tileX*1000 + tileY) com raio 2, sprite 12x11 de Cursors.
            if (fireplaceOn && cursors != null)
            {
                var fp = new Vector2(8 * 64f + 32f, 4 * 64f - 32f);
                batch.Draw(cursors, fp, new Rectangle(276, 1985, 12, 11),
                    new Color(255, 214, 150) * 0.35f, 0f,
                    new Vector2(cursors.Bounds.Center.X, cursors.Bounds.Center.Y),
                    2f, SpriteEffects.None, 0f);
            }

            batch.End();
            batch.Begin(samplerState: SamplerState.PointClamp,
                transformMatrix: Game1.SceneTransform);
        }

        /// <summary>
        /// Moveis que no xTile vem de tiles com a sheet errada no nosso
        /// parser. Usamos os indices corretos do townInterior (cama) para
        /// sobrepor as brasoes do walls_and_floors.
        /// </summary>
        private void DrawFurniture(SpriteBatch batch, bool skipBed = false)
        {
            // FarmHouse.doAction / Game1.loadForNewGame: moveis iniciais do
            // casal (whichFarm = 0), exatamente como no original.
            // Lista exata de Game1.loadForNewGame / FarmHouse (whichFarm=0).
            var time = elapsedMs;
            if (!skipBed)
                furniture.Draw(batch, 2048, 9, 8);   // cama
            furniture.Draw(batch, 1120, 5, 4, heldObject: 1364); // mesa + tigela
            furniture.Draw(batch, 1376, 1, 10);  // planta
            furniture.Draw(batch, 0, 4, 4);      // cadeira
            furniture.Draw(batch, 1466, 1, 4);   // TV
            furniture.Draw(batch, 1614, 3, 1);   // janela
            furniture.Draw(batch, 1618, 6, 8);   // tapete
            furniture.Draw(batch, 1602, 5, 1);   // quadro
            furniture.Draw(batch, 1792, 8, 4,
                isOn: fireplaceOn, timeMs: time); // lareira (FarmHouse.fireplaceOn)
            if (!giftboxOpened)
                furniture.DrawGiftbox(batch, 3, 7);  // bau-presente com 15 parafusos
        }

        /// <summary>Mesmas camadas do preview da criacao: corpo, calca, camisa,
        /// cabelo, braco e acessorio. Escala 4 = 64x128 no espaco do mundo.
        /// </summary>
        private readonly FarmerSpritePort farmerSprite = new FarmerSpritePort();
        private FarmerRendererPort farmerRenderer;
        private int lastFacing = -1;
        private bool lastWalking;

        private void DrawFarmer(SpriteBatch batch)
        {
            if (farmerBase == null)
                return;

            if (farmerRenderer == null)
                farmerRenderer = new FarmerRendererPort(farmerBase, hairSheet, shirtSheet,
                    pantSheet, accessorySheet, slot ?? new SaveSlot(),
                    skinSheet, shoeSheet, shirtSheet);

            // Farmer.Position e o TOPO da caixa de colisao (pes - 32). O sprite
            // e desenhado com origin.Y = 32, entao a sombra fica 8px acima dos pes.
            if (shadow != null)
                batch.Draw(shadow, new Vector2(player.X, player.Y - 8f),
                    shadow.Bounds, Color.White, 0f,
                    new Vector2(shadow.Bounds.Center.X, shadow.Bounds.Center.Y),
                    4f, SpriteEffects.None, player.Y / 10000f - 0.000001f);

            var walking = hasTarget;
            if (facing != lastFacing || walking != lastWalking)
            {
                farmerSprite.SetDirection(facing, walking);
                lastFacing = facing;
                lastWalking = walking;
            }

            // Pele: trio sombra/base/luz do skinColors, indice do save.
            var skin = new Color(224, 172, 140);
            if (skinSheet != null && slot != null)
            {
                var palette = new Color[skinSheet.Width * skinSheet.Height];
                skinSheet.GetData(palette);
                var which = Wrap(slot.Skin, skinSheet.Height);
                skin = palette[which * 3 + 1];
            }

            // Sprite de 128px com pes no ponto do jogador.
            var pos = new Vector2((float)Math.Floor(player.X - 32), (float)Math.Floor(player.Y - 128));
            farmerRenderer.Draw(batch, farmerSprite, pos, facing, skin);
        }

        private static int Wrap(int value, int count)
        {
            if (count <= 0)
                return 0;
            value %= count;
            return value < 0 ? value + count : value;
        }

        private static void DrawLayer(SpriteBatch batch, Texture2D texture, Vector2 pos,
            Rectangle source, Color color, SpriteEffects effects = SpriteEffects.None)
        {
            if (texture == null || source.X < 0 || source.Y < 0 ||
                source.Right > texture.Width || source.Bottom > texture.Height)
                return;
            batch.Draw(texture, pos, source, color, 0f, Vector2.Zero, 4f, effects, 0f);
        }

        private void DrawHud(SpriteBatch batch)
        {
            // HUD usa coordenadas de tela: somamos a camera para compensar
            // a transformacao do mundo aplicada pelo FarmScreen.
            var ox = (int)camera.X;
            var oy = (int)camera.Y;

            // Porte literal de Game1.drawHUD (linhas 18155-18175 da 1.5.6).
            const float staminaScale = 0.625f;
            const int maxStamina = 270;
            var bx = ViewportConfig.VirtualWidth - 48 - 8 + ox;
            var by = ViewportConfig.VirtualHeight - 224 - 16
                - (int)((maxStamina - 270) * staminaScale) + oy;

            if (cursors != null)
            {
                // Topo, meio e base da moldura (o meio ajusta a altura da viewport).
                batch.Draw(cursors, new Vector2(bx, by), new Rectangle(256, 408, 12, 16),
                    Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                var middleHeight = ViewportConfig.VirtualHeight - 64 - 16 - (int)(by - oy + 64 - 8);
                batch.Draw(cursors, new Rectangle((int)bx, (int)by + 64, 48, middleHeight),
                    new Rectangle(256, 424, 12, 16), Color.White);
                batch.Draw(cursors, new Vector2(bx, by + 224 - 64),
                    new Rectangle(256, 448, 12, 16), Color.White, 0f, Vector2.Zero, 4f,
                    SpriteEffects.None, 1f);

                // Preenchimento: Y = by + 16 + 32 + (maxStamina - stamina)*0.625.
                var fillY = by + 16 + 32 + (int)((maxStamina - energy) * staminaScale);
                var fillHeight = (int)(energy * staminaScale);
                var fillRect = new Rectangle((int)bx + 12, (int)fillY, 24, fillHeight);
                var fillColor = new Color(104, 184, 72);
                batch.Draw(pixel, fillRect, fillColor);

                // Faixa escura de 4px no topo do preenchimento.
                var dark = new Color(
                    Math.Max(0, fillColor.R - 50),
                    Math.Max(0, fillColor.G - 50), fillColor.B);
                batch.Draw(pixel, new Rectangle(fillRect.X, fillRect.Y, 24, 4), dark);
            }

            if (font == null)
                return;

            // DayTimeMoneyBox: relogio/data/clima/dinheiro 1:1.
            // O original nao desenha hora/dinheiro no drawHUD - delega ao menu.
            var timeOfDay = (6 + gameMinutes / 60) * 100 + gameMinutes % 60;
            clock.Draw(batch, ViewportConfig.VirtualWidth, timeOfDay, 1,
                "spring", 0, 500, (int)elapsedMs % 7000, ox, oy);

            // Toolbar com os 5 itens iniciais de Farmer.initialTools().
            toolbar.Draw(batch, ViewportConfig.VirtualWidth,
                ViewportConfig.VirtualHeight, selectedSlot, ox, oy);

            // drawWithBorder: a esquerda da barra, com Y = by + 64.
            var staminaText = energy + "/" + maxStamina;
            var labelSize = font.MeasureString("999/999");
            batch.DrawString(font, staminaText,
                new Vector2(bx - labelSize.X - 16, by + 64), Color.White);




        }

    }
}
