export function initializeInfiniteScroll(container, dotNetHelper) {
    if (!container) return;

    const sentinel = document.createElement('div');
    sentinel.style.height = '10px';
    sentinel.style.width = '100%';
    sentinel.style.marginTop = '-10px';

    const table = container.querySelector('table');
    if (table && table.parentElement) {
        table.parentElement.appendChild(sentinel);
    } else {
        container.appendChild(sentinel);
    }

    let isLoading = false;
    let hasMorePages = true;

    const observer = new IntersectionObserver(
        async (entries) => {
            const entry = entries[0];

            if (entry.isIntersecting && !isLoading && hasMorePages) {
                isLoading = true;

                try {
                    await dotNetHelper.invokeMethodAsync('LoadMoreItems');
                } catch (error) {
                    console.error('Error loading more items:', error);
                } finally {
                    isLoading = false;
                }
            }
        },
        {
            root: container,
            rootMargin: '200px',
            threshold: 0.1
        }
    );

    observer.observe(sentinel);

    return {
        dispose: () => {
            observer.disconnect();
            sentinel.remove();
        }
    };
}