enum ShapeType {
    circle = 'circle',
    square = 'square',
    triangle = 'triangle'
}

type Shape =
    | { type: 'circle', radius: number }
    | { type: 'square', side: number }
    | { type: 'triangle', base: number, height: number }


function typeSwitch<T, K extends keyof T, U>(value: T, d: { [k in K]: (T) => U }): U {
}

const shape: Shape = { type: 'circle', radius: 10 };
typeSwitch(shape, {
    circle: (circle:Shape) => circle.radius,
    square: (square:Shape) => square.side,
    triangle: (triangle:Shape) => triangle.base,
})