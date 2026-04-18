let currentObserver = null;
let currentSentinel = null;

export function initializeInfiniteScroll(container, dotNetHelper) {
    if (!container) return;

    if (currentSentinel) {
        currentSentinel.remove();
    }
    if (currentObserver) {
        currentObserver.disconnect();
    }

    currentSentinel = document.createElement('div');
    currentSentinel.style.height = '10px';
    currentSentinel.style.width = '100%';

    const table = container.querySelector('table');
    if (table && table.parentElement) {
        table.parentElement.appendChild(currentSentinel);
    } else {
        container.appendChild(currentSentinel);
    }

    let isLoading = false;

    currentObserver = new IntersectionObserver(
        async (entries) => {
            const entry = entries[0];

            if (entry.isIntersecting && !isLoading) {
                isLoading = true;

                try {
                    await dotNetHelper.invokeMethodAsync('LoadMoreItems');
                } catch (error) {
                    console.error('Error loading more items:', error);
                } finally {
                    setTimeout(() => {
                        isLoading = false;
                    }, 500);
                }
            }
        },
        {
            root: container,
            rootMargin: '200px',
            threshold: 0.1
        }
    );

    currentObserver.observe(currentSentinel);
}

export function stopInfiniteScroll() {
    if (currentObserver) {
        currentObserver.disconnect();
        currentObserver = null;
    }
    if (currentSentinel) {
        currentSentinel.remove();
        currentSentinel = null;
    }
}

export function dispose() {
    stopInfiniteScroll();
}