---
title: Introduction
type: docs
---

# Unity Gist Pack

anatawa12によるgist群です。
このパックを使用することでVPMからgistを更新できます！

Unity Gists Packは[GitHub]上で開発され、[MIT License]で公開されているオープンソースソフトウェアです。

[GitHub]: https://github.com/anatawa12/unity-gist-pack
[MIT License]: https://github.com/anatawa12/unity-gist-pack/blob/HEAD/LICENSE

## インストール {#installation}

Unity Gists Packは[VPM][vpm]レポジトリを使用して公開されているため、任意のvpmクライアントを使用してインストールできます。

### VCC を使用する (推奨) {#installation-vcc}

[VCC]

1. [このリンク][VCC-add-repo-link]をクリックしてanatawa12のレポジトリを追加する。
2. VCCでanatawa12's gists packを追加する。

### VPAIによるインストーラUnityPackageを使用する {#installation-vpai}

[VPAI]により、unitypackageをインポートするだけでこのツールをインストールできます。

1. [ここ][installer unitypackage]からインストーラunitypackageをダウンロードする。
2. unitypackageをプロジェクトにインポートする。

### vrc-getを使用する {#installation-vrc-get}

もしコマンドラインに精通しているのであれば、[vrc-get][vrc-get]を使用してインストールできます。

```bash
# add our vpm repository
vrc-get repo add https://vpm.anatawa12.com/vpm.json
# add package to your project
cd /path/to/your-unity-project
vrc-get install com.anatawa12.gists
```

### VPMコマンドラインインターフェースを使用する {#installation-vpm-cli}

もしコマンドラインに精通しているのであれば、[VPM/VCC CLI][vcc-cli]を使用してインストールできます。

```bash
# add our vpm repository
vpm add repo https://vpm.anatawa12.com/vpm.json
# add package to your project
cd /path/to/your-unity-project
vpm add package com.anatawa12.gists
```

[VPAI]: https://github.com/anatawa12/VPMPackageAutoInstaller
[vpm]: https://vcc.docs.vrchat.com/vpm/
[vcc-cli]: https://vcc.docs.vrchat.com/vpm/cli
[vrc-get]: https://github.com/anatawa12/vrc-get
[VCC-add-repo-link]: https://vpm.anatawa12.com/add-repo

[installer unitypackage]: https://api.anatawa12.com/create-vpai/?name=unity-gist-pack-{}-installer.unitypackage&repo=https://vpm.anatawa12.com/vpm.json&package=com.anatawa12.gists&version=x.x.x
