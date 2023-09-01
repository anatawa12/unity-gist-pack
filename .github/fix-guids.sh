#!/bin/bash


<gists.json jq --raw-output '.gists[].guids | select( . != null ) | .[] | (.enabled + ":" + .disabled)' | while read -r line; do
  enabled=${line%:*}
  disabled=${line##*:}
  meta_path=$(grep -lR Scripts -e "$enabled" | head -1)

  if [ -n "$meta_path" ]; then
    mv "$meta_path" "$meta_path.bak"
    sed "s/$enabled/$disabled/g" < "$meta_path.bak" > "$meta_path"
    rm "$meta_path.bak"
  fi
done

