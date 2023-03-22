#!/bin/sh

git submodule add "https://gist.github.com/anatawa12/$1.git"

cat << HEREDOC > "$1.meta"
fileFormatVersion: 2
guid: $1
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
HEREDOC

git add "$1.meta"
