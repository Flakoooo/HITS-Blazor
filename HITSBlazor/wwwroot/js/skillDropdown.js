window.skillDropdown = {
    registerClickOutside: function (element, dotNetHelper) {
        element._dotNetHelper = dotNetHelper;

        function isInsideDropdown(target) {
            const componentRoot = element.closest('.w-25') || element;
            return componentRoot.contains(target);
        }

        element._clickOutsideHandler = function (event) {
            if (!isInsideDropdown(event.target)) {
                dotNetHelper.invokeMethodAsync('CloseDropdown');
            }
        };

        setTimeout(() => {
            document.addEventListener('click', element._clickOutsideHandler, true);
        }, 0);
    },

    unregisterClickOutside: function (element) {
        if (element._clickOutsideHandler) {
            document.removeEventListener('click', element._clickOutsideHandler, true);
            element._clickOutsideHandler = null;
        }

        if (element._dotNetHelper) {
            element._dotNetHelper.dispose();
            element._dotNetHelper = null;
        }
    }
};