var app;
(function (app) {
    "use strict;";
    var StartController = (function () {
        function StartController(startService) {
            this.startService = startService;
            console.log("jere");
        }
        StartController.prototype.intent = function () {
            var _this = this;
            this.startService.intent(this.term).then(function (response) {
                _this.lang = response.lang;
                _this.data = JSON.stringify(response.data);
                _this.xintent = response.intent;
            });
        };
        return StartController;
    }());
    StartController.$inject = ["startService"];
    angular.module("app").controller("StartController", StartController);
})(app || (app = {}));
//# sourceMappingURL=start.controller.js.map