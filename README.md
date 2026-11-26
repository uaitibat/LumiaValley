# LumiaValley

Port experimental de **Stardew Valley 1.5.6** para **Lumia 640 / Windows 10 Mobile (UWP ARM)**.

## Estado atual

- Pipeline UWP ARM validado no Lumia: touch por tap e escala pelo `SwapChainPanel` funcionando.
- XNB v5 da 1.5.6 lido direto do appx no Lumia (validado com `LooseSprites/daybg`).
- Home da Fase 1: splash, fade, câmera, logo e botões com sprites reais da 1.5.6.
- Protótipo de fazenda com o sprite do farmer e movimento por toque (será substituído na Fase 4).
- Host de telas `IScreen`, com transições isoladas por fase.
- Esqueleto UWP ARM com MonoGame 3.8.0.1641 e build via GitHub Actions.
- Código descompilado com ILSpy 9.0 fica **somente local** em `reference/` (753 arquivos C# + dependências).

## Resolução e orientação

O app é **paisagem-only** (o Stardew não tem layout vertical). O manifest e o `Game1` forçam
`landscape` / `landscapeFlipped`.

| Espaço | Tamanho | Uso |
|---|---|---|
| Tela física do Lumia 640 | 1280x720 | saída final |
| Coordenadas virtuais do jogo | 1280x720 | todo o código portado desenha aqui |
| Cena renderizada | 640x360 | `RenderTarget2D`, por performance na Adreno 305 |

`Game/ViewportConfig.cs` centraliza esses valores. Cada tela desenha em 1280x720 com
`Matrix.CreateScale(0.5f)` dentro do `RenderTarget`, e um único blit escalado leva o
resultado ao backbuffer real do `SwapChainPanel`. O touch é convertido com `ToVirtual()`.
O viewport do blit é lido **depois** de `SetRenderTarget(null)` — ler antes fazia o
conteúdo aparecer apenas no canto superior esquerdo.

## Arquitetura do build (repo público + cofre privado)

O repo `uaitibat/LumiaValley` é **público** e contém só código escrito por nós. Para não publicar assets nem appx:

1. `uaitibat/LumiaValley-assets` é um repo **privado** usado apenas como cofre. Ele não roda workflow, então não consome minutos de Actions.
2. O cofre contém `Content.zip` no root (11 MB, base + pt-BR, sem XACT).
3. O workflow `build-appx` do repo público baixa esse zip via Contents API usando o secret `ASSETS_TOKEN`, extrai em `Content/`, compila, assina e publica o `.appx` na release `latest` **do cofre privado**.
4. A release pública do `LumiaValley` não é mais usada para o appx.

### Setup único

- Criar o repo privado `uaitibat/LumiaValley-assets` — **feito**, com `Content.zip` no commit `57fa2db`.
- Criar um PAT fine-grained com acesso somente a `LumiaValley-assets` e permissão **Contents: read+write**.
- Adicionar esse PAT como secret `ASSETS_TOKEN` em `LumiaValley` (Settings → Secrets and variables → Actions).

Depois disso, qualquer push (ou `Run workflow`) gera o appx no cofre privado.

## Estrutura local

- `reference/` — código descompilado (ignorado pelo git).
- `Content/` — assets podados da 1.5.6 (ignorado pelo git; vem do cofre no CI).
- `tools/prepare_content.sh` — gera `Content/` a partir da instalação Steam (`SKIP_XACT=1 bash tools/prepare_content.sh`).
- `.github/workflows/build-appx.yml` — build ARM + assinatura + publicação no cofre.

## Base confirmada

- Build Windows 1.5.6, depot 413151, build id 8043676.
- .NET Core 5.0 / win-x64, MonoGame 3.8.0.1641 DesktopGL.
- XNB `w` v5 — por isso o MonoGame UWP é 3.8.0.1641 (mesma versão).

## Aviso legal

Código descompilado e assets do jogo são propriedade da ConcernedApe. O repo público contém apenas
código do port escrito por nós. Assets e appx ficam exclusivamente no cofre privado, para uso pessoal.

## Como gerar o appx

Abra **Actions → build-appx → Run workflow** no repo `LumiaValley`. O `.appx` aparece na release
`latest` do repo privado `LumiaValley-assets`.

## Próximos passos

1. Fase 1: validar a home no aparelho e acertar sprites/posições/strings.
2. Fase 2: funções de New/Load/Co-op/Exit + saves próprios no `LocalFolder`.
3. Fase 3: customização de personagem e fazenda.
4. Fase 4: mapa xTile real, HUD, inventário e input por toque.
5. Fase 5: NPCs, mobs, eventos e áudio essencial (XACT convertido).
6. Remover Steam/Galaxy/Skia/Lidgren/Reflection.Emit do caminho de build.
