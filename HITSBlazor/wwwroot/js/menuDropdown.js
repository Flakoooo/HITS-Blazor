window.menuDropdown = (function () {
    const openMenus = new Map();

    function registerClickOutside(triggerRef, menuRef, dotNetRef) {
        setTimeout(() => {
            const handler = (e) => {
                // Если кликнули ВНЕ триггера и ВНЕ самого меню
                if (triggerRef && !triggerRef.contains(e.target) && menuRef && !menuRef.contains(e.target)) {
                    dotNetRef.invokeMethodAsync('CloseDropdown');
                    document.removeEventListener('click', handler);
                }
            };
            document.addEventListener('click', handler);
        }, 10);
    }

    function closeOtherMenus(currentMenuId) {
        openMenus.forEach((dotNet, id) => {
            if (id !== currentMenuId) {
                dotNet.invokeMethodAsync('CloseDropdown');
            }
        });
    }

    function startMenuAnimation(menuRef) {
        if (!menuRef) return;

        menuRef.classList.remove('show');

        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                menuRef.classList.add('show');
            });
        });
    }

    return {
        registerClickOutside: registerClickOutside,
        closeOtherMenus: (id, dotNet) => {
            closeOtherMenus(id);
            openMenus.set(id, dotNet);
        },
        startMenuAnimation: startMenuAnimation,
        cleanupAll: () => {
            openMenus.forEach(dotNet => dotNet.invokeMethodAsync('CloseDropdown'));
            openMenus.clear();
        }
    };
})();