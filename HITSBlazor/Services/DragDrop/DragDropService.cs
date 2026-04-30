using HITSTask = HITSBlazor.Models.Projects.Entities.Task;

namespace HITSBlazor.Services.DragDrop
{
    public class DragDropService
    {
        private HITSTask? _draggedTask;
        private string? _draggedFromCategory;

        public HITSTask? DraggedTask => _draggedTask;
        public string? DraggedFromCategory => _draggedFromCategory;
        public bool IsDragging => _draggedTask != null;

        public event Action? OnDragStateChanged;

        public void StartDrag(HITSTask task, string category)
        {
            _draggedTask = task;
            _draggedFromCategory = category;
            NotifyStateChanged();
        }

        public (HITSTask? task, string? category) EndDrag()
        {
            var result = (_draggedTask, _draggedFromCategory);
            _draggedTask = null;
            _draggedFromCategory = null;
            NotifyStateChanged();
            return result;
        }

        private void NotifyStateChanged()
        {
            OnDragStateChanged?.Invoke();
        }
    }
}
