#!/bin/bash

VERSION="$(jq -r .version < package.json)"
VERSION_PATCH="${VERSION##[0-9.]*.}"
VERSION_MAJOR_PATCH="${VERSION%[0-9]*}"

NEW_PATCH="$(("$VERSION_PATCH" + 1))"

VERSION="$VERSION_MAJOR_PATCH$NEW_PATCH"

export VERSION

./.github/commit.sh
