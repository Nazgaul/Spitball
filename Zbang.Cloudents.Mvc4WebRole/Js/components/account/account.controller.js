(function () {
    angular.module('app.account').controller('AccountSettingsController', account);
    account.$inject = ['accountService', '$location', '$state'];

    function account(accountService, $location, $state) {
        var self = this;
        if (!$location.hash()) {
            $state.go('settings.profile');
        }

        accountService.getAccountDetails().then(function (response) {
            self.data = response;
        });
    }
})();








