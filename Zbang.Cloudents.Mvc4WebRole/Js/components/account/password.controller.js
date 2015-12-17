(function () {
    angular.module('app.account').controller('AccountSettingsPasswordController', password);

    password.$inject = ['accountService', '$mdToast', '$document', '$scope'];
    function password(accountService, $mdToast, $document, $scope) {
        var self = this;


        self.submit = function (myform) {
            accountService.updatePassword(self.old, self.new).then(function () {
         
                self.old = '';
                self.new = '';
                $scope.app.resetForm(myform);
                $scope.app.showToaster('password change', 'accountPage');
            }, function (response) {
                myform.old.$setValidity('server',false);
                self.error = response;
            });
        }

        self.cancel = cancel;

        function cancel(myform) {
            self.old = '';
            self.new = '';
            $scope.app.resetForm(myform);
        }
    }
})();