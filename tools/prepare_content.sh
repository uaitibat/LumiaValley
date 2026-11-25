#!/usr/bin/env bash
# Copia e poda o Content do Stardew para um projeto UWP.
# SKIP_XACT=1 pula os 475 MB de audio (bom para o teste inicial do pipeline).
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

SRC="${1:-/home/uaiti/.steam/steam/steamapps/common/Stardew Valley/Content}"
DST="${2:-$SCRIPT_DIR/../Content}"
SKIP_XACT="${SKIP_XACT:-0}"

if [ ! -d "$SRC" ]; then
  echo "Content de origem nao encontrado: $SRC" >&2
  exit 1
fi

mkdir -p "$DST"

if [ "$SKIP_XACT" = "1" ]; then
  find "$SRC" -mindepth 1 -maxdepth 1 ! -name XACT -exec cp -a {} "$DST/" \;
else
  cp -a "$SRC/." "$DST/"
fi

# Mantem so o base + pt-BR. Ajuste KEEP_LOCALE se quiser outro idioma.
KEEP_LOCALE="${KEEP_LOCALE:-pt-BR}"
LOCALES="de-DE|es-ES|fr-FR|hu-HU|it-IT|ja-JP|ko-KR|ru-RU|tr-TR|zh-CN"
find "$DST" -type f -name '*.xnb' \
  | grep -E "\\.(${LOCALES})\\.xnb\$" \
  | while IFS= read -r f; do rm -f "$f"; done

# Fontes CJK e russas sao gigantes e so sao usadas por esses idiomas.
rm -rf "$DST"/Fonts/Chinese* "$DST"/Fonts/Japanese* \
       "$DST"/Fonts/Korean* "$DST"/Fonts/Russian*

echo "Idioma mantido: base + $KEEP_LOCALE"
echo "SKIP_XACT=$SKIP_XACT"
du -sh "$DST"
du -sh "$DST"/* 2>/dev/null | sort -h | tail -12
