#!/bin/bash

set -eu

generate() {
  local GIST_HASH="$1"
  local GIST_ID="$2"
  local IN_PATH="$3"
  local GUID
  GUID="$(printf "%s:%s" "$GIST_HASH" "$IN_PATH" | md5sum | cut -d' ' -f 1)"

  cat <<EOF > "Scripts/$GIST_ID/$IN_PATH.meta"
fileFormatVersion: 2
guid: $GUID
EOF
}

generate-gist() {
  local GIST_HASH="$1"
  local GIST_ID="$2"
  local IN_PATH

  ( cd "Scripts/$GIST_ID" && find .  -not -path '*/.*' -and -not -name '*.meta' -mindepth 1 ) | while read -r IN_PATH; do
    if [ ! -f "Scripts/$GIST_ID/$IN_PATH.meta" ]; then
        generate "$GIST_HASH" "$GIST_ID" "$IN_PATH"
    fi
  done
}

generate-all() {
  local GIST_ID
  ls Scripts | grep -Ev '(\.meta|\.asmdef)$' | while read -r GIST_ID; do
    local GIST_HASH="$(grep 'guid:' < "Scripts/$GIST_ID.meta" | head -1 | cut -d' ' -f 2)"
    generate-gist "$GIST_HASH" "$GIST_ID"
  done
}

generate-all
