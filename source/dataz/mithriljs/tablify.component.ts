import m from "./m";
import { Component, Vnode } from "mithril";

interface TransformState {
  path: PropertyKey[];
}
type Transform<T> = (state: TransformState, value: T) => m.Children;
interface Transforms {
  undefined?: Transform<undefined>;
  null?: Transform<null>;
  boolean?: Transform<boolean>;
  number?: Transform<number>;
  bigint?: Transform<bigint>;
  string?: Transform<string>;
  date?: Transform<Date>;
  symbol?: Transform<Symbol>;
  function?: Transform<Function>;
  rest?: Transform<unknown>;
}
const defaultTransforms: Transforms = {
  undefined(state, value) {
    return m("pre", { class: "keyword undefined" }, "undefined");
  },
  null(state, value) {
    return m("pre", { class: "keyword null" }, "null");
  },
  boolean(state, value) {
    return m("pre", { class: "keyword boolean" }, String(value));
  },
  number(state, value) {
    return m("pre", { class: "value number" }, String(value));
  },
  bigint(state, value) {
    return m("pre", { class: "value bigint" }, String(value));
  },
  string(state, value) {
    return m("span", { class: "value string" }, JSON.stringify(value));
  },
  date(state, value) {
    return m("span", { class: "value date" }, `${String(value)}`);
  },
  symbol(state, value) {
    return m("span", { class: "value symbol" }, `${String(value)}`);
  },
  function(state, value) {
    return m("pre", { class: "value reference function" }, String(value));
  },
};
export interface Strings {
  propertyNames?: Record<string, string>;
  values?: Record<string, string>;
}
export interface Configuration {
  strings?: Strings;
  maxLevel: number;
  transforms: Transforms;
  showPaths?: boolean;
  cssClassNamespace?: string;
  hideControls?: boolean;
}

export interface Attrs {
  configuration: Configuration;
  data: unknown;
}

type PropertyKey = string | Symbol | number;
interface PathContext {
  hide: boolean;
}

interface State {
  nonce: string;
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
      return m(
        "div",
        { class: state.configuration.cssClassNamespace || "tablify" },
        [
          !state.configuration.hideControls
            ? tablifyControlsElement(state)
            : m("span"),
          renderUnknown(state, state.data, ["$"]),
          state.debug ? renderUnknown(state, state, ["_$"]) : [],
        ]
      );
    },
  };
}

function initState(attrs: Attrs): State {
  return {
    nonce: ((9999 + Math.random() * 99999) | 0).toString(36),
    configuration: attrs.configuration,
    referenceLoopCheck: new WeakMap<object, PropertyKey[]>(),
    pathState: {},
    data: attrs.data,
    debug: true,
  };
}

function transform<T>(
  state: State,
  data: T,
  path: PropertyKey[],
  customTransform?: Transform<T>,
  defaultTransform?: Transform<T>
): m.Children {
  const tstate: TransformState = {
    path,
  };
  if (customTransform) {
    return customTransform(tstate, data);
  } else if (defaultTransform) {
    return defaultTransform(tstate, data);
  } else {
    return m("span", String(data));
  }
}

function renderUnknown(
  state: State,
  data: unknown,
  path: PropertyKey[]
): m.Children {
  if (typeof data === "object") {
    if (data === null) {
      return transform(
        state,
        data,
        path,
        state.configuration.transforms.null,
        defaultTransforms.null
      );
    } else if (data instanceof Date) {
      return transform(
        state,
        data,
        path,
        state.configuration.transforms.date,
        defaultTransforms.date
      );
    } else if (data instanceof WeakMap) {
      return m("code", `WeakMap()`);
    } else if (data instanceof Map) {
      return m("code", `Map()`);
    } else if (Array.isArray(data)) {
      return renderArray(state, data, path, false);
    } else {
      // Object probably?
      return renderArray(state, data, path, true);
    }
  } else if (typeof data === "undefined") {
    return transform(
      state,
      data,
      path,
      state.configuration.transforms.undefined,
      defaultTransforms.undefined
    );
  } else if (typeof data === "bigint") {
    return transform(
      state,
      data,
      path,
      state.configuration.transforms.bigint,
      defaultTransforms.bigint
    );
  } else if (typeof data === "number") {
    return transform(
      state,
      data,
      path,
      state.configuration.transforms.number,
      defaultTransforms.number
    );
  } else if (typeof data === "boolean") {
    return transform(
      state,
      data,
      path,
      state.configuration.transforms.boolean,
      defaultTransforms.boolean
    );
  } else if (typeof data === "string") {
    return transform(
      state,
      data,
      path,
      state.configuration.transforms.string,
      defaultTransforms.string
    );
  } else if (typeof data === "symbol") {
    return transform(
      state,
      data,
      path,
      state.configuration.transforms.symbol,
      defaultTransforms.symbol
    );
  } else if (typeof data === "function") {
    return transform(
      state,
      data,
      path,
      state.configuration.transforms.function,
      defaultTransforms.function
    );
  } else {
    return m("span", { class: "error" }, "[UNKNOWN]");
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
    if (isAssoc) {
      return m("pre", { class: "empty" }, "{}");
    } else {
      return m("pre", { class: "empty" }, "[]");
    }
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
        const strConflictingPath = stringifyPath(conflictingPath || []);
        children.push(
          m("tr", { key: domKey }, [
            m("td", k.toString()),
            m("td", [
              m(
                "pre",
                { style: "display:inline-block;" },
                m(
                  "a",
                  { href: "#" + strConflictingPath, class: "reference-loop" },
                  `@${strConflictingPath}`
                )
              ),
            ]),
          ])
        );
        continue;
      } else if (item != null) {
        state.referenceLoopCheck.set(item, propPath);
      }
    }
    if (propPath.length > state.configuration.maxLevel && isComplex) {
      const strPropPath = stringifyPath(propPath);
      if (strPropPath.indexOf("_$.pathState") !== 0) {
        state.pathState[strPropPath] = {
          hide: true,
        };
      }
    }

    const hide = state.pathState[stringifyPath(propPath)]?.hide || false;
    children.push(
      m("tr", { key: domKey }, [
        m("td", { id: stringifyPath(propPath) }, [
          m(
            "span",
            {
              title: stringifyPath(propPath),
              "data-original-property-name": k.toString(),
            },
            interpolatePropertyName(state, k.toString())
          ),
          !isComplex ? m("span") : toggleButtonElement(state, propPath),
          !state.configuration.showPaths
            ? m("span")
            : m("pre", stringifyPath(propPath)),
        ]),
        m(
          "td",
          hide
            ? m("span", [
                "…",
                !isComplex
                  ? m("span")
                  : m("span", `${Object.keys(item as any).length} items`),
              ])
            : renderUnknown(state, item, propPath)
        ),
      ])
    );
  }

  const thead = m("thead", [m("tr", [m("th", "Property"), m("th", "Value")])]);
  const table = m("table", [thead, m("tbody", children)]);
  return table;
}

function toggleButtonElement(state: State, path: PropertyKey[]) {
  const index = stringifyPath(path);
  const pathState = state.pathState[index];

  return m(
    "button",
    {
      title: pathState?.hide ? "Expand" : "Collapse",
      type: "button",
      onclick: () => {
        if (!pathState) {
          state.pathState[index] = { hide: true };
        } else {
          pathState.hide = !pathState.hide;
          if (!pathState.hide && path.length > state.configuration.maxLevel)
            state.configuration.maxLevel = path.length;
        }
      },
    },
    pathState?.hide ? "⇲" : "⇱"
  );
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

function tablifyControlsElement(state: State): m.Children {
  return m("div", [
    m("label", { for: id(state, "max-level-manipulator") }, "Max level:"),
    m("input", {
      id: id(state, "max-level-manipulator"),
      type: "number",
      step: 1,
      value: state.configuration.maxLevel,
      oninput(event: InputEvent) {
        const el = <HTMLInputElement>event.target;
        state.configuration.maxLevel = parseInt(el.value);
      },
    }),
    m(
      "button",
      {
        type: "button",
        onclick() {
          expandAllOnce(state);
        },
      },
      "Expand all once"
    ),
    m("label", { for: id(state, "show-paths-toggle") }, "Show paths"),
    m("input", {
      type: "checkbox",
      id: id(state, "show-paths-toggle"),
      checked: state.configuration.showPaths || false,
      onchange(event) {
        const el = <HTMLInputElement>event.target;
        state.configuration.showPaths = el.checked;
      },
    }),
  ]);
}

function id(state: State, v: string): string {
  return `${v}-${state.nonce}`;
}

function expandAllOnce(state: State) {
  let newMaxLevel = 1;
  for (let k of Object.keys(state.pathState)) {
    state.pathState[k].hide = false;
    const t = k.split(/[\.\[]/g);
    newMaxLevel = Math.max(newMaxLevel, t.length);
  }
  state.configuration.maxLevel = newMaxLevel;
}

function interpolatePropertyName(
  state: State,
  propertyName: string
): m.Children {
  const a = state.configuration?.strings?.propertyNames || 0;
  return a[propertyName] || propertyName;
}
