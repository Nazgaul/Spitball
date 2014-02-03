(function (mmc, $) {
    "use strict";
    var handlers = {};
    mmc.updateHandler = function (handler, updatefunction) {
        if (handler in handlers) {
            throw 'handler exists';
        }
        handlers[handler] = updatefunction;
    };
    mmc.doUpdate = function (handler, data) {
        if (handler in handlers) {
            handlers[handler](data);
        }
    };
}(window.mmc = window.mmc || {}, jQuery));