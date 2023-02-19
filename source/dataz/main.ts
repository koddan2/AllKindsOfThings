import { getAllData } from "./example-data";
import { tablify } from "./tablify";

document.addEventListener("DOMContentLoaded", function init() {
  const mountPoint = document.getElementById("root");
  if (mountPoint)
    tablify(mountPoint, getAllData({}), {
      level: 2,
      transforms: {
        boolean(ctx, element, value) {
          const el = ctx.ele("input", element) as HTMLInputElement;
          el.type = "checkbox";
          el.disabled = true;
          if (value) {
            el.checked = true;
            el.title = "boolean:true";
          } else {
            el.title = "boolean:false";
          }
        },
        number(ctx, element, value) {
          const num = ctx.ele("span", element, "number", value.toString());
          num.title = `➡
          Base2:${value.toString(2)}
          Base8:${value.toString(8)}
          Base16:${value.toString(16)}
          Base36:${value.toString(36)}`
            .replace(/\s+/g, "\n")
            .replace(/:/g, ": \t");
        },
      },
      strings: {
        properties: {
          id: "Identity",
          url: "Address",
        },
      },
    });
});