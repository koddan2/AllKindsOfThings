import * as esbuild from "https://deno.land/x/esbuild@v0.17.5/mod.js";

// let result = await esbuild.build({
//   entryPoints: ["main.ts"],
//   bundle: true,
//   outdir: "dist",
// });
// console.log(result);
let ctx = await esbuild.context({
  entryPoints: ["main.ts"],
  bundle: true,
  outdir: "dist",
  plugins: [
    {
      name: "info",
      setup(b) {
        b.onEnd((result) => {
            console.log(result)
        });
      },
    },
  ],
});

await ctx.watch();
console.log("Watching...");

function sigIntHandler() {
  console.log("Stopping.");
  esbuild.stop();
  console.log("Bye.");
  Deno.exit(0);
}

Deno.addSignalListener("SIGINT", sigIntHandler);
