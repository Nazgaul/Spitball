"use strict";
var app;
(function (app) {
    "use strict";
    var InteractionDone = (function () {
        function InteractionDone($timeout) {
            var _this = this;
            this.$timeout = $timeout;
            this.restrict = "A";
            this.scope = true;
            this.link = function (scope, element) {
                var x;
                element.on("click keydown", function () {
                    _this.$timeout.cancel(x);
                    x = _this.$timeout(function () {
                        scope.$emit("update-model");
                    }, 2500);
                });
            };
        }
        InteractionDone.factory = function () {
            var directive = function ($timeout) {
                return new InteractionDone($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        };
        return InteractionDone;
    }());
    angular
        .module("app.flashcard")
        .directive("interactionDone", InteractionDone.factory());
})(app || (app = {}));
//# sourceMappingURL=interaction.directive.js.map