#!/bin/sh

set -eu

git submodule add --name "Scripts/$1" "https://gist.github.com/anatawa12/$1.git" "$2"

cat << HEREDOC > "$2.meta"
fileFormatVersion: 2
guid: $1
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
HEREDOC

git add "$2.meta"
