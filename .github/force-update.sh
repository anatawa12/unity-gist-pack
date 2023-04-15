#!/bin/bash

{
  echo "Manually forced release"
} >> /tmp/COMMIT-MESSAGE

echo "RELEASE_NOTE=/tmp/COMMIT-MESSAGE" >> "$GITHUB_ENV"

./.github/set-patch-release.sh
