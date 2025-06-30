import fs from 'fs';

/** @type {{ gists: {id: string, name: string, description: string, constraints?: string[]}[] }} */
const gistJson = JSON.parse(fs.readFileSync('gists.json', 'utf-8'));
const gistInfoById = new Map(gistJson.gists.map((gist) => [gist.id, gist]));

for (const gist of fs.readdirSync('Scripts')) {
    if (gist.endsWith(".meta")) continue;
    if (!fs.statSync(`Scripts/${gist}`).isDirectory()) continue;
    const id = getIdFromMetaFile(fs.readFileSync(`Scripts/${gist}.meta`));
    const gistInfo = gistInfoById.get(id);
    if (!gistInfo) throw new Error("No gist info found for " + id + " in gists.json");
    const slug = createSlug(gist);
    const dirName = `.docs/content/docs/reference/${slug}`;

    if (fs.existsSync(dirName + '/index.md')) {
        console.log("Skipping generation for " + gist + " as it already exists.");
        continue;
    }

    const readmeFiles = fs.readdirSync(`Scripts/${gist}`).filter(file => file.endsWith('README.md') && !file.endsWith('.meta'));
    if (readmeFiles.length == 1) {
        const readmeFile = `Scripts/${gist}/${readmeFiles[0]}`;
        const readmeFileContents = fs.readFileSync(readmeFile, 'utf-8');
        const jaReadmeFile = readmeFile.replace(/README\.md$/, 'README.ja.md');
        const jaReadmeFileContents = fs.existsSync(jaReadmeFile) ? fs.readFileSync(jaReadmeFile, 'utf-8') : readmeFileContents;

        const enContents = extractDocumentFromReadmeFile(slug, readmeFileContents);
        const jaContents = extractDocumentFromReadmeFile(slug, jaReadmeFileContents);

        fs.mkdirSync(dirName, { recursive: true });
        fs.writeFileSync(`${dirName}/index.md`, generateMarkdownFile(slug, enContents, gistInfo, 'en'));
        fs.writeFileSync(`${dirName}/index.ja.md`, generateMarkdownFile(slug, jaContents, gistInfo, 'ja'));
        continue;// TODO
    }

    const files = fs.readdirSync(`Scripts/${gist}`).filter(file => file.endsWith('.cs') && !file.endsWith('.meta'));
    if (files.length == 1) {
        const file = `Scripts/${gist}/${files[0]}`;
        const contents = styleCommentToMarkdown(extractDocumentFromCsFile(slug, fs.readFileSync(file, 'utf-8')));
        fs.mkdirSync(dirName, { recursive: true });
        fs.writeFileSync(`${dirName}/index.md`, generateMarkdownFile(slug, contents, gistInfo, 'en'));
        fs.writeFileSync(`${dirName}/index.ja.md`, generateMarkdownFile(slug, contents, gistInfo, 'ja'));
        continue;
    }
    console.log("Skipping generation for " + gist + " as it has no valid files.");
}

function getIdFromMetaFile(metaFileContents) {
    // The meta file contains a line like 'guid: 1234567890abcdef1234567890abcdef'
    const match = metaFileContents.toString().match(/guid:\s*([a-z0-9]{32})/i);
    if (!match) {
        throw new Error("No guid found in meta file");
    }
    return match[1];
}

function createSlug(name) {
    let slug = name.replaceAll(/(?=[A-Z])/g, '-').toLowerCase();
    // remove leading '-'
    slug = slug.replace(/^-/, '');
    // we might have something like -p-n-g so we need to replace that with a single '-'
    slug = slug.replaceAll(/(-[a-z])+((?=-)|$)/g, match => '-' + match.replaceAll(/-/g, ''));
    // exceptions: VRChat -> vrchat instead of vr-chat
    slug = slug.replace(/vr-chat/g, 'vrchat');
    return slug;
}

function extractHeadingComment(fileContents) {
    // heading comments are in the form of '/*' .. '*/' at the top of the file, or muti-line '//' comments
    const firstRangeCommentIndex = fileContents.indexOf('/*');
    const firstLineCommentIndex = fileContents.indexOf('//');
    if (firstRangeCommentIndex === -1 && firstLineCommentIndex === -1) return null;

    if (firstRangeCommentIndex !== -1 && firstRangeCommentIndex < firstLineCommentIndex) {
        // /* .. */ comment is first
        const firstComment = fileContents.match(/\/\*[\s\S]*?\*\//)?.[0];
        if (!firstComment) return null;

        // remove leading '/*', ' * ', and trailing '*/' from the comment
        return firstComment
            .replaceAll(/^\/\*\s*/g, '')
            .replaceAll(/\s*\*\/$/g, '')
            .replaceAll(/^\s*\* ?/gm, '');
    } else {
        // // comment is first
        const firstComment = fileContents.match(/(\/\/.*\n|\n)+/)?.[0];
        if (!firstComment) return null;
        // remove leading '//' and trailing '\n' from the comment
        return firstComment.replaceAll(/^\/\/ */gm, '')
    }
}

function extractDocumentFromCsFile(slug, fileContents) {
    // First, we extract first portion of the file with '/*' .. '*/' comment
    let commentContent = extractHeadingComment(fileContents);

    // The heading command consists of tree parts:
    // a) The name of the gist at the top
    // b) (optional) short description of the gist
    // c) (optional) url to the gist on GitHub
    // d) longer description of the gist with how to use it
    // e) the license information, tipically MIT License
    // f) the short description of how to install the gist in Unity
    // remove a, c, and f)

    // rmeove the heading newlines
    commentContent = commentContent.replace(/^\s*\n/, '');

    // rmeove a) the first line if it matches the gistname
    {
        const firstLine = commentContent.split('\n', 2)[0].trim().replaceAll(/\s+/g, '').toLowerCase();
        const gistName = slug.replace(/-/g, '');
        if (firstLine === gistName) {
            commentContent = commentContent.replace(/^.*\n+/, '');
        }
    }

    // Remove line with the url to the gist on GitHub
    commentContent = commentContent.replace(/https:\/\/gist\.github\.com\/.+\n/, '');

    // Remove e) remove lines after 'MIT License' followed by a blank line, and then a line starting with 'Copyright'
    commentContent = commentContent.replace(/(\n+|^)MIT License\n\nCopyright[\s\S]*/, '');
    // Remove e) for CompileLogger. it consists of 'Copyright' line followed by 'Published under .*', and multiple 'see URL' lines
    commentContent = commentContent.replace(/Copyright.*?\nPublished under .*?\n(?:see .*\n)+/, '');

    // Remove f). It will use 'Copy this cs file to anywhere in your asset folder is the only step to install this tool.'
    commentContent = commentContent.replace(/Copy this cs file to anywhere in your asset folder is the only step to install this tool\.\n+/, '');

    // Remove any leading and trailing newlines
    commentContent = commentContent.replace(/^\s*\n+/, '').replace(/\n+\s*$/, '');

    return commentContent;
}

function extractDocumentFromReadmeFile(slug, fileContents) {
    // Remove the first heading
    const firstHeadingLevel = fileContents.match(/^(#+)\s+(.+)/m)[1].length;
    if (!firstHeadingLevel) return null;
    // remove the first heading
    fileContents = fileContents.replace(/^(#+)\s+(.+)/m, '');
    // remove heaidng / trailing blank lines
    fileContents = fileContents.replace(/^\s*\n+/, '').replace(/\n+\s*$/, '');
    // remove license section
    fileContents = fileContents.replace(/\n+(##+) License[\s\S]*?(\1|$)/, '');
    // remove install steps
    fileContents = fileContents.replaceAll(/.*(download zip|unzip this|ZIP.*展開).*\n/gi, '');
    // we might have removed '1.' '2.' so replace \d+\. with '1. '
    fileContents = fileContents.replaceAll(/^\d+\.\s+/gm, '1. ');
    return fileContents;
}

function styleCommentToMarkdown(comment) {
    if (!comment) return comment;
    // Convert new lines to markdown new-line with backslash
    comment = comment.replaceAll(/(?<!\n)\n(?![\n-])/g, "\\\n");
    // Tools/anatawa12 gists/\w+ to codeblock
    comment = comment.replaceAll(/(?<!`)(Tools\/anatawa12 gists\/\w+)(?!`)/g, '`$1`');
    return comment;
}

function generateMarkdownFile(slug, extractedContent, gistInfo, language = 'en') {
    const messages = {
        en: {
            generatedWarning: "This documentation is automatically generated from the comment of the script, or README.md file in the script directory. You might see some formatting issues and heading links might not work in the future.",
            dependencyPackages: "Dependency Packages",
            descriptionAndUsage: "Description and Usage",
            noDependencyPackages: "This tool does not depend on any other packages, can be used in any modern Unity project.",
            howToAddToProject: "How to add this tool to your project",
            howToAddToProjectContent: "Enable '{{name}}' in the selector window. Please refer [Basic Usage] page for more details.",
            noDescription: "No description provided for this tool.",
        },
        ja: {
            generatedWarning: "このドキュメントは、スクリプトのコメントまたはスクリプトディレクトリ内のREADME.mdファイルから自動的に生成されています。将来、フォーマットの問題や見出しリンクが機能しない可能性があります。また、翻訳が行われていない場合があります。",
            dependencyPackages: "依存パッケージ",
            descriptionAndUsage: "説明と使用方法",
            noDependencyPackages: "このツールは他のパッケージに依存していません。最新のUnityプロジェクトで使用できます。",
            howToAddToProject: "このツールをプロジェクトに追加する方法",
            howToAddToProjectContent: "セレクタウィンドウで「{{name}}」を有効にします。詳細については、[Basic Usage]ページを参照してください。",
            noDescription: "このツールの説明は提供されていません。",
        },
    }
    const dependenctNames = {
        VRCSDK_AVATARS: "VRCSDK Avatars",
        VRCSDK_BASE: "VRCSDK Base (Avatars or Worlds)",
        VRCSDK_WORLD: "VRCSDK Worlds",
        UDON_SHARP: "UdonSharp or VRCSDK Worlds 3.4.0+"
    }

    const gistDisplayName = slug.replace(/-/g, ' ').replace(/\b\w/g, (c) => c.toUpperCase());

    const dependencyPackages = (gistInfo.constraints || []).map((constraint) => dependenctNames[constraint] || constraint).filter((name) => name);

    return `---
weight: 5
bookFlatSection: true
title: "${gistDisplayName}"
---

# ${gistDisplayName}

${gistInfo.description}

{{< hint info >}}

${messages[language].generatedWarning}

{{< /hint >}}

## ${messages[language].dependencyPackages} {#dependency-packages}

${dependencyPackages.length > 0 ? dependencyPackages.map((name) => `- ${name}`).join('\n') : messages[language].noDependencyPackages}

## ${messages[language].howToAddToProject} {#how-to-add-to-project}

${messages[language].howToAddToProjectContent.replace('{{name}}', gistInfo.name)}

## ${messages[language].descriptionAndUsage} {#description-and-usage}

${extractedContent || messages[language].noDescription}

[Basic Usage]: /gists/${language}/docs/basic-usage/
`
}
