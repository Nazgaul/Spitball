(function () {
    angular.module('app.user.account').controller('AccountSettingsController', account);
    account.$inject = ['accountService'];

    function account(accountService) {
        var self = this;


        accountService.getAccountDetails().then(function (response) {
            self.data = response;
        });
    }
})();






