var app;
(function (app) {
    "use strict;";
    var StartController = (function () {
        function StartController(startService) {
            this.startService = startService;
            this.loading = false;
        }
        StartController.prototype.intent = function () {
            var _this = this;
            this.loading = true;
            this.startService.intent(this.term).then(function (response) {
                _this.loading = false;
                if (!response.data) {
                    _this.sentence = "I don't understand what you want";
                    return;
                }
                _this.sentence = response.sentence;
                _this.term = "";
            });
        };
        return StartController;
    }());
    StartController.$inject = ["startService"];
    angular.module("app").controller("StartController", StartController);
})(app || (app = {}));
//# sourceMappingURL=start.controller.js.map