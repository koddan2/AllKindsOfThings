import cls from "classnames";
import { createAffix } from "@texture/helpers/affixes";
import m, { Component, Vnode } from "mithril";
import hh from "hyperscript-helpers";
const { button } = hh(m);

export interface Attrs {
  disabled?: boolean;
  className?: string;
  label: string;
  signals: {
    activate: (event: MouseEvent | KeyboardEvent) => void;
  };
}

export const affix = createAffix();

export const css = `
.${affix}.button {
  padding: 2px;
  border: 1px solid silver;
}
`;

export function Button(vnode: Vnode<Attrs>): Component<Attrs> {
  return {
    view({ attrs }: Vnode<Attrs>): Vnode {
      return button({
        className: cls(affix, "button", attrs.className),
        onclick: (event: MouseEvent) => attrs.signals.activate(event),
      }, attrs.label);
    },
  };
}
