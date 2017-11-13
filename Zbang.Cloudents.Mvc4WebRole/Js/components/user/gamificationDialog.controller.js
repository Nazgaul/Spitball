"use strict";
var app;
(function (app) {
    "use strict";
    var GamificationDialogController = (function () {
        function GamificationDialogController($mdDialog) {
            this.$mdDialog = $mdDialog;
        }
        GamificationDialogController.prototype.hide = function () {
            this.$mdDialog.hide();
        };
        return GamificationDialogController;
    }());
    GamificationDialogController.$inject = ['$mdDialog'];
    angular.module('app.user').controller('gamificationDialog', GamificationDialogController);
})(app || (app = {}));
//# sourceMappingURL=gamificationDialog.controller.js.map