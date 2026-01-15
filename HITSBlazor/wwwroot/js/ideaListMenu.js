window.setupDropdownCloseHandler = (dotNetHelper) => {
    document.addEventListener('click', function (event) {
        const clickedOnMenu = event.target.closest('.idea-row-modal');
        const clickedOnTrigger = event.target.closest('.idea-row-icon i.bi-three-dots');

        if (!clickedOnMenu && !clickedOnTrigger) {
            dotNetHelper.invokeMethodAsync('CloseDropdown');
        }
    });
};