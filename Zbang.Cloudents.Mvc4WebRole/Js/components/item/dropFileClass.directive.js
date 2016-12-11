var app;
(function (app) {
    "use strict";
    var dragTimer;
    var DropFileClass = (function () {
        function DropFileClass($timeout) {
            var _this = this;
            this.$timeout = $timeout;
            this.link = function (scope, element) {
                $(document).on("dragover", function (e) {
                    var dt = e.originalEvent.dataTransfer;
                    if (dt.types && (dt.types.indexOf ? dt.types.indexOf('Files') !== -1 : dt.types.contains('Files'))) {
                        element.addClass("drop");
                        _this.$timeout.cancel(dragTimer);
                    }
                })
                    .on('dragleave', function (e) {
                    dragTimer = _this.$timeout(function () {
                        element.removeClass("drop");
                    }, 25);
                });
            };
        }
        DropFileClass.factory = function () {
            var directive = function ($timeout) {
                return new DropFileClass($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        };
        return DropFileClass;
    }());
    angular
        .module("app.item")
        .directive("dropFile", DropFileClass.factory());
})(app || (app = {}));
//# sourceMappingURL=dropFileClass.directive.js.map