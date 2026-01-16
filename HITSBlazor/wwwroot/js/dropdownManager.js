window.dropdownManager = (function () {
    const resizeObservers = new Map();
    let clickOutsideHandler = null;

    function calculateMenuPosition(triggerElement) {
        if (!triggerElement) return null;

        const triggerRect = triggerElement.getBoundingClientRect();
        const viewportHeight = window.innerHeight;
        const viewportWidth = window.innerWidth;

        const MENU_HEIGHT = 120;
        const MENU_WIDTH = 160;

        const spaceBelow = viewportHeight - triggerRect.bottom;
        const spaceAbove = triggerRect.top;

        const showAbove = spaceBelow < MENU_HEIGHT && spaceAbove >= MENU_HEIGHT;

        let left = 0;
        let maxWidth = MENU_WIDTH;

        if (triggerRect.right + MENU_WIDTH > viewportWidth) {
            left = triggerRect.width - MENU_WIDTH;
            if (left < 0) left = 0;

            maxWidth = Math.min(MENU_WIDTH, viewportWidth - triggerRect.left);
        }

        return {
            showAbove: showAbove,
            left: left,
            maxWidth: maxWidth
        };
    }

    function calculateMenuPositionOptimized(triggerElement) {
        return new Promise(resolve => {
            requestAnimationFrame(() => {
                resolve(calculateMenuPosition(triggerElement));
            });
        });
    }

    return {
        calculateMenuPosition: calculateMenuPositionOptimized,

        registerClickOutside: function (element, dotNetHelper) {
            if (clickOutsideHandler) {
                document.removeEventListener('click', clickOutsideHandler, true);
            }

            function isInsideDropdown(target) {
                const componentRoot = element.closest('[tabindex="-1"]') || element;
                return componentRoot.contains(target);
            }

            clickOutsideHandler = function (event) {
                if (!isInsideDropdown(event.target)) {
                    dotNetHelper.invokeMethodAsync('CloseDropdown');
                }
            };

            document.addEventListener('click', clickOutsideHandler, {
                capture: true,
                passive: true
            });

            element._dotNetHelper = dotNetHelper;
        },

        unregisterClickOutside: function (element) {
            if (clickOutsideHandler) {
                document.removeEventListener('click', clickOutsideHandler, true);
                clickOutsideHandler = null;
            }

            if (element._dotNetHelper) {
                element._dotNetHelper = null;
            }
        },

        registerResizeObserver: function (element, dotNetHelper) {
            const id = Date.now() + Math.random();

            let ticking = false;
            const handleResize = function () {
                if (!ticking) {
                    requestAnimationFrame(() => {
                        dotNetHelper.invokeMethodAsync('OnViewportChange');
                        ticking = false;
                    });
                    ticking = true;
                }
            };

            const observer = new ResizeObserver(handleResize);
            observer.observe(element);

            const handleScroll = function () {
                handleResize();
            };

            window.addEventListener('resize', handleResize, { passive: true });
            window.addEventListener('scroll', handleScroll, { passive: true });

            resizeObservers.set(id, {
                observer: observer,
                resizeHandler: handleResize,
                scrollHandler: handleScroll,
                element: element,
                dotNetHelper: dotNetHelper
            });

            return id;
        },

        unregisterResizeObserver: function (id) {
            const observerData = resizeObservers.get(id);
            if (observerData) {
                observerData.observer.disconnect();
                window.removeEventListener('resize', observerData.resizeHandler);
                window.removeEventListener('scroll', observerData.scrollHandler);

                if (observerData.dotNetHelper) {
                    observerData.dotNetHelper = null;
                }

                resizeObservers.delete(id);
            }
        },

        cleanupAll: function () {
            resizeObservers.forEach((data, id) => {
                data.observer.disconnect();
                window.removeEventListener('resize', data.resizeHandler);
                window.removeEventListener('scroll', data.scrollHandler);
            });
            resizeObservers.clear();

            if (clickOutsideHandler) {
                document.removeEventListener('click', clickOutsideHandler, true);
                clickOutsideHandler = null;
            }
        }
    };
})();

window.addEventListener('beforeunload', function () {
    if (window.dropdownManager && window.dropdownManager.cleanupAll) {
        window.dropdownManager.cleanupAll();
    }
});