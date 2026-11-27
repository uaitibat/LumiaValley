# Roadmap do port

## Principios

- Codigo do port escrito por nos, no repo publico.
- Codigo descompilado fica apenas em `reference/` (local, ignorado pelo git).
- Cada fase termina em um `.appx` instalavel no Lumia para validacao no aparelho.
- Base: **Stardew Valley 1.5.6 Windows** (XNB plataforma `w`, formato v5).
- Espaco virtual **1280x720**, cena renderizada em **640x360**, paisagem travada.
- Idiomas: base + pt-BR. Sem assets ou appx no repo publico.
- **REGRA DE OURO**: antes de QUALQUER alteracao, consultar o projeto original em
  `reference/StardewValley/` e `reference/Dependencies/xTile/`. Nunca inventar
  geometria, indice de sprite, formula de sourceRect, cor ou comportamento.
  Se o original fizer de um jeito, portar esse jeito. O catalogo de itens e
  formulas esta em `docs/ITENS.md`.
- **Dispositivo alvo**: Lumia 640, 1280x720 fisicos, escala 150%, resolucao
  logica 720x1280 em retrato / 1280x720 no jogo. Toda coordenada do port e
  validada nesse aparelho (run do Actions + install via Device Portal).

## Fase 0 — Infraestrutura (concluida)

- [x] Esqueleto UWP ARM + MonoGame 3.8.0.1641 no Actions.
- [x] Leitura de XNB v5 direto do appx (validado no Lumia).
- [x] Fullscreen em paisagem, status/nav bar tratadas.
- [x] Coordenadas virtuais 1280x720 + touch convertido.
- [x] RenderTarget 640x360 com blit escalado (performance na Adreno 305).
- [x] Tela de titulo minima + first slice de fazenda (prototipo, sera substituida).
- [x] **Validado no Lumia 640**: fullscreen paisagem, titulo cobrindo a tela e
  fazendeiro andando por toque (run #23).
- [x] Host de telas `IScreen` para transicoes limpas entre fases.

## Fase 1 — Boot ate a home (concluida)

- [x] Splash clicavel da ConcernedApe com animacao de surpresa.
- [x] Fade branco, pausa com paisagem/passaros e subida da camera.
- [x] Logo descendo de cima para baixo (`TitleButtons`) com swipe final.
- [x] Passaros em Y fixo na subida e nova passada quando a camera assenta.
- [x] Home com ceu, montanhas, nuvens, primeiro plano e botoes normal/hover.
- [x] Removidos o botao Voltar e o quadrado de janela do PC.
- [x] **Validado no Lumia 640**: usuario optou por deixar a intro como esta.

## Fase 2 — Funcoes da home (em andamento)

- [x] **New** → tela de confirmacao, cria save proprio e entra na fazenda.
- [x] **Load** → listagem de saves em `ApplicationData.Current.LocalFolder`.
- [x] **Co-op** → mensagem honesta (sem rede no escopo do port).
- [x] **Exit** → confirmacao e `Application.Current.Exit()`.
- [x] Sobre e opcoes basicas (som e resolucao da cena) pelo rodape.
- [x] Volta dos menus para a home sem repetir a intro.
- [ ] **Validar no Lumia**: criar save, carregar, co-op, sair e opcoes.

## Fase 3 — Customizacao de personagem e fazenda (em andamento)

- [x] `New` abre direto a criacao, sem tela de confirmacao intermedia.
- [x] Layout refeito com as coordenadas da 1.5.6: menu, caixas de texto,
  retrato, setas, pickers de cor, OK, Voltar e painel dos tipos de fazenda.
- [x] Nome do fazendeiro, nome da fazenda e coisa favorita (teclado touch proprio).
- [x] Genero, direcao, pet, cor de pele, cabelo/cor, olhos, camisa/cor,
  calcas/cor, calcados e acessorio.
- [x] Botao Aleatorio e os sete tipos de fazenda com descricao ao selecionar.
- [x] Preview do fazendeiro com sprites e paletas da 1.5.6.
- [x] OK so habilita com os tres campos preenchidos, como no original.
- [x] Botoes da home com um unico toque (run #33, `success`).
- [x] Criacao do save inicial com todos os valores escolhidos.
- [ ] **Validar no Lumia**: digitar nomes, trocar aparencia, escolher fazenda e entrar jogando.

## Fase 4 — UI / mapa / input (em andamento)

- [x] Leitor de mapa xTile (`Maps/Farm.xnb`) com `ContentTypeReader` propio
  (formato tBIN10, registrado como `xTile.Pipeline.TideReader, xTile`).
- [x] Renderizador das 5 camadas com as tilesheets `paths` e
  `spring_outdoorsTileSheet`, com culling por viewport.
- [x] Camera seguindo o jogador.
- [x] D-pad virtual (toque) e toque no mundo para mover.
- [x] HUD basico: energia, relogio e dinheiro.
- [x] Moveis sao ITENS (`Furniture.draw` portado): `Data/Furniture` +
  `TileSheets/furniture`, lista inicial da casa (cama 2048, mesa 1120, TV 1466...).
- [ ] Validar no Lumia: moveis, cama e paletas do jogador.
- [ ] Catalogar todos os itens (hierarquia de classes, XNBs, Data, indices).
- [ ] Hold no D-pad, gestures e swipe.
- [ ] Ferramenta selecionada e inventario basico.

## Fase 5 — Mobs e eventos

- [ ] NPCs com animacao e rotinas simples.
- [ ] Mobs, combate e drops.
- [ ] Eventos e/scripts.
- [ ] Audio: converter XACT (468 MB) para o essencial, com SoundEffect.

## Bloqueios conhecidos (tratar antes de cada fase)

- `Steamworks`, `GalaxyCSharp`, `Lidgren.Network`, `SkiaSharp`, `Reflection.Emit` — remover ou substituir.
- `System.IO` de disco → `ApplicationData.Current.LocalFolder`.
- .NET Native nao suporta geracao de codigo dinamica.
- Formato de save: **decidido — formato proprio simples** (JSON/binario no `LocalFolder`).
  Nao ha compatibilidade com saves do PC; isso simplifica a escrita no ARM.
- XACT e o maior peso do pacote; audio fica por ultimo.
