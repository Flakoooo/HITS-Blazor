window.dragDrop = {
    helpers: {},
    isDragging: false,
    sourceCategory: null,

    initializeGlobalMouseEvents: function (dotNetHelper, category) {
        this.helpers[category] = dotNetHelper;
        if (!this._eventsInitialized) {
            this._eventsInitialized = true;
            document.addEventListener('mousemove', this.globalMouseMoveHandler);
            document.addEventListener('mouseup', this.globalMouseUpHandler);
            window.addEventListener('mousemove', this.globalMouseMoveHandler);
            window.addEventListener('mouseup', this.globalMouseUpHandler);
        }
    },

    startGlobalDrag: function (sourceCategory) {
        this.isDragging = true;
        this.sourceCategory = sourceCategory;
    },

    endGlobalDrag: function () {
        this.isDragging = false;
        this.sourceCategory = null;
    },

    globalMouseMoveHandler: function (e) {
        if (!window.dragDrop.isDragging) return;

        var overlay = document.querySelector('.drag-overlay');
        if (overlay) overlay.style.display = 'none';
        var element = document.elementFromPoint(e.clientX, e.clientY);
        if (overlay) overlay.style.display = '';

        var column = element ? element.closest('.sprint-column-wrapper') : null;
        
        if (!column) {
            var allColumns = document.querySelectorAll('.sprint-column-wrapper');
            for (var c = 0; c < allColumns.length; c++) {
                var cr = allColumns[c].getBoundingClientRect();
                if (e.clientX >= cr.left && e.clientX <= cr.right && e.clientY >= cr.top && e.clientY <= cr.bottom) {
                    column = allColumns[c];
                    break;
                }
            }
        }

        var category = column ? column.getAttribute('data-category') : null;

        var dropIndex = -1;
        if (column) {
            var tasks = column.querySelectorAll('.task-item:not(.task-item-dragging)');
            var dragged = column.querySelector('.task-item-dragging');
            var allTasks = column.querySelectorAll('.task-item');
            var draggedIndex = dragged ? Array.from(allTasks).indexOf(dragged) : -1;

            if (draggedIndex === -1) {
                for (var i = 0; i < tasks.length; i++) {
                    var rect = tasks[i].getBoundingClientRect();
                    if (e.clientY < rect.top + rect.height * 0.5) {
                        dropIndex = i;
                        break;
                    }
                }
            } else {
                for (var i = 0; i < tasks.length; i++) {
                    var rect = tasks[i].getBoundingClientRect();
                    var threshold;
                    if (i < draggedIndex) {
                        threshold = rect.top + rect.height * 0.5;
                    } else {
                        threshold = rect.top + rect.height * 0.1;
                    }
                    if (e.clientY < threshold) {
                        dropIndex = i;
                        break;
                    }
                }
            }

            if (dropIndex === -1) {
                var colRect = column.getBoundingClientRect();
                if (e.clientY >= colRect.top && e.clientY <= colRect.bottom) {
                    dropIndex = tasks.length;
                }
            }

            if (dropIndex !== -1 && draggedIndex !== -1 && dropIndex >= draggedIndex) {
                dropIndex = dropIndex + 1;
            }
        }

        var sourceHelper = window.dragDrop.helpers[window.dragDrop.sourceCategory];
        if (sourceHelper) {
            sourceHelper.invokeMethodAsync('OnGlobalMouseMove', e.clientX, e.clientY, category, dropIndex);
        }
        if (category && category !== window.dragDrop.sourceCategory) {
            var targetHelper = window.dragDrop.helpers[category];
            if (targetHelper) {
                targetHelper.invokeMethodAsync('OnGlobalMouseMove', e.clientX, e.clientY, category, dropIndex);
            }
        }
    },

    globalMouseUpHandler: function (e) {
        if (!window.dragDrop.isDragging) return;

        var overlay = document.querySelector('.drag-overlay');
        if (overlay) overlay.style.display = 'none';
        var element = document.elementFromPoint(e.clientX, e.clientY);
        if (overlay) overlay.style.display = '';

        var column = element ? element.closest('.sprint-column-wrapper') : null;
        var category = column ? column.getAttribute('data-category') : null;

        var dropIndex = -1;
        if (column) {
            var tasks = column.querySelectorAll('.task-item:not(.task-item-dragging)');
            var dragged = column.querySelector('.task-item-dragging');
            var allTasks = column.querySelectorAll('.task-item');
            var draggedIndex = dragged ? Array.from(allTasks).indexOf(dragged) : -1;

            for (var i = 0; i < tasks.length; i++) {
                var rect = tasks[i].getBoundingClientRect();
                if (e.clientY < rect.top + rect.height / 2) {
                    dropIndex = i;
                    break;
                }
            }
            if (dropIndex === -1) dropIndex = tasks.length;

            if (dropIndex !== -1 && draggedIndex !== -1 && dropIndex >= draggedIndex) {
                dropIndex = dropIndex + 1;
            }
        }

        var sourceHelper = window.dragDrop.helpers[window.dragDrop.sourceCategory];
        if (sourceHelper) {
            sourceHelper.invokeMethodAsync('OnGlobalMouseUp', e.clientX, e.clientY, category, dropIndex);
        }
        if (category && category !== window.dragDrop.sourceCategory) {
            var targetHelper = window.dragDrop.helpers[category];
            if (targetHelper) {
                targetHelper.invokeMethodAsync('OnGlobalMouseUp', e.clientX, e.clientY, category, dropIndex);
            }
        }

        window.dragDrop.isDragging = false;
        window.dragDrop.sourceCategory = null;
    },

    preventSelection: function () {
        document.body.style.userSelect = 'none';
    },
    allowSelection: function () {
        document.body.style.userSelect = '';
    },
    removeGlobalMouseEvents: function () {
        document.removeEventListener('mousemove', this.globalMouseMoveHandler);
        document.removeEventListener('mouseup', this.globalMouseUpHandler);
        window.removeEventListener('mousemove', this.globalMouseMoveHandler);
        window.removeEventListener('mouseup', this.globalMouseUpHandler);
        this._eventsInitialized = false;
    },
};