
/* ============================================================
   BISLAT - WebJScript.js
   Shared JavaScript utilities for ASP.NET WebForms
   Compatible with Visual Studio 2013
   ============================================================ */

var BISLAT = (function () {
    'use strict';

    /* --------------------------------------------------------
       Element helpers
       -------------------------------------------------------- */

    function byId(id) {
        return document.getElementById(id);
    }

    function queryAll(selector, parent) {
        return (parent || document).querySelectorAll(selector);
    }

    function closestElement(element, selector) {
        while (element && element !== document) {
            if (element.matches && element.matches(selector)) {
                return element;
            }

            element = element.parentNode;
        }

        return null;
    }

    /* --------------------------------------------------------
       HTML encoding
       -------------------------------------------------------- */

    function htmlEncode(value) {
        if (value === null || typeof value === 'undefined') {
            return '';
        }

        return String(value)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    /* --------------------------------------------------------
       AJAX helper
       Sends JSON and returns a parsed JSON response.
       -------------------------------------------------------- */

    function ajax(url, method, data, callback) {
        var xhr = new XMLHttpRequest();

        xhr.open(method || 'GET', url, true);
        xhr.setRequestHeader(
            'Content-Type',
            'application/json; charset=utf-8'
        );

        xhr.onreadystatechange = function () {
            var response;

            if (xhr.readyState !== 4) {
                return;
            }

            try {
                response = xhr.responseText
                    ? JSON.parse(xhr.responseText)
                    : {};
            } catch (e) {
                response = {
                    error: xhr.responseText
                };
            }

            if (typeof callback === 'function') {
                callback(
                    xhr.status >= 200 && xhr.status < 300,
                    response,
                    xhr.status
                );
            }
        };

        if (data !== null && typeof data !== 'undefined') {
            xhr.send(JSON.stringify(data));
        } else {
            xhr.send(null);
        }
    }

    /* --------------------------------------------------------
       Modal helpers
       -------------------------------------------------------- */

    function openModal(id) {
        var modal = byId(id);

        if (modal) {
            modal.classList.add('show');
        }
    }

    function closeModal(id) {
        var modal = byId(id);

        if (modal) {
            modal.classList.remove('show');
        }
    }

    /* --------------------------------------------------------
       Public API
       -------------------------------------------------------- */

    return {
        byId: byId,
        queryAll: queryAll,
        closestElement: closestElement,
        htmlEncode: htmlEncode,
        ajax: ajax,
        openModal: openModal,
        closeModal: closeModal
    };
})();

/*
 * Backward-compatible global modal helpers.
 * Remove these globals if other pages already define them.
 */
window.openModal = BISLAT.openModal;
window.closeModal = BISLAT.closeModal;


//--------------EventListeners - UI/UX-------------------


document.addEventListener("DOMContentLoaded", function () {
    var sidebar = document.querySelector(".sidebar");
    var toggle = document.getElementById("sidebarToggle");

    if (!sidebar || !toggle) {
        return;
    }

    toggle.onclick = function () {
        sidebar.classList.toggle("collapsed");

        toggle.title = sidebar.classList.contains("collapsed")
            ? "פתח תפריט"
            : "סגור תפריט";
    };
});