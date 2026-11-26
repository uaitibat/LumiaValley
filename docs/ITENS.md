# Catalogação de itens — Stardew Valley 1.5.6

Revista do código descompilado em `reference/` feita para guiar o port UWP.
Nenhum código descompilado é copiado para o repo público — aqui estão apenas
referências e formatos observados.

## 1. Hierarquia

```
Item (abstract)                 Item.cs:19
├── Tool (abstract)             Tool.cs:26
│   ├── Stackable               Tools/Stackable.cs
│   │   └── Seeds               Tools/Seeds.cs
│   ├── Axe, Hoe, Pickaxe, WateringCan, MilkPail, Shears, Pan, Slingshot,
│   │   FishingRod, Wand, GenericTool, Blueprints, Lantern, MagnifyingGlass
│   └── MeleeWeapon             Tools/MeleeWeapon.cs
├── Object                      Object.cs:45
│   ├── Furniture               Objects/Furniture.cs
│   │   ├── BedFurniture, TV, StorageFurniture
│   │   │   └── FishTankFurniture
│   ├── Chest, Cask, ColoredObject, CrabPot, Fence, IndoorPot,
│   ├── ItemPedestal, MiniJukebox, Phone, Sign, SwitchFloor,
│   ├── Torch, Wallpaper, WoodChipper, Workbench, BreakableContainer
├── Boots, Clothing, Hat, Ring, SpecialItem, CombinedRing
```

`Furniture.GetFurnitureInstance` decide a subclasse: ID 1466/1468/1680/2326 →
`TV`; campo[1] = `fishtank` → `FishTankFurniture`; nome começa com `bed` →
`BedFurniture`; `dresser` → `StorageFurniture`; resto → `Furniture`.

## 2. Texturas por tipo

| Classe | XNB |
|---|---|
| Objeto comum | `Maps/springobjects` |
| Big craftable | `TileSheets/Craftables` |
| Mobília | `TileSheets/furniture`, `TileSheets/furnitureFront` |
| Ferramentas | `TileSheets/tools` |
| Armas | `TileSheets/weapons` |
| Chapéu | `Characters/Farmer/hats` |
| Camisa/calça (item e jogador) | `Characters/Farmer/shirts`, `pants` |
| Cabelo | `Characters/Farmer/hairstyles`, `hairstyles2` |
| Sapato (cor no jogador) | `Characters/Farmer/shoeColors` |
| Pele | `Characters/Farmer/skinColors` |
| Corpo | `farmer_base`, `farmer_girl_base` |
| Acessório | `Characters/Farmer/accessories` |
| Papel de parede/piso | `Maps/walls_and_floors` |

## 3. Data/*.xnb (campos separados por `/`)

- `ObjectInformation`: nome, preço, comestibilidade, tipo+categoria, nome, descrição, comida/bebida, buffs.
- `Furniture`: nome, tipo, source `"W H"`, bounding `"W H"`, rotações, preço, nome pt-BR.
- `ClothingInformation`: nome, display, descrição, índice masc, índice fem, preço, RGB, dyeable, tipo (`pants`/`shirt`/`accessory`).
- `HairData`: XNB, tileX, tileY, uniqueLeft, coveredIndex, isBald.
- `hats`: nome, descrição, hideHair, ignoreOffset, tags.
- `weapons`: nome, descrição, dano min/max, knockback, speed, precisão, defesa, tipo, AOE, crit, multiplier.
- `Boots`: nome, descrição, preço, defesa, imunidade, índice em `shoeColors`.
- `Crops`: fases, estação, tile row, índice colheita, regrow, harvestMethod.
- `Fish`: nome, dificuldade, movimento, tamanho min/max, estação/hora, locação...

## 4. sourceRect (fórmulas)

```
Tile padrão 64:  (pos*W % tex.Width, pos*W / tex.Width * H, W, H)   W=H=64 default
Objeto 16:       (idx*16 % tex.Width, idx*16 / tex.Width * 16, 16, 16)
Furniture:       (which*16 % tex.Width, which*16 / tex.Width*16, W*16, H*16)
Chapéu:          (which*20 % tex.Width, which*20 / tex.Width * 80, 20, 20)
Camisa:          (idx*8 % 128, idx*8 / 128*32, 8, 8) + 2ª camada em X+128
Calça:           X += idx%10*192 (+96 se feminino); Y += idx/10*688
Cabelo jogador:  (hair*16 % tex.Width, hair*16 / tex.Width*96, 16, 15)
```

## 5. Roupa e cabelo: item ou índice?

- **Cabelo NÃO é item**: é `Farmer.hair` + `Data/HairData` → `hairstyles2`.
- **Camisa/calça são as duas coisas**: índice primitivo (`Farmer.shirt/pants`) e
  item `Clothing` que guarda o índice. Mesma tilesheet no jogador e no inventário.
- **Chapéu** é item `Hat` com índice em `hats.xnb`.
- **Sapato** é item `Boots`; o ícone vem de `springobjects`, a cor em `shoeColors`.
- `Boots.applyStats` → `Farmer.changeShoeColor` → `shoeColors` troca os pixels
  **268, 269, 270, 271** do `farmer_base` (não é paleta simples).

## 6. Disponibilidade

Todos os XNBs de item, dados e variantes pt-BR já estão em `Content/`. Falta
portar a lógica, não os assets.
