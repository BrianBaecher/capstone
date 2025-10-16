
window.scrollToElementById = (elementId) => {
    const el = document.getElementById(elementId);
    if (el) {
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};

window.domHelpers = {
    getClassList: function (element) {
        console.log(element);
        if (!element) return [];
        return Array.from(element.classList);
    },
    addClass: function (element, className) {
        if (!element) return;
        element.classList.add(className);
    },
    removeClass: function (element, className) {
        if (!element) return;
        element.classList.remove(className);
    },
    hasClass: function (element, className) {
        if (!element) return false;
        return element.classList.contains(className);
    }
};