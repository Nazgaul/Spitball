(function () {
    'use strict';
    angular.module('app.account').controller('AccountSettingsPasswordController', password);

    password.$inject = ['accountService', '$mdToast', '$document', '$scope', 'resManager', 'showToasterService'];
    function password(accountService, $mdToast, $document, $scope, resManager, showToasterService) {
        var self = this;
        self.submit = function (myform) {
            accountService.updatePassword(self.old, self.new).then(function () {
                cancel(myform);
                showToasterService.showToaster(resManager.get('passwordChangeSuccess'), 'accountPage');
            }, function (response) {
                myform.old.$setValidity('server',false);
                self.error = response;
            });
        }
        self.cancel = cancel;

        function cancel(myform) {
            self.old = '';
            self.new = '';
            myform.$setPristine();
            myform.$setUntouched();
        }
    }
})();