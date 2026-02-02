function getElementHeight(element) {
    if (!element) return 0;
    return element.scrollHeight;
}

window.getElementHeight = getElementHeight;