
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

window.openUrlInNewTab = (url) => {
    if (!url) return false;
    try {
        // Use noopener,noreferrer to prevent reverse tabnabbing and avoid leaking referrer
        const newWin = window.open(url, '_blank', 'noopener,noreferrer');
        if (newWin) {
            // best-effort: detach opener and focus the new tab/window
            try { newWin.opener = null; } catch (e) { /* ignore */ }
            try { newWin.focus(); } catch (e) { /* ignore */ }
            return true;
        }
        return false;
    } catch (err) {
        console.error('openUrlInNewTab error:', err);
        return false;
    }
};