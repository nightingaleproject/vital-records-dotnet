const reRegExpChar = /[\\^$.*+?()[\]{}|]/g,
  reHasRegExpChar = RegExp(reRegExpChar.source);

export const escapeRegExp = (s: string) => {
  return s && reHasRegExpChar.test(s) ? s.replace(reRegExpChar, '\\$&') : s;
};
