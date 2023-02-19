import m from "mithril";
import { Configuration, Attrs, TablifyComponent } from "./tablify.component";

document.addEventListener("DOMContentLoaded", () => {
  m.mount(document.body, {
    view() {
      // const data = window;
      const data = {
        array: [1,2,3,new Date()],
        location: window.location,
      };
      const configuration: Configuration = {
        maxLevel: 2,
      };
      const attrs: Attrs = {
        configuration,
        data,
      };
      return m(TablifyComponent, attrs);
    },
  });
});
