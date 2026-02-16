window.menuDropdown = (function () {
    let currentHandler = null;

    function registerClickOutside(triggerRef, menuRef, dotNetRef) {
        if (currentHandler) {
            document.removeEventListener('click', currentHandler);
        }

        currentTriggerRef = triggerRef;
        currentMenuRef = menuRef;

        const handler = (e) => {
            if (!document.body.contains(triggerRef) || !document.body.contains(menuRef)) {
                document.removeEventListener('click', handler);
                currentHandler = null;
                return;
            }

            if (triggerRef && !triggerRef.contains(e.target) &&
                menuRef && !menuRef.contains(e.target)) {

                dotNetRef.invokeMethodAsync('CloseDropdown');
            }
        };

        setTimeout(() => {
            document.addEventListener('click', handler);
            currentHandler = handler;
        }, 10);
    }

    function unregisterClickOutside() {
        if (currentHandler) {
            document.removeEventListener('click', currentHandler);
            currentHandler = null;
        }
        currentTriggerRef = null;
        currentMenuRef = null;
    }

    function ensureMenuVisible(menuRef) {
        if (!menuRef || !document.body.contains(menuRef)) return;

        menuRef.style.display = 'block';
        menuRef.style.visibility = 'visible';
        menuRef.style.opacity = '0';
        menuRef.offsetHeight;
    }

    function startMenuAnimation(menuRef) {
        if (!menuRef || !document.body.contains(menuRef)) return;

        menuRef.classList.remove('is-visible');

        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                if (menuRef && document.body.contains(menuRef)) {
                    menuRef.classList.add('is-visible');
                    menuRef.style.opacity = '1';
                }
            });
        });
    }

    function hideMenu(menuRef) {
        if (!menuRef || !document.body.contains(menuRef)) return;

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
        unregisterClickOutside: unregisterClickOutside,
        ensureMenuVisible: ensureMenuVisible,
        startMenuAnimation: startMenuAnimation,
        hideMenu: hideMenu
    };
})();