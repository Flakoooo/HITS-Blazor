using Microsoft.JSInterop;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;
using SharpTask = System.Threading.Tasks.Task;


namespace HITSBlazor.Services.DragAndDrop
{
    public class DragDropService
    {
        private IJSRuntime? _jsRuntime;

        public HITSTask? DraggedTask { get; private set; }
        public string? DraggedFromCategory { get; private set; }
        public string? TargetCategory { get; private set; }
        public int TargetDropIndex { get; private set; } = -1;
        public double MouseX { get; private set; }
        public double MouseY { get; private set; }
        public bool IsDragging => DraggedTask != null;

        public string? LastTempCategory { get; set; }

        public event Action? OnDragStateChanged;
        public event Action? OnCleanupTempTask;

        public event Action? OnOverlayNeedsUpdate;
        private DateTime _lastOverlayUpdate = DateTime.MinValue;

        private long _lastRenderTicks;

        public void SetJSRuntime(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

        public void UpdateMouseMove(double clientX, double clientY, string? targetCategory, int dropIndex)
        {
            MouseX = clientX;
            MouseY = clientY;
            TargetCategory = targetCategory;
            TargetDropIndex = dropIndex;
        }

        public void UpdateOverlayIfNeeded()
        {
            var now = DateTime.Now;
            if ((now - _lastOverlayUpdate).TotalMilliseconds < 30) return;
            _lastOverlayUpdate = now;
            OnOverlayNeedsUpdate?.Invoke();
        }

        public async SharpTask StartDrag(HITSTask task, string fromCategory)
        {
            DraggedTask = task;
            DraggedFromCategory = fromCategory;

            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("dragDrop.preventSelection");
                await _jsRuntime.InvokeVoidAsync("dragDrop.startGlobalDrag", fromCategory);
            }

            OnDragStateChanged?.Invoke();
        }

        public async SharpTask EndDrag()
        {
            OnCleanupTempTask?.Invoke();
            LastTempCategory = null;
            DraggedTask = null;
            DraggedFromCategory = null;

            if (_jsRuntime != null)
                await _jsRuntime.InvokeVoidAsync("dragDrop.allowSelection");

            OnDragStateChanged?.Invoke();
        }

        public void CleanupTempTask(Func<bool> hasTask, Action removeTask, string taskCategory, HITSTaskStatus currentStatus)
        {
            if (DraggedTask is not null && hasTask()
                && taskCategory != DraggedFromCategory
            ) removeTask();
        }

        public void NotifyStateChanged()
        {
            var now = Environment.TickCount64;

            if (now - _lastRenderTicks < 16)
                return;

            _lastRenderTicks = now;

            OnDragStateChanged?.Invoke();
        }

        public void NotifyCleanUp() => OnCleanupTempTask?.Invoke();
    }
}
