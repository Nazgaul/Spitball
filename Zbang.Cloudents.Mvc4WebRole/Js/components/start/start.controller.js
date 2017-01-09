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
                _this.luis = {};
                _this.wit = {};
                _this.luis.data = response.luis;
                _this.luis.intent = response.luisIntent;
                _this.wit.data = response.wit;
                _this.wit.intent = response.witIntent;
            });
        };
        return StartController;
    }());
    StartController.$inject = ["startService"];
    angular.module("app").controller("StartController", StartController);
})(app || (app = {}));
//# sourceMappingURL=start.controller.js.map