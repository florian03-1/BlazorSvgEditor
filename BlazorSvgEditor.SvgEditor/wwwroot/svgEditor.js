// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function getBoundingBox(element) { return element ? element.getBoundingClientRect() : {}; }
export function getElementWidth(element) { return element ? element.clientWidth : 0; }
export function getElementHeight(element) { return element ? element.clientHeight : 0; }


export function getElementWidthAndHeight(element) {
  return { width: getElementWidth(element), height: getElementHeight(element) };
}