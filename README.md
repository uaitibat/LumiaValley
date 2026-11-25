# LumiaValley

Port experimental de **Stardew Valley 1.5.6** para **Lumia 640 / Windows 10 Mobile (UWP ARM)**.

## Estado atual

- Pipeline UWP ARM validado no Lumia: backbuffer 640x360, touch por tap e escala pelo `SwapChainPanel` funcionando.
- Esqueleto UWP ARM com MonoGame 3.8.0.1641 e build via GitHub Actions.
- `Game/Game1.cs` ainda é o teste de pipeline (fundo verde + quadrado amarelo). O jogo real ainda não foi migrado.
- Código descompilado com ILSpy 9.0 fica **somente local** em `reference/` (753 arquivos C# + dependências).

## Arquitetura do build (repo público + cofre privado)

O repo `uaitibat/LumiaValley` é **público** e contém só código escrito por nós. Para não publicar assets nem appx:

1. `uaitibat/LumiaValley-assets` é um repo **privado** usado apenas como cofre. Ele não roda workflow, então não consome minutos de Actions.
2. A release `assets` desse cofre contém `Content.zip` (14 MB, base + pt-BR, sem XACT).
3. O workflow `build-appx` do repo público baixa esse zip usando o secret `ASSETS_TOKEN`, extrai em `Content/`, compila, assina e publica o `.appx` na release `latest` **do cofre privado**.
4. A release pública do `LumiaValley` não é mais usada para o appx.

### Setup único

- Criar o repo privado `uaitibat/LumiaValley-assets`.
- Criar nele uma release com tag `assets` e anexar `Content.zip`.
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

1. Migrar a primeira fatia do `Game1` descompilado (init + leitura de um XNB), reescrevendo o código em vez de copiar.
2. Trocar filesystem por `ApplicationData.Current.LocalFolder` e mapear touch para o input do jogo.
3. Remover Steam/Galaxy/Skia/Lidgren/Reflection.Emit.
4. Reduzir XACT e incluir áudio essencial.
