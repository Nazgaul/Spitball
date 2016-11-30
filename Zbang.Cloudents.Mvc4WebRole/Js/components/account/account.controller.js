(function () {
    'use strict';
    angular.module('app.account').controller('AccountSettingsController', account);
    account.$inject = ['$stateParams', '$state', 'userData'];

    function account($stateParams, $state, userData) {
        var self = this;

        self.canChangePassword = userData.system;
        //if ($state.current.name === 'settings') {
        //    $state.go('settings.profile', $stateParams, { location: "replace", notify: false });
        //}
      
        self.isActiveState = isActiveState;
        self.needChangePassword = userData.system;
        function isActiveState(state) {
            return state === $state.current.name;
        }
        //accountService.getAccountDetails().then(function (response) {
        //    self.data = response;
        //});
    }
})();








