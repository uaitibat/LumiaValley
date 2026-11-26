# LumiaValley

Port experimental de **Stardew Valley 1.5.6** para **Lumia 640 / Windows 10 Mobile (UWP ARM)**.

## Estado atual

- Pipeline UWP ARM validado no Lumia: touch por tap e escala pelo `SwapChainPanel` funcionando.
- XNB v5 da 1.5.6 lido direto do appx no Lumia (validado com `LooseSprites/daybg`).
- Tela de título mínima com sprites reais (`Minigames/TitleButtons`) e seleção por toque.
- Esqueleto UWP ARM com MonoGame 3.8.0.1641 e build via GitHub Actions.
- Código descompilado com ILSpy 9.0 fica **somente local** em `reference/` (753 arquivos C# + dependências).

## Resolução e orientação

O app é **paisagem-only** (o Stardew não tem layout vertical). O manifest e o `Game1` forçam
`landscape` / `landscapeFlipped`.

| Espaço | Tamanho | Uso |
|---|---|---|
| Tela física do Lumia 640 | 1280x720 | saída final |
| Coordenadas virtuais do jogo | 1280x720 | todo o código portado desenha aqui |
| Backbuffer renderizado | 640x360 | escala 2x na saída, por performance |

`Game/ViewportConfig.cs` centraliza esses valores. O `SpriteBatch.Begin` aplica
`Matrix.CreateScale(0.5f)` e o touch é convertido com `ToVirtual()` antes do uso.
Assim as coordenadas do `Game1` descompilado (1280x720) continuam válidas sem reescrita.

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

1. Evoluir a tela de título para iniciar um save de teste com o `Game1` real.
2. Trocar filesystem por `ApplicationData.Current.LocalFolder` e mapear touch para o input do jogo.
3. Remover Steam/Galaxy/Skia/Lidgren/Reflection.Emit.
4. Reduzir XACT e incluir áudio essencial.
5. Ajustar o fator de escala em `ViewportConfig` conforme FPS medido no dispositivo.
