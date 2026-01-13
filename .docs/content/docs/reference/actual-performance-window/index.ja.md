---
weight: 5
bookFlatSection: true
title: "Actual Performance Window"
---

# Actual Performance Window

Actual Performance WindowはPlay Modeやbuild時のアバターのPerformance RankをVRChat Clientを起動せずに見るためのウィンドウです。

[GitHub Gistでソースコードを見る](https://gist.github.com/a4bb4e2e5d75b4fa5ba42e236aae564d)

[Avatar Optimizer]や[Modular Avatar]のようなPlay Modeに入るときに適用される非破壊改変ツールを使っている場合、Play Modeのプレビューのパフォーマンスランクを確認できます。

もし複数のアバターがSceneにある場合には、上部でパフォーマンスランクをみるアバターを選択できます。

表示されるパフォーマンスランクはVRCSDKによって計算されますが、VRChatの変更によりVRChat Clientで表示されるものと違う場合があります。

## このツールをプロジェクトに追加する方法 {#how-to-add-to-project}

セレクタウィンドウで「AlignXAxisOnPlaneWithYRotation」を有効にします。詳細については、[Basic Usage]ページを参照してください。

![window image](window.png)

[Avatar Optimizer]: https://vpm.anatawa12.com/avatar-optimizer/ja/
[Modular Avatar]: https://modular-avatar.nadena.dev/ja/

[Basic Usage]: /gists/ja/docs/basic-usage/
