window.getScrollInfo = function (element) {
    return {
        scrollTop: element.scrollTop,
        scrollHeight: element.scrollHeight,
        clientHeight: element.clientHeight
    };
};

window.scrollToTop = function (element) {
    element.scrollTop = 0;
};

window.scrollToBottom = function (element) {
    element.scrollTop = element.scrollHeight - element.clientHeight;
};