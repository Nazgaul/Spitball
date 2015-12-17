(function () {
    angular.module('app.account').controller('AccountSettingsPasswordController', password);

    password.$inject = ['accountService', '$mdToast', '$document'];
    function password(accountService, $mdToast, $document) {
        var self = this;


        self.submit = function (myform) {
            accountService.updatePassword(self.old, self.new).then(function () {
                //alert('password change');
                reset(myform);
                self.old = '';
                self.new = '';
               
                $mdToast.show(
                    $mdToast.simple()
                    .textContent('password change')
                    .position('top')
                    .parent($document[0].querySelector('#accountPage'))
                    .hideDelay(1000));
            }, function (response) {
                myform.old.$error.server = true;
                self.error = response;
            });
        }
        self.reset = reset;

    

        function reset(myform) {
            myform.$setPristine();
            myform.$setUntouched();
        }
    }
})();