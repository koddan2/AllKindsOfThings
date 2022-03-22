export function createAffix(): string {
  return `_${(Math.random() * 999999 | 0).toString(36)}`;
}
