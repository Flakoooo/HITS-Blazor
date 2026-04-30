window.dragDrop = {
    dotNetHelper: null,
    isDragging: false,

    initializeGlobalMouseEvents: function (dotNetHelper) {
        this.dotNetHelper = dotNetHelper;
    },

    startGlobalDrag: function () {
        this.isDragging = true;

        // Добавляем глобальные обработчики на document
        document.addEventListener('mousemove', this.globalMouseMoveHandler);
        document.addEventListener('mouseup', this.globalMouseUpHandler);

        // Также добавляем на window для обработки за пределами документа
        window.addEventListener('mousemove', this.globalMouseMoveHandler);
        window.addEventListener('mouseup', this.globalMouseUpHandler);
    },

    endGlobalDrag: function () {
        this.isDragging = false;
        this.removeGlobalMouseEvents();
    },

    globalMouseMoveHandler: function (e) {
        if (window.dragDrop.dotNetHelper && window.dragDrop.isDragging) {
            window.dragDrop.dotNetHelper.invokeMethodAsync('OnGlobalMouseMove', e.clientX, e.clientY);
        }
    },

    globalMouseUpHandler: function (e) {
        if (window.dragDrop.dotNetHelper && window.dragDrop.isDragging) {
            window.dragDrop.dotNetHelper.invokeMethodAsync('OnGlobalMouseUp', e.clientX, e.clientY);
        }
        window.dragDrop.isDragging = false;
        window.dragDrop.removeGlobalMouseEvents();
    },

    removeGlobalMouseEvents: function () {
        document.removeEventListener('mousemove', this.globalMouseMoveHandler);
        document.removeEventListener('mouseup', this.globalMouseUpHandler);
        window.removeEventListener('mousemove', this.globalMouseMoveHandler);
        window.removeEventListener('mouseup', this.globalMouseUpHandler);
    },

    preventSelection: function () {
        document.body.style.userSelect = 'none';
        document.body.style.webkitUserSelect = 'none';
        document.body.style.mozUserSelect = 'none';
        document.body.style.msUserSelect = 'none';
    },

    allowSelection: function () {
        document.body.style.userSelect = '';
        document.body.style.webkitUserSelect = '';
        document.body.style.mozUserSelect = '';
        document.body.style.msUserSelect = '';
    }
};