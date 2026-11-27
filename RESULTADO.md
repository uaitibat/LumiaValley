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

## Experimento 3 — compilacao real no UWP ARM (GitHub Actions)

A medicao acima era com .NET 8 DesktopGL. Aqui as fontes entram no mesmo
`.csproj` UWP ARM do app funcional (`windows-2022`, MSBuild 17.14, Roslyn
`/langversion:latest`, .NET Native desligado).

| Etapa | Erros CS |
|---|---|
| Build #102 (commit `069c7c0`) | 800 (dominante: `CS8370` file-scoped namespace — 798/807 arquivos) |
| Build #103 (commit `fbf97a9`): `LangVersion` + `AllowUnsafeBlocks` no csproj | 16 |
| Build #105 (commit `b72e833`): stubs/compat de API | 3 (`CS0506` em `XmlSerializer`) |
| Build #106 (commit `9b4ce32`): `Serialize`/`Deserialize` com `new` | 360 (segunda onda: perfil .NET 5 -> UWP) |
| Build #108 (commit `b52ecec`): shim global de API + SDK/SpriteText/audio | 3 (`ThreadStart`) |
| Build #109 (commit `6e07554`): shim `ThreadStart` | 56 (`Binder`, reflexao, XML) |
| Build #110 (commit `dbcee35`): shims de reflexao/IO | 9 (duplicatas + casts) |
| Build #111 (commit `6069700`): remover extensoes duplicadas | **0 — SUCESSO** |

O `LangVersion` nao precisou converter 798 arquivos: o `.csproj` original das
fontes declara `LangVersion 12.0` e `AllowUnsafeBlocks`, e o projeto UWP nao
declarava nada — herdava C# 7.3. Alinhar os duas propriedades removeu os 784
erros de sintaxe de uma vez.

Os 16 restantes sao apenas diferencas de API .NET 5 -> UWP:

- `Stream.Close()`/`BinaryWriter.Close()` nao sao virtuais no UWP (3 erros
  `CS0115`): trocados por `new` em `CrcCalculatorStream`, `ZlibBaseStream` e
  `LoggingBinaryWriter`, preservando o corpo original.
- `System.UnhandledExceptionEventArgs` nao existe: stub em `FullPatches/Stubs.cs`.
- `System.Reflection.Emit` nao existe: `LocalMultiplayer.cs` substituido por
  `FullPatches/LocalMultiplayerCompat.cs` (mesma API publica, usando
  `FieldInfo`; sem splitscreen no Lumia).
- `XmlSerializationReader`/`Writer` nao existem: `Vector2Reader.cs`,
  `Vector2Writer.cs` e `Vector2Serializer.cs` substituidos por
  `FullPatches/Vector2SerializerCompat.cs`, com a mesma conversao XML
  `<Vector2><X/><Y/></Vector2>` sobre a API publica do `XmlSerializer`.

A segunda onda (360 erros) nao era logica do jogo: eram `Console`,
`Thread`, `Environment`, `Process`, `AppDomain`, `Stream.Close()`, `Index`
(para `array[^1]`), `Type.IsGenericType` via extensoes, `XmlTextReader` e
stubs de SDK com assinaturas erradas. Tudo resolvido em `FullPatches/
UwpCompatShims.cs`, `SdkShims.cs` e ajustes nos stubs existentes.

O worktree `fullport` foi criado no commit `f5ca074`, **antes** dos 5 commits de
fidelidade do jogador que entraram na `main` (`9069bb3`, `bcc2c03`, `08f5b17`,
`eaffc46`, `3a59511`). Por isso o primeiro appx testado no Lumia mostrava o
personagem sem roupa, fora da sombra e com animação antiga — nao era regressao
do fullport, era o port incremental daquele commit antigo. O merge `4e178c2`
traz esses 5 commits para o fullport antes de seguir com a troca 1:1.

O build #112 (commit `6069700`) fechou a compilação: **0 erros**. O appx gerado
(`LumiaValley_0.1.0.0_ARM.appx`, 21 MB / 46 MB com dependencias) contem
1.213 arquivos XNB e 120 DLLs de runtime — a arvore inteira do Stardew 1.5.6
compilada dentro de um appx ARM instalavel.

**Estado atual do appx:** o host ainda e o `Game1` do port incremental
(`Pages/GamePage.xaml.cs` usa `XamlGame<LumiaValley.Game.Game1>`). As classes
originais estao linkadas, mas ainda nao sao usadas em runtime. O proximo passo e
trocar sistemas por classes 1:1, um de cada vez, começando por renderizacao de
fazendeiro/moveis — os pontos de fidelidade apontados pelo usuario.

O appx do experimento **nao**
publica na release `latest` (que pertence ao build funcional da `main`): o
workflow do branch salva o pacote como artifact do proprio run.
