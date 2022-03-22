import cls from "classnames";
import m, { Vnode } from "mithril";
import hh from "hyperscript-helpers";
const { section } = hh(m);

import {
  affix as affixButton,
  Button,
  css as cssButton,
} from "@texture/components/button/mod";

document.addEventListener("DOMContentLoaded", init, { capture: false });

function Counter({ attrs: initialAttrs }: Vnode<{ count?: number }>) {
  let state = initialAttrs.count || 0;
  const extraCss = `
    .wrapper span {margin: 0 10px;}
    .wrapper .${affixButton}.button {background:red;}
  `;
  return {
    view() {
      return section({ className: cls("wrapper") }, [
        m("style", cssButton),
        m("style", extraCss),
        m(Button, { label: "-", signals: { activate: (_) => state -= 1 } }),
        m("span", state),
        m(Button, { label: "+", signals: { activate: (_) => state += 1 } }),
      ]);
    },
  };
}

function App() {
  return {
    view() {
      return [
        m("style", "html,body{background:#222;color:#eee}"),
        m(Counter),
      ];
    },
  };
}

function init() {
  m.mount(document.body, App);
}
