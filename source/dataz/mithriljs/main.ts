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
      const data = getAllData();
      const configuration: Configuration = {
        maxLevel: 1,
      };
      const attrs: Attrs = {
        configuration,
        data,
      };
      return m(TablifyComponent, attrs);
    },
  });
});