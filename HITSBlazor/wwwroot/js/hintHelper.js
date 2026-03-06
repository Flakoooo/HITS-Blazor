window.hintHelper = {
    getBoundingClientRect: function (element) {
        const rect = element.getBoundingClientRect();
        return {
            left: rect.left,
            top: rect.top,
            width: rect.width,
            height: rect.height,
            right: rect.right,
            bottom: rect.bottom
        };
    },

    getWindowHeight: function () {
        const height = window.innerHeight;
        return height;
    },

    getWindowWidth: function () {
        const width = window.innerWidth;
        return width;
    }
};