# LumiaValley

Port experimental de **Stardew Valley 1.5.6** para **Lumia 640 / Windows 10 Mobile (UWP ARM)**.

## Estado atual

- Extração completa do `Stardew Valley.dll` com ILSpy 9.0 em `reference/StardewValley/` (753 arquivos C#).
- Dependências descompiladas em `reference/Dependencies/` (xTile, BmFont, CPExtBmFont, GameData, Lidgren, TextCopy).
- Esqueleto UWP ARM com MonoGame 3.8.0.1641 e workflow de appx via GitHub Actions.
- `Game/Game1.cs` é apenas o teste de pipeline (backbuffer 640x360 + touch). Ainda não é o jogo.

## Estrutura

- `reference/` — código descompilado (somente local, **ignorado pelo git**).
- `Content-156/` — assets podados da 1.5.6 (somente local, **ignorados pelo git**).
- `tools/prepare_content.sh` — gera `Content-156/` a partir da instalação Steam.
- `.github/workflows/build-appx.yml` — build ARM + assinatura + release `latest`.

## Base confirmada

- Build Windows 1.5.6, depot 413151, build id 8043676.
- .NET Core 5.0 / win-x64, MonoGame 3.8.0.1641 DesktopGL.
- XNB `w` v5 — por isso o MonoGame UWP é 3.8.0.1641 (mesma versão).

## Aviso legal

Código descompilado e assets do jogo são propriedade da ConcernedApe. Este repositório deve conter
apenas o código do port escrito por nós. Não publicar `reference/`, `Content/` ou appx com assets
do jogo em repositório público.

## Próximos passos

1. Validar o appx de teste no Lumia (Modo Desenvolvedor + Device Portal).
2. Substituir `Game1.cs` pelo `Game1` descompilado, removendo Steam/Galaxy/Skia/Lidgren/Emit.
3. Trocar filesystem por `ApplicationData.Current.LocalFolder` e adicionar UI touch.
4. Reduzir XACT e incluir áudio essencial.
