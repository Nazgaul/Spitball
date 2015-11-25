(function () {
    angular
        .module('app')
        .provider('routerHelper', routerHelperProvider);

    routerHelperProvider.$inject = ['$locationProvider', '$stateProvider', '$urlRouterProvider'];

    /* @ngInject */
    function routerHelperProvider($locationProvider, $stateProvider, $urlRouterProvider) {
        /* jshint validthis:true */
        this.$get = routerHelper;

        $locationProvider.html5Mode(true);

        routerHelper.$inject = ['$state'];

        /* @ngInject */
        function routerHelper($state) {
            var hasOtherwise = false;
            //var universityRedirect = [
            //   'userDetails', '$state', function (userDetails, $state2) {
            //       if (!userDetails.get().university.id) {
            //           $state2.go('universityChoose');
            //           return;
            //       }
            //   }
            //];

            var service = {
                configureStates: configureStates,
                getStates: getStates,
                //universityRedirect: universityRedirect
                //buildUrl: buildUrl
            };

            return service;

            ///////////////

            function configureStates(states, otherwisePath) {
                states.forEach(function (state) {
                    if (!state.config.parent) {
                        state.config.parent = 'root';
                    }

                    if (state.templateUrl) {
                        state.config.templateUrl = function() {
                            return buildUrl(state.templateUrl);
                        };
                    }
                    $stateProvider.state(state.state, state.config);
                });
                if (otherwisePath && !hasOtherwise) {
                    hasOtherwise = true;
                    $urlRouterProvider.otherwise(otherwisePath);
                }
            }

            function getStates() { return $state.get(); }

            function buildUrl(path) {
                return path + '?lang=' + getCookie('l2') + '&version=' + window.version;

                function getCookie(cname) {
                    var name = cname + "=";
                    var ca = document.cookie.split(';');
                    for (var i = 0; i < ca.length; i++) {
                        var c = ca[i];
                        while (c.charAt(0) == ' ') c = c.substring(1);
                        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
                    }
                    return "";
                }
            }

           

        }
    }
})();