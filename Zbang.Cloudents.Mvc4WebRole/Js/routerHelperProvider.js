'use strict';
(function () {
    angular.module('app').provider('routerHelper', routerHelperProvider);
    routerHelperProvider.$inject = ['$locationProvider', '$stateProvider', '$urlRouterProvider'];
    function routerHelperProvider($locationProvider, $stateProvider, $urlRouterProvider) {
        this.$get = routerHelper;
        $locationProvider.html5Mode(true);
        routerHelper.$inject = ['$state'];
        function routerHelper($state) {
            var hasOtherwise = false;
            var service = {
                configureStates: configureStates,
                getStates: getStates,
                buildUrl: buildUrl
            };
            return service;
            function configureStates(states, otherwisePath) {
                states.forEach(function (state) {
                    if (!state.config.parent) {
                        state.config.parent = 'root';
                    }
                    if (state.templateUrl) {
                        var template = [
                            'ajaxService2', function (ajaxService2) { return ajaxService2
                                .getHtml(state.templateUrl); }
                        ];
                        if (state.config.views) {
                            state.config.views[''] = {
                                templateProvider: template,
                                controller: state.config.controller
                            };
                        }
                        else {
                            state.config.templateProvider = template;
                        }
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
                return path + '?lang=' + handleLanguage.getLangCookie() + '&version=' + version;
            }
        }
        return this;
    }
})();
