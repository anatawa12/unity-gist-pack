#!/bin/bash

{
  echo "Manually forced release"
} >> .COMMIT-MESSAGE

echo "RELEASE_NOTE=.COMMIT-MESSAGE" >> "$GITHUB_ENV"

./.github/verup-commit.sh
