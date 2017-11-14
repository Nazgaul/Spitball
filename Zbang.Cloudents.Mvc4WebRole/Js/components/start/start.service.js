"use strict";
var app;
(function (app) {
    "use strict";
    var Start = (function () {
        function Start(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        Start.prototype.intent = function (term) {
            return this.ajaxService2.get("start/intent/", { term: term });
        };
        return Start;
    }());
    Start.$inject = ["ajaxService2"];
    angular.module("app").service("startService", Start);
})(app || (app = {}));
//# sourceMappingURL=start.service.js.map