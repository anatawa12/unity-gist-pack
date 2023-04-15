#!/bin/bash

cp package.json package.json.1

jq --arg VERSION "$VERSION" '.version=$VERSION | .' < package.json.1 > package.json

git add "package.json"

git commit -m "Version $VERSION"

echo "VERSION=$VERSION" >> "$GITHUB_ENV"

git tag "v$VERSION"

git clean -fd

exit 0
