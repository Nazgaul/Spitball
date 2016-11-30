module app {
    "use strict";

    class GamificationDialogController {
        static $inject = ['$mdDialog'];
        constructor(private $mdDialog: angular.material.IDialogService) {
        }

        hide() {
            this.$mdDialog.hide();
        }
    }

    angular.module('app.user').controller('gamificationDialog', GamificationDialogController);
}