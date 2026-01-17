window.dropdownManager = (function () {
    const openMenus = new Map();
    let clickOutsideHandler = null;

    function shouldShowAbove(triggerElement) {
        if (!triggerElement) return false;

        const triggerRect = triggerElement.getBoundingClientRect();
        const viewportHeight = window.innerHeight;

        const MENU_HEIGHT = 120;

        const spaceBelow = viewportHeight - triggerRect.bottom;
        const spaceAbove = triggerRect.top;

        return spaceBelow < MENU_HEIGHT && spaceAbove >= MENU_HEIGHT;
    }

    function shouldShowAboveOptimized(triggerElement) {
        return new Promise(resolve => {
            requestAnimationFrame(() => {
                resolve(shouldShowAbove(triggerElement));
            });
        });
    }

    function registerClickOutside(element, dotNetHelper, menuId) {
        if (clickOutsideHandler) {
            document.removeEventListener('click', clickOutsideHandler, true);
        }

        function isInsideDropdown(target) {
            const componentRoot = element.closest('[tabindex="-1"]') || element;
            return componentRoot.contains(target);
        }

        clickOutsideHandler = function (event) {
            if (!isInsideDropdown(event.target)) {
                openMenus.forEach((helper, id) => {
                    if (helper) {
                        try {
                            helper.invokeMethodAsync('CloseDropdown');
                        } catch (e) {
                            console.warn('Error closing dropdown:', e);
                            openMenus.delete(id);
                        }
                    }
                });
                openMenus.clear();
            }
        };

        document.addEventListener('click', clickOutsideHandler, {
            capture: true,
            passive: true
        });

        element._menuId = menuId;
        openMenus.set(menuId, dotNetHelper);
    }

    function unregisterClickOutside(element) {
        if (element._menuId) {
            openMenus.delete(element._menuId);
            element._menuId = null;
        }
    }

    function closeOtherMenus(currentMenuId) {
        openMenus.forEach((helper, menuId) => {
            if (menuId !== currentMenuId && helper) {
                try {
                    helper.invokeMethodAsync('CloseDropdown');
                } catch (e) {
                    console.warn('Error closing other dropdown:', e);
                }
            }
        });

        const currentHelper = openMenus.get(currentMenuId);
        openMenus.clear();
        if (currentHelper) {
            openMenus.set(currentMenuId, currentHelper);
        }
    }

    function cleanupAll() {
        if (clickOutsideHandler) {
            document.removeEventListener('click', clickOutsideHandler, true);
            clickOutsideHandler = null;
        }

        openMenus.forEach((helper, id) => {
            if (helper) {
                try {
                    helper.invokeMethodAsync('CloseDropdown');
                } catch (e) {
                    console.warn('Error cleaning up dropdown:', e);
                }
            }
        });
        openMenus.clear();
    }

    return {
        shouldShowAbove: shouldShowAboveOptimized,
        registerClickOutside: registerClickOutside,
        unregisterClickOutside: unregisterClickOutside,
        closeOtherMenus: closeOtherMenus,
        cleanupAll: cleanupAll
    };
})();

window.addEventListener('beforeunload', function () {
    if (window.dropdownManager && window.dropdownManager.cleanupAll) {
        window.dropdownManager.cleanupAll();
    }
});

if (window.Blazor) {
    window.Blazor.addEventListener('beforeNavigate', function () {
        if (window.dropdownManager && window.dropdownManager.cleanupAll) {
            window.dropdownManager.cleanupAll();
        }
    });
}