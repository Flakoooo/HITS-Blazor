using HITSBlazor.Components.Typography;
using KristofferStrube.Blazor.Popper;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Hint
{
    public partial class Hint : IAsyncDisposable
    {
        [Inject]
        protected Popper PopperService { get; set; } = null!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public string IconClass { get; set; } = string.Empty;

        [Parameter]
        public TextColor IconColor { get; set; } = TextColor.Primary;

        [Parameter]
        public Placement Placement { get; set; } = Placement.Top;

        [Parameter]
        public Strategy StrategyPosition { get; set; } = Strategy.Fixed;

        private ElementReference _triggerRef;
        private ElementReference _tooltipRef;
        private Instance? _popperInstance;
        private bool _isVisible;
        private bool _isDisposed;
        private bool _isReady;
        private CancellationTokenSource? _cts;

        private static readonly Modifier[] HintModifiers =
        [
            new(ModifierName.Offset)
            {
                Options = new { offset = new[] { 0, 8 } }
            },
            new(ModifierName.PreventOverflow)
            {
                Options = new
                {
                    padding = 8,
                    altAxis = true
                }
            },
            new(ModifierName.Flip)
            {
                Options = new
                {
                    fallbackPlacements = new[]
                    {
                        Placement.Top, Placement.Bottom,
                        Placement.Left, Placement.Right
                    },
                    padding = 8
                }
            }
        ];

        private string GetIconClass()
        {
            var classes = new List<string> { $"text-{IconColor.ToString().ToLower()}" };
            if (!string.IsNullOrWhiteSpace(IconClass))
                classes.Add(IconClass);
            return string.Join(" ", classes);
        }

        private async Task ShowTooltip()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            if (_isDisposed) return;

            _isVisible = true;
            _isReady = false;
            StateHasChanged();

            try
            {
                await Task.Delay(16, token);
                if (token.IsCancellationRequested) return;

                await CreatePopper();
                if (token.IsCancellationRequested)
                {
                    await DestroyPopper();
                    _isVisible = false;
                    StateHasChanged();
                    return;
                }

                await Task.Delay(20, token);
                if (token.IsCancellationRequested)
                {
                    _isReady = false;
                    StateHasChanged();
                    await Task.Delay(150, CancellationToken.None);
                    await DestroyPopper();
                    _isVisible = false;
                    StateHasChanged();
                    return;
                }

                _isReady = true;
                StateHasChanged();
            }
            catch (OperationCanceledException) { }
        }

        private async Task HideTooltip()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            if (!_isVisible || _isDisposed) return;

            _isReady = false;
            StateHasChanged();

            await Task.Delay(150);

            if (_cts == null)
            {
                await DestroyPopper();
                _isVisible = false;
                StateHasChanged();
            }
        }

        private async Task CreatePopper()
        {
            await DestroyPopper();

            try
            {
                _popperInstance = await PopperService.CreatePopperAsync(
                    reference: _triggerRef,
                    popper: _tooltipRef,
                    options: new Options
                    {
                        Placement = Placement,
                        Strategy = StrategyPosition,
                        Modifiers = HintModifiers
                    }
                );

                await _popperInstance.Update();
            }
            catch (Exception) { }
        }

        private async Task DestroyPopper()
        {
            if (_popperInstance != null)
            {
                try
                {
                    await _popperInstance.Destroy();
                }
                catch { }
                _popperInstance = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            await DestroyPopper();
        }
    }
}
