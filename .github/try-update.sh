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
} >> /tmp/COMMIT-MESSAGE

git commit -F /tmp/COMMIT-MESSAGE

echo "RELEASE_NOTE=/tmp/.CHANGES" >> "$GITHUB_ENV"

./.github/set-patch-release.sh
