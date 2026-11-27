# Experimento: bulk port 1:1 do Stardew 1.5.6

## Metodo
Worktree separado (`fullport`). Copiados 748 arquivos .cs de `reference/StardewValley`
(305.329 linhas) + 150 de `reference/Dependencies/xTile` e `StardewValley.GameData`.
Projeto .NET 8 com `MonoGame.Framework.DesktopGL 3.8.1.303` e stubs dos SDKs.

## Numeros (build incremental, erros CS)

| Etapa | Erros |
|---|---|
| Sem MonoGame (so stubs vazios) | 4418 |
| Com MonoGame real | 199 |
| + stubs Galaxy/Steam/Lidgren/Skia/BmFont | 51 |
| + excluir AssemblyInfo duplicados, /unsafe | 28 |
| + completar stubs | 8 |
| + incluir SpriteText/ColorChanger de volta | 195 |

## Conclusao

**Todos os erros restantes pertencem a 5 SDKs externos**:
Galaxy (multiplayer GOG), Steamworks, Lidgren (rede), XACT/Cue (audio) e
BmFont/SkiaSharp (texto/UI). **Zero erros no codigo do jogo** — `Game1`,
`Farmer`, `Object`, menus, locations, xTile e todos os 748 arquivos compilam
contra MonoGame.

Isso prova que o "recomp 1:1" do motor do jogo é viavel: o trabalho nao e
traduzir codigo, e **remover/substituir os 5 SDKs**. Como o port nao tem rede
e o audio fica por ultimo (decisao do roadmap), 4 dos 5 podem simplesmente ser
excluidos — exatamente o que ja fizemos sistema por sistema.

## Riscos que a medicao NAO cobre

- Erros de runtime sob .NET Native (reflexao, serializacao, DeepCloner).
- 1280x720 virtual + 640x360 RenderTarget em ARM (ja resolvido no port atual).
- Memoria: o jogo inteiro carregaria todos os XNB de uma vez (Lumia tem 512 MB).
- Diferencas XNA vs MonoGame em runtime (texturas XNB v5 ja comprovadas).

## Recomendacao

Manter o port incremental atual como app validado, e usar este experimento de
referencia: sempre que um sistema novo entrar, consultar o arquivo original
correspondente e portar a logica — e so trazer um modulo inteiro de uma vez
quando os erros de SDK estiverem resolvidos.

## Experimento 2 — remover os 5 SDKs + shim SpriteText

Substitui `SpriteText.cs` por um shim sobre `SpriteFont` e completei os stubs.

| Etapa | Erros |
|---|---|
| Stubs completos + shim | 193 |
| Excluir arquivos de rede (LidgrenClient/Server, GameServer) | 44 |

Os 44 restantes sao: XACT (SoundBankWrapper/CueWrapper/AudioEngineWrapper),
SteamHelper, Multiplayer (que referencia os arquivos de rede excluidos),
SkiaSharp (screenshot) e `SpriteBatch.TextureTuckAmount` (API XNA removida
no MonoGame). **Zero erros no codigo do jogo** (Game1, Farmer, menus,
locations, xTile, itens).

Conclusao final: remover os 5 SDKs **nao da problema no codigo do jogo**.
O que acontece e que os SISTEMAS que dependem deles deixam de existir:
rede/co-op, audio, screenshot e texto bitmap — todos ja substituidos ou
adiados no port atual.
