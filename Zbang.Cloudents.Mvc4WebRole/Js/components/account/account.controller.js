(function () {
    angular.module('app.account').controller('AccountSettingsController', account);
    account.$inject = ['$stateParams', '$state'];

    function account($stateParams, $state) {
        
        if ($state.current.name === 'settings') {
            $state.go('settings.profile', $stateParams, { location: "replace" });
        }
      
        this.isActiveState = isActiveState;

        function isActiveState(state) {
            return state === $state.current.name;
        }
        //accountService.getAccountDetails().then(function (response) {
        //    self.data = response;
        //});
    }
})();








