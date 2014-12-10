app.filter('escapeHtmlChars',
    [function () {
        "use strict";
        return function (value) {
            if (!value) {
                return '';
            }
            return value
         .replace(/&/g, "&amp;")
         .replace(/</g, "&lt;")
         .replace(/>/g, "&gt;")
         .replace(/"/g, "&quot;")
         .replace(/'/g, "&#039;");
        };
    }
    ]);