
const defines = [
    "VRCSDK_WORLDS",
    "VRCSDK_AVATARS",
    "VRCSDK_BASE",
    "UDON_SHARP",
]

console.log("// generated by .defines.ts")
console.log("// ReSharper disable IdentifierTypo")
console.log("// ReSharper disable InconsistentNaming")
console.log("")
console.log("using System;")
console.log("")
console.log("namespace anatawa12.gists.selector")
console.log("{")
console.log("    [Flags]")
console.log("    enum Define")
console.log("    {")
console.log(`        None = 0,`)
for (let i = 0; i < defines.length; i++)
    console.log(`        ${defines[i]} = 1 << ${i},`)
console.log("    }")
console.log("")
console.log("    static class Defines")
console.log("    {")
console.log("        public const Define Active = 0")
for (let define of defines) {
    console.log(`#if ${define}`)
    console.log(`            | Define.${define}`)
    console.log("#endif")
}
console.log("            ;")
console.log("        public static bool IsActive(Define define) => (Active & define) == define;")
console.log("    }")
console.log("}")
