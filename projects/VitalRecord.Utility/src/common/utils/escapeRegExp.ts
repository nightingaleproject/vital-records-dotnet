const reRegExpChar = /[\\^$.*+?()[\]{}|]/g,
  reHasRegExpChar = RegExp(reRegExpChar.source);

export const escapeRegExp = (string: string) => {
  return string && reHasRegExpChar.test(string)
    ? string.replace(reRegExpChar, '\\$&')
    : string;
};
