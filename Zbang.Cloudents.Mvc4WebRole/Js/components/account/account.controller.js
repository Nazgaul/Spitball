(function () {
    angular.module('app.account').controller('AccountSettingsController', account);
    account.$inject = [ '$location', '$state'];

    function account( $location, $state) {
        if (!$location.hash()) {
            $state.go('settings.profile');
        }

        //accountService.getAccountDetails().then(function (response) {
        //    self.data = response;
        //});
    }
})();








