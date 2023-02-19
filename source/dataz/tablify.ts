interface Transforms {
  boolean?(context: Context, element: HTMLElement, value: boolean): void;
  number?(context: Context, element: HTMLElement, value: number): void;
  bigint?(context: Context, element: HTMLElement, value: bigint): void;
  string?(context: Context, element: HTMLElement, value: string): void;
  date?(context: Context, element: HTMLElement, value: Date): void;
  symbol?(context: Context, element: HTMLElement, value: Symbol): void;
  null?(context: Context, element: HTMLElement, value: null): void;
}
export interface TablifyConfiguration {
  /**
   * Configure which starting level to show. 0 means to show all. 1 means show only first level, etc.
   */
  level?: number;
  /**
   * Supply custom transforms for specific types of values by defining functions here.
   */
  transforms?: Transforms;
  /**
   * Supply custom mapping of values here.
   */
  strings: {
    tablify?: Record<string, string>;
    properties?: Record<string, string>;
    values?: Record<string, string>;
  };
}

const defaultTablifyStrings: Record<string, string> = {
  Property: "Property",
  Index: "Index",
  Value: "Value",
};

export function tablify(
  element: HTMLElement,
  data: unknown,
  configuration?: TablifyConfiguration
): void {
  cleanElement(element);

  const context: Context = makeContext(
    element.ownerDocument,
    configuration || {
      strings: {},
    }
  );

  renderThing(context, element, data);
}

function cleanElement(element: HTMLElement) {
  const children = element.childNodes;
  for (let child of children) {
    element.removeChild(child);
  }
}

function maybeTransform<T>(
  ctx: Context,
  element: HTMLElement,
  value: T,
  t: keyof Transforms,
  defaultClass?: string
) {
  if (ctx.configuration.transforms && ctx.configuration.transforms[t]) {
    const f = ctx.configuration.transforms[t];
    if (f) f(ctx, element, value as never);
  } else if (t === "null") {
    renderKeyword(ctx, element, ctx.getClass("keyword"), "null");
  } else {
    ctx.ele(
      "span",
      element,
      ctx.getClass(defaultClass),
      ((value as any) || "undefined").toString()
    );
  }
}

function renderThing(ctx: Context, element: HTMLElement, data: unknown): void {
  if (typeof data === "object") {
    if (data === null) {
      maybeTransform(ctx, element, data, "null", "keyword");
    } else if (data instanceof Date) {
      ctx.ele("span", element, ctx.getClass("date"), data.toString());
    } else if (Array.isArray(data)) {
      renderArray(ctx, element, data);
    } else {
      if (ctx.referenceLoopCheck.has(data)) {
        ctx.ele("span", element, "reference-loop", "💥");
      } else {
        ctx.referenceLoopCheck.set(data, true);
        renderAssoc(ctx, element, data);
      }
    }
  } else if (typeof data === "bigint") {
    maybeTransform(ctx, element, data, "bigint", "number");
  } else if (typeof data === "number") {
    maybeTransform(ctx, element, data, "number", "number");
  } else if (typeof data === "boolean") {
    maybeTransform(ctx, element, data, "boolean");
  } else if (typeof data === "string") {
    maybeTransform(ctx, element, data, "string");
  } else if (typeof data === "symbol") {
    maybeTransform(ctx, element, data, "symbol", "symbol");
  } else if (data === void 0) {
    renderKeyword(ctx, element, ctx.getClass("keyword"), "undefined");
  } else {
    ctx.errors.push(
      new Error(`Falling back to default rendering of a ${typeof data}`)
    );
    const fallback = data?.toString();
    renderKeyword(ctx, element, ctx.getClass("unknown"), fallback);
  }
}

function renderKeyword(
  ctx: Context,
  element: HTMLElement,
  cls?: string | null,
  contents?: string
) {
  const containingEl = ctx.ele("code", element, cls);
  ctx.ele("pre", containingEl, null, contents);
}

function renderArray(
  ctx: Context,
  element: HTMLElement,
  data: unknown[]
): void {
  if (data.length < 1) {
    renderKeyword(ctx, element, null, "[]");
    return;
  }
  const assocData: Record<string, unknown> = {};
  for (let i = 0; i < data.length; i++) {
    const element = data[i];
    assocData[i.toString()] = element;
  }
  renderAssoc(ctx, element, assocData, ["Index", "Value"]);
}

function renderAssoc(
  ctx: Context,
  element: HTMLElement,
  data: object,
  headingNames?: [string, string]
): void {
  const top = ctx.top;
  if (top) {
    ctx.path.push("$");
  }
  ctx.top = false;
  const keys = Object.keys(data);
  const symbols = Object.getOwnPropertySymbols(data);
  if (keys.length < 1 && symbols.length < 1) {
    renderKeyword(ctx, element, null, "{}");
    return;
  }

  if (headingNames == null) {
    headingNames = ["Property", "Value"];
  }
  const table = ctx.ele("table", element);
  table.className = ctx.getClass();
  const thead = ctx.ele("thead", table);
  const theadtr = ctx.ele("tr", thead);
  const propTh = ctx.ele("th", theadtr, null, ctx.str(headingNames[0]));
  const propThContent = `:${stringifyPath(ctx.path)}`;
  const pre = ctx.ele("pre", propTh, null, propThContent);
  pre.style.display = "inline-block";
  ctx.ele("th", theadtr, null, ctx.str(headingNames[1]));

  const tbody = ctx.ele("tbody", table);

  for (let k of [...keys, ...symbols]) {
    ctx.path.push(k);
    const tr = ctx.ele("tr", tbody);

    const isArray = headingNames[0] == "Index";
    const propCls = isArray ? ctx.getClass("number") : null;
    const propContent = isArray ? k.toString() : ctx.strProp(k.toString());
    const propTd = ctx.ele("td", tr, propCls, propContent);
    const isComplex = data[k] != null && typeof data[k] === "object";
    if (isComplex) {
      propTd.style.cursor = "pointer";

      propTd.addEventListener(
        "click",
        () => {
          if (valuePlaceholder.style.display === "none") {
            valuePlaceholder.style.display = "";
            valueContent.style.display = "none";
          } else {
            valuePlaceholder.style.display = "none";
            valueContent.style.display = "";
          }
        },
        {
          passive: true,
        }
      );
    }

    const valueCell = ctx.ele("td", tr);
    const valueContent = ctx.ele("div", valueCell);
    renderThing(ctx, valueContent, data[k]);
    const valuePlaceholder = ctx.ele("div", valueCell, null, "…");

    const maxLevel = ctx.configuration.level || 999;
    if (isComplex) {
      if (ctx.path.length <= maxLevel) {
        valuePlaceholder.style.display = "none";
      } else {
        valueContent.style.display = "none";
      }
    } else {
      valuePlaceholder.style.display = "none";
    }
    ctx.path.pop();
  }
}

function stringifyPath(path: (string | Symbol)[]): string {
  const builder: string[] = [];
  for (let i = 0; i < path.length; i++) {
    const element = path[i];
    builder.push(element.toString());
  }
  return builder.join(".");
}

interface Context {
  referenceLoopCheck: WeakMap<object, boolean>;
  doc: Document;
  configuration: TablifyConfiguration;
  top: boolean;
  path: (string | Symbol)[];
  getClass(name?: string): string;
  ele(
    name: string,
    parent?: HTMLElement,
    cls?: string | null,
    text?: string
  ): HTMLElement;
  str(key: string, category?: string);
  strProp(key: string, category?: string);
  strValue(key: string, category?: string);
  errors: Error[];
}

function makeContext(
  doc: Document,
  configuration: TablifyConfiguration
): Context {
  return {
    referenceLoopCheck: new WeakMap<object, boolean>(),
    doc,
    configuration,
    top: true,
    path: [],
    getClass(name?: string): string {
      if (name) {
        return `${name} level-${this.level}`;
      }
      return `level-${this.level}`;
    },
    ele(name, parent?, cls?, text?) {
      const result = doc.createElement(name);
      if (parent) {
        parent.appendChild(result);
      }
      if (cls) {
        result.className = cls;
      }
      if (text) {
        result.innerText = text;
      }
      return result;
    },
    str(key, category?) {
      return (
        tryGet(key, configuration.strings.tablify) || defaultTablifyStrings[key]
      );
    },
    strProp(key, category?) {
      return tryGet(key, configuration.strings.properties) || key;
    },
    strValue(value, category?) {
      return tryGet(value, configuration.strings.values) || value;
    },
    errors: [],
  };
}

function tryGet(key: string, obj?: Record<string, string>): string | undefined {
  if (obj) {
    return obj[key];
  }
}
