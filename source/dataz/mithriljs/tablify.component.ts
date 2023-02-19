import m, { Component, Vnode } from "mithril";
// import * as m from "mithril";

export interface Configuration {
  maxLevel: number;
}
export interface Attrs {
  configuration: Configuration;
  data: unknown;
}

type PropertyKey = string | Symbol | number;
interface PathContext {
  toggle: boolean;
}
interface State {
  configuration: Configuration;
  referenceLoopCheck: WeakMap<object, PropertyKey[]>;
  pathState: Record<string, PathContext>;
  data: unknown;
  debug: boolean;
}
export function TablifyComponent(
  initialVnode: Vnode<Attrs>
): Component<Attrs, State> {
  const state = initState(initialVnode.attrs);
  return {
    view(vnode: Vnode<Attrs>) {
      state.referenceLoopCheck = new WeakMap<object, PropertyKey[]>();
      return m("div", [
        renderUnknown(state, state.data, ["$"]),
        state.debug ? renderUnknown(state, state, ["$"]) : [],
      ]);
    },
  };
}
function initState(attrs: Attrs): State {
  return {
    configuration: attrs.configuration,
    referenceLoopCheck: new WeakMap<object, PropertyKey[]>(),
    pathState: {},
    data: attrs.data,
    debug: true,
  };
}

function renderUnknown(
  state: State,
  data: unknown,
  path: PropertyKey[]
): m.Children {
  if (typeof data === "object") {
    if (data === null) {
      return m("span", "null");
    } else if (data instanceof Date) {
      return m("span", data.toLocaleString());
      // } else if (data instanceof WeakMap) {
      // } else if (data instanceof Map) {
    } else if (Array.isArray(data)) {
      return renderArray(state, data, path, false);
    } else {
      // Object probably?
      return renderArray(state, data, path, true);
    }
  } else if (typeof data === "bigint") {
    return m("span", (data as bigint).toString());
  } else if (typeof data === "number") {
    return m("span", (data as number).toString());
  } else if (typeof data === "boolean") {
    return m("span", (data as boolean).toString());
  } else if (typeof data === "string") {
    return m("span", (data as string).toString());
  } else if (typeof data === "symbol") {
    return m("span", (data as Symbol).toString());
  } else if (typeof data === "function") {
    return m("pre", (data as Function).toString());
  } else if (typeof data === "undefined") {
    return m("span", "undefined");
  } else {
    return m("span", "[UNKNOWN]");
  }
}

function renderArray(
  state: State,
  data: object,
  path: PropertyKey[],
  isAssoc: boolean
): m.Children {
  const keys = Object.keys(data);
  const symbols = Object.getOwnPropertySymbols(data);
  const allProps = [...keys, ...symbols];
  if (allProps.length < 1) {
    return m("span", "{}");
  }
  allProps.sort((a, b) => Number(a.toString() > b.toString()));

  const children: m.Children[] = [];
  for (let k of allProps) {
    const propPath = [...path, k];
    const domKey = stringifyPath(propPath);
    const item: unknown = data[k];
    const isComplex = typeof item === "object" && item != null;
    if (isComplex) {
      if (state.referenceLoopCheck.has(item)) {
        const conflictingPath = state.referenceLoopCheck.get(item);
        children.push(
          m("tr", { key: domKey }, [
            m("td", k.toString()),
            m("td", [
              m("span", "💢"),
              m(
                "pre",
                { style: "display:inline-block;" },
                `@${stringifyPath(conflictingPath || [])}`
              ),
            ]),
          ])
        );
        continue;
      } else if (item != null) {
        state.referenceLoopCheck.set(item, propPath);
      }
    }
    children.push(
      m("tr", { key: domKey }, [
        m("td", [
          m("span", k.toString()),
          m(
            "pre",
            { style: "display:inline-block;margin-left:5px;" },
            stringifyPath(propPath)
          ),
          m(
            "button",
            {
              type: "button",
              onclick: () => {
                const index = stringifyPath(propPath);
                const v = state.pathState[index];
                if (!v) {
                  state.pathState[index] = { toggle: true };
                } else {
                  v.toggle = !v.toggle;
                }
              },
            },
            "toggle"
          ),
        ]),
        m("td", renderUnknown(state, item, propPath)),
      ])
    );
  }

  const thead = m("thead", [m("tr", [m("th", "Property"), m("th", "Value")])]);
  const table = m("table", [thead, m("tbody", children)]);
  return table;
}

function stringifyPath(path: PropertyKey[]): string {
  // return path.map((x) => x.toString()).join(".");
  const re = /^[A-Za-z\$_][A-Za-z0-9\$_]*$/;
  let result = "";
  for (let i = 0; i < path.length; i++) {
    const element = path[i];
    if (typeof element === "string") {
      if (re.test(element)) {
        result += `${i === 0 ? "" : "."}${element}`;
      } else {
        result += `[${element}]`;
      }
    } else if (typeof element === "symbol") {
      result += `[${element.toString()}]`;
    } else if (typeof element === "number") {
      result += `[${element.toString()}]`;
    }
  }

  return result;
}
