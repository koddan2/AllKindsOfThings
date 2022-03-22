const cp = require("child_process");

require("esbuild").build({
  entryPoints: ["test/app.ts"],
  outfile: "test/out/app.js",
  sourcemap: true,
  bundle: true,
  watch: {
    onRebuild(error, result) {
      if (error) console.error("watch build failed:", error);
      else console.log("watch build succeeded:", result);
      // TODO: Xplat
      cp.exec('"../../_tools/deno.exe" fmt --ignore=node_modules/', (error) => {
        if (error) console.error(error);
      });
    },
  },
}).then((result) => {
  console.log("watching...");
});

cp.exec('"../../_tools/deno.exe" fmt --ignore=node_modules/', (error) => {
  if (error) console.error(error);
});
