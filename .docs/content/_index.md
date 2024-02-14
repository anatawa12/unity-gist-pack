---
title: Introduction
type: docs
---

# anatawa12's gist pack for Unity

Set of anatawa12's gists for Unity.
With this pack, you can upgrade gists using VPM.

anatawa12's gist pack for Unity is an Open Source Software developed on [GitHub] published under the [MIT License].

[GitHub]: https://github.com/anatawa12/unity-gist-pack
[MIT License]: https://github.com/anatawa12/unity-gist-pack/blob/HEAD/LICENSE

## Installation {#installation}

anatawa12's gist pack for Unity is published with [VPM][vpm] repository, so you can install this package using any vpm clients.

### With VCC (Recommended) {#installation-vcc}

1. Click [this link][VCC-add-repo-link] to add anatawa12's repository.
2. Add anatawa12's gists pack from VCC.

### Using Installer UnityPackage with VPAI {#installation-vpai}

With [VPAI] You can install this tool with just importing one unitypackage.

1. download installer unitypackage [here][installer unitypackage].
2. Import the unitypackage into your project.

### Using vrc-get {#installation-vrc-get}

If you're familiar with command line, You may install this package using [vrc-get][vrc-get].

```bash
# add our vpm repository
vrc-get repo add https://vpm.anatawa12.com/vpm.json
# add package to your project
cd /path/to/your-unity-project
vrc-get install com.anatawa12.gists
```

### Using VPM CommandLine Interface {#installation-vpm-cli}

If you're familiar with command line, You may install this package using [VPM/VCC CLI][vcc-cli].

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
