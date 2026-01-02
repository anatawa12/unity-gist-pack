#!/bin/sh

set -eu

if [ "$#" -lt 4 ] || [ "$#" -gt 6 ]; then
  echo "Usage: $0 GIST_GUID FOLDER_NAME GIST_NAME DESCRIPTION [CONSTRAINTS] [REPLACE_GUIDS]" >&2
  exit 1
fi

GIST_GUID="$1"
FOLDER_NAME="$2"
GIST_NAME="$3"
DESCRIPTION="$4"
CONSTRAINTS="${5:-}"
REPLACE_GUIDS="${6:-}"

git submodule add --name "Scripts/$GIST_GUID" "https://gist.github.com/anatawa12/$GIST_GUID.git" "Scripts/$FOLDER_NAME"

cat << HEREDOC > "Scripts/$FOLDER_NAME.meta"
fileFormatVersion: 2
guid: $GIST_GUID
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
HEREDOC

git add "Scripts/$FOLDER_NAME.meta"

mv gists.json gists.json.bak
jq <gists.json.bak >gists.json \
  --arg GIST_GUID "$GIST_GUID" \
  --arg GIST_NAME "$GIST_NAME" \
  --arg DESCRIPTION "$DESCRIPTION" \
  --arg CONSTRAINTS "$CONSTRAINTS" \
  --arg REPLACE_GUIDS "$REPLACE_GUIDS" \
  '.gists += [
    {
     id: $GIST_GUID,
     name: $GIST_NAME,
     description: $DESCRIPTION,
    }
    | .constraints = ($CONSTRAINTS | split(";"))
    | .guids = [($REPLACE_GUIDS | split(";") | .[] | {enabled: (.|split(":")|.[0]), disabled: (.|split(":")|.[1])})]
    | (if .constraints == [] then del(.constraints) else . end)
    | (if .guids == [] then del(.guids) else . end)
  ]'
rm gists.json.bak
git add gists.json
