(function() {
    angular.module('app').directive('animClass', animClass);
    animClass.$inject = ['$state'];

    function animClass($state) {
        return {
            link: function(scope, elm) {
                if ($state.current.data) {
                    var enterClass = $state.current.data.animateClass;

                    elm.addClass(enterClass);
                }
                scope.$on('$destroy', function() {
                    elm.removeClass(enterClass);
                    if ($state.current.data) {
                        elm.addClass($state.current.data.animateClass);
                    }
                });
            }
        }
    };
})();

(function() {
    angular.module('app').service('history', h);
    h.$inject = ['$state', '$stateParams', '$rootScope'];
    function h($state, $stateParams, $rootScope) {
        var self = this;
        self.arr = [];
        $rootScope.$on("$stateChangeSuccess", function(event, toState, toParams, fromState, fromParams) {
            // to be used for back button //won't work when page is reloaded.
            self.arr.push({
                name: fromState.name,
                params: fromParams

            });
            console.log(self.arr);
        });

        //back button function called from back button's ng-click="back()"
        //$rootScope.back = function () {
        //    $state.go($rootScope.previousState_name, $rootScope.previousState_params);
        //};
    }
})();