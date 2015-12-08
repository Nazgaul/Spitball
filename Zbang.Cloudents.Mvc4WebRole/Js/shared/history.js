(function () {
    angular.module('app').service('history', h);
    h.$inject = ['$state', '$stateParams', '$rootScope'];
    function h($state, $stateParams, $rootScope) {
        var self = this;
        self.arr = [];
        $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
            if (fromState.name === toState.name) {
                return;
            }
            // to be used for back button //won't work when page is reloaded.
            self.arr.push({
                name: fromState.name,
                params: fromParams

            });
        });

        //back button function called from back button's ng-click="back()"
        //$rootScope.back = function () {
        //    $state.go($rootScope.previousState_name, $rootScope.previousState_params);
        //};
    }
})();