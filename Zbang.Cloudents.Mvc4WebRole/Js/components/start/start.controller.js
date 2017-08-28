"use strict";
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
                //if (!response.data) {
                //    this.sentence = "I don't understand what you want";
                //    return;
                //}
                //this.sentence = response.sentence;
                //this.term = "";
                _this.sentence = JSON.stringify(response, null, "\t");
            });
        };
        //xintent;
        //data;
        StartController.$inject = ["startService"];
        return StartController;
    }());
    angular.module("app").controller("StartController", StartController);
})(app || (app = {}));
//# sourceMappingURL=start.controller.js.map