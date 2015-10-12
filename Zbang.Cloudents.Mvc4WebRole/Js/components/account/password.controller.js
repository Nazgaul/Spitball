(function () {
    angular.module('app.account').controller('AccountSettingsPasswordController', password);

    password.$inject = ['accountService'];
    function password(accountService) {
        var self = this;


        self.submit = function () {
            accountService.updatePassword(self.old, self.new).then(function() {
                alert('password change');
                self.old = '';
                self.new = '';
            });
        }

    }
})();