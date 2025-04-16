// scrollIntoView is not defined in jsdom, so make an empty implementation.
window.HTMLElement.prototype.scrollIntoView = jest.fn();