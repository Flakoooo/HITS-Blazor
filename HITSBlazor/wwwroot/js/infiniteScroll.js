let currentObserver = null;
let currentSentinel = null;

export function initializeInfiniteScroll(container, dotNetHelper) {
    if (!container || typeof container.querySelector !== 'function') {
        console.warn('initializeInfiniteScroll: container не является DOM-элементом', container);
        return;
    }

    stopInfiniteScroll();

    currentSentinel = document.createElement('div');
    currentSentinel.style.height = '1px';
    currentSentinel.style.width = '100%';
    currentSentinel.style.opacity = '0';
    currentSentinel.style.pointerEvents = 'none';

    let scrollContainer = container;
    const table = container.querySelector('table');

    if (table) {
        table.after(currentSentinel);
        scrollContainer = table.parentElement || container;
    } else {
        container.appendChild(currentSentinel);
    }

    console.log('Sentinel добавлен в DOM:', currentSentinel);
    console.log('Скролл-контейнер:', scrollContainer);

    let isLoading = false;

    currentObserver = new IntersectionObserver(
        (entries) => {
            const entry = entries[0];
            console.log('IntersectionObserver сработал. isIntersecting:', entry.isIntersecting, 'isLoading:', isLoading);

            if (entry.isIntersecting && !isLoading) {
                console.log('Начинаем загрузку данных...');
                isLoading = true;

                dotNetHelper.invokeMethodAsync('LoadMoreItems')
                    .then(() => {
                        console.log('LoadMoreItems успешно вызван');
                    })
                    .catch((error) => {
                        console.error('Ошибка вызова LoadMoreItems:', error);
                    })
                    .finally(() => {
                        setTimeout(() => {
                            isLoading = false;
                            console.log('Разблокировали загрузку');
                        }, 500);
                    });
            }
        },
        {
            root: scrollContainer,
            rootMargin: '0px 0px 200px 0px',
            threshold: 0.01
        }
    );

    currentObserver.observe(currentSentinel);
    console.log('IntersectionObserver создан и наблюдает за sentinel');
}

export function stopInfiniteScroll() {
    if (currentObserver) {
        currentObserver.disconnect();
        currentObserver = null;
        console.log('Observer остановлен');
    }
    if (currentSentinel) {
        currentSentinel.remove();
        currentSentinel = null;
        console.log('Sentinel удалён');
    }
}

export function dispose() {
    stopInfiniteScroll();
}