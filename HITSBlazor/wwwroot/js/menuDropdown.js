window.menuDropdown = (function () {
    const openMenus = new Map();

    function registerClickOutside(triggerRef, menuRef, dotNetRef) {
        setTimeout(() => {
            const handler = (e) => {
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

    function ensureMenuVisible(menuRef) {
        if (!menuRef) return;

        menuRef.style.display = 'block';
        menuRef.style.visibility = 'visible';
        menuRef.style.opacity = '0';

        menuRef.offsetHeight;
    }

    function startMenuAnimation(menuRef) {
        if (!menuRef) return;

        menuRef.classList.remove('is-visible');

        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                menuRef.classList.add('is-visible');
                menuRef.style.opacity = '1';
            });
        });
    }

    function hideMenu(menuRef) {
        if (!menuRef) return;

        menuRef.classList.remove('is-visible');

        menuRef.style.display = 'none';
        menuRef.style.visibility = 'hidden';
        menuRef.style.opacity = '0';

        menuRef.style.transform = '';
        menuRef.style.top = '';
        menuRef.style.left = '';
    }

    return {
        registerClickOutside: registerClickOutside,
        closeOtherMenus: (id, dotNet) => {
            closeOtherMenus(id);
            openMenus.set(id, dotNet);
        },
        ensureMenuVisible: ensureMenuVisible,
        startMenuAnimation: startMenuAnimation,
        cleanupAll: () => {
            openMenus.forEach(dotNet => dotNet.invokeMethodAsync('CloseDropdown'));
            openMenus.clear();
        },
        hideMenu: hideMenu
    };
})();