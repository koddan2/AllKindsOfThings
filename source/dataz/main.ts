export interface API {
  render(data: unknown): void;
}
document.addEventListener("DOMContentLoaded", () => {
  const api = mount(document.getElementById("root"));
  api.render({
    name: "Hello",
    values: [1, 2, 3, 4],
    question: null,
  });
});
export function mount(mountPoint: HTMLElement): API {
  const doc = mountPoint.ownerDocument;
  function e(
    name: string,
    parent?: HTMLElement,
    text?: string,
    className?: string
  ): HTMLElement {
    const element = doc.createElement(name);
    if (parent) {
      parent.appendChild(element);
    }
    if (text) {
      element.innerText = text;
    }
    if (className) {
      element.className = className;
    }
    return element;
  }

  function renderInto(parent: HTMLElement, data?: unknown) {
    if (data != null) {
      if (typeof data === "object") {
        if (Array.isArray(data)) {
          e("div", parent, "[");

          const elementContents = e("div", parent);
          for (let i = 0; i < data.length; i++) {
            const element = data[i];
            renderInto(elementContents, element);
          }

          e("div", parent, "]");
        } else {
          e("div", parent, "{");

          const elementContents = e("div", parent);
          for (let k of Object.keys(data)) {
            const element = data[k];
            renderInto(elementContents, k);
            renderInto(elementContents, element);
          }

          const elementCloseBracket = e("div", parent, "]");
        }
      } else if (typeof data === "number") {
      } else if (typeof data === "boolean") {
      } else if (typeof data === "string") {
        e("span", parent, data);
      }
    } else {
      const element = e("span", parent, "NULL", "keyword");
    }
  }

  let volatileRoot: HTMLElement;
  return {
    render(data?: unknown): void {
      if (volatileRoot != null) {
        volatileRoot.remove();
      }
      volatileRoot = e("div", mountPoint);
      renderInto(volatileRoot, data);
    },
  };
}
