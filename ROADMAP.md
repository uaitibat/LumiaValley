# Roadmap do port

## Principios

- Codigo do port escrito por nos, no repo publico.
- Codigo descompilado fica apenas em `reference/` (local, ignorado pelo git).
- Cada fase termina em um `.appx` instalavel no Lumia para validacao no aparelho.
- Base: **Stardew Valley 1.5.6 Windows** (XNB plataforma `w`, formato v5).
- Espaco virtual **1280x720**, cena renderizada em **640x360**, paisagem travada.
- Idiomas: base + pt-BR. Sem assets ou appx no repo publico.

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

## Fase 1 — Boot ate a home (em andamento)

- [x] Splash/intro da ConcernedApe + fade branco + subida da camera (toque pula).
- [x] Home com ceu, montanhas, nuvens, passaros e logo (`TitleButtons`, `Clouds`, `Cursors`).
- [x] Botoes **New / Load / Co-op / Exit** com sprite normal/hover e selecao por toque.
- [x] Rodape Voltar/Sobre/Idioma/Mudo/Janela + strings de `StringsFromCSFiles.pt-BR`.
- [x] Transicoes entre telas sem travar o backbuffer (`IScreen`).
- [ ] **Validar no Lumia**: sprites/posicoes corretos, toque pulando a intro e
  selecao dos botoes respondendo. Ajustar o que estiver diferente.

## Fase 2 — Funcoes da home

- [ ] **New** → fluxo de criacao de save.
- [ ] **Load** → listagem de saves em `ApplicationData.Current.LocalFolder`.
- [ ] **Co-op** → comportamento honesto (sem rede no escopo do port).
- [ ] **Exit** → suspender/fechar o app.
- [ ] Menu de opcoes basico (volume, escala, idioma).

## Fase 3 — Customizacao de personagem e fazenda

- [ ] Nome do fazendeiro e nome da fazenda (texto por toque).
- [ ] Genero, cor de pele, cabelo, camisa, calcas, calcados.
- [ ] Pet (cachorro/gato) e tipo de fazenda.
- [ ] Criacao do save inicial.

## Fase 4 — UI / mapa / input

- [ ] Leitor de mapa xTile (`Maps/Farm.xnb`) com `ContentTypeReader` proprio.
- [ ] Renderizador de camadas e tilesheets.
- [ ] Camera seguindo o jogador e culling por viewport.
- [ ] D-pad virtual + gestures (tap, hold, swipe).
- [ ] HUD: energia, relogio, dinheiro, ferramenta selecionada.
- [ ] Inventario basico.

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
