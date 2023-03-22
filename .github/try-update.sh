#!/bin/bash

set -eu

pushd Scripts

find ./* -type d | while read -r file; do
  pushd "$file"

  PRE_HEAD="$(git rev-parse HEAD)"

  git fetch origin HEAD
  git reset --hard FETCH_HEAD

  if [ "$PRE_HEAD" != "$(git rev-parse HEAD)" ]; then
    echo "$(basename "$file") is updated to $(git rev-parse HEAD)" >> ../../.CHANGES
  fi

  popd

  git add "$file"      
done

popd

if git diff-index --quiet --cached HEAD -- ; then
  exit 0
fi

{
  echo "chore: update gists"
  echo ""
  cat .CHANGES
} >> .COMMIT-MESSAGE

git commit -F .COMMIT-MESSAGE

echo "RELEASE_NOTE.COMMIT-MESSAGE" >> "$GITHUB_ENV"

VERSION="$(jq -r .version < package.json)"
VERSION_PATCH="${VERSION##[0-9.]*.}"
VERSION_MAJOR_PATCH="${VERSION%[0-9]*}"

NEW_PATCH="$(("$VERSION_PATCH" + 1))"

VERSION="$VERSION_MAJOR_PATCH$NEW_PATCH"

cp package.json package.json.1 

jq --arg VERSION "$VERSION" '.version=$VERSION | .' < package.json.1 > package.json

git add "package.json"

git commit -m "Version $VERSION"

echo "VERSION=$VERSION" >> "$GITHUB_ENV"

git tag "v$VERSION"

git clean -fd

exit 0
