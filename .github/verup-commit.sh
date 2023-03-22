#!/bin/bash

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
