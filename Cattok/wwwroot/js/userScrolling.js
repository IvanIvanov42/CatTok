window.initializeInteractionListeners = function (dotNetHelper) {
    let touchStartY = 0;
    let touchEndY = 0;

    function handleTouchStart(event) {
        touchStartY = event.changedTouches[0].screenY;
    }

    function handleTouchEnd(event) {
        touchEndY = event.changedTouches[0].screenY;
        handleGesture();
    }

    function handleGesture() {
        const deltaY = touchEndY - touchStartY;
        if (Math.abs(deltaY) > 50) {
            if (deltaY < 0) {
                dotNetHelper.invokeMethodAsync('OnSwipeUp');
            } else {
                dotNetHelper.invokeMethodAsync('OnSwipeDown');
            }
        }
    }

    document.addEventListener('touchstart', handleTouchStart, false);
    document.addEventListener('touchend', handleTouchEnd, false);

    window._touchStartHandler = handleTouchStart;
    window._touchEndHandler = handleTouchEnd;
};

window.removeInteractionListeners = function () {
    if (window._touchStartHandler && window._touchEndHandler) {
        document.removeEventListener('touchstart', window._touchStartHandler);
        document.removeEventListener('touchend', window._touchEndHandler);
        window._touchStartHandler = null;
        window._touchEndHandler = null;
    }
};

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

window.isMobileDevice = function () {
    return /Android|Mobile|webOS|iPhone|iPad|iPod|BlackBerry|BB|PlayBook|IEMobile|Windows Phone|Kindle|Silk|Opera Mini/i.test(navigator.userAgent);
};
