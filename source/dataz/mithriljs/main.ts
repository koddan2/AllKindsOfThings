import m from "mithril";
import { Configuration, Attrs, TablifyComponent } from "./tablify.component";

import { getAllData } from "../example-data";

document.addEventListener("DOMContentLoaded", () => {
  m.mount(document.body, {
    view() {
      // const data = window;
      // const data = {
      //   array: [1,2,3,new Date()],
      //   location: window.location,
      // };
      const configuration: Configuration = {
        maxLevel: 3,
        strings: {
          propertyNames: {
            data0: "Grundläggande data",
            created_at: "Skapad",
          },
        },
        transforms: {
          boolean(state, value) {
            return m("input[type=checkbox]", {
              disabled: true,
              checked: value,
            });
          },
          // number(state, value) {
          //   const x = n => [value.toString(n), m('sub', n)]
          //   return m("div", [
          //     m("div", [value, m('sub', 10)]),
          //     m("div", x(2)),
          //     m("div", x(8)),
          //     m("div", x(16)),
          //     m("div", x(36)),
          //   ]);
          // },
          string(state, value) {
            return m("pre", value);
          },
        },
        // showPaths: false,
        // hideControls: true,
      };
      const data = getAllData({
        test: TablifyComponent,
        multiline: `TESTING MULTI
"Here we go!"
- What the hell?`,
        anotherTest: `"there are" double quotes ""in here""`,
      });
      const attrs: Attrs = {
        configuration,
        data,
      };
      return m(TablifyComponent, attrs);
    },
  });
});
