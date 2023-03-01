import { readFileSync } from "fs";
import fetch from "node-fetch";

const argv = process.argv;
if (argv.length < 3) {
  throw new Error("Missing args");
}

fetch(`${argv[2]}/InkassoÄrende/Import`, {
  method: "POST",
  body: readFileSync("ärende-import-1.json"),
});
