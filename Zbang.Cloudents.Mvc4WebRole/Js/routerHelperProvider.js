﻿'use strict';
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
                        //state.config.templateUrl = function() {
                        //    return buildUrl(state.templateUrl);
                        //};
                        state.config.templateProvider = [
                            'ajaxService2', function (ajaxService2) {
                                return ajaxService2.getHtml(state.templateUrl);
                            }
                        ];
                    }
                    $stateProvider.state(state.state, state.config);
                });
                if (otherwisePath && !hasOtherwise) {
                    hasOtherwise = true;
                    $urlRouterProvider.otherwise(otherwisePath);
                }
            }

            function getStates() { return $state.get(); }

            //cookie in here
            function buildUrl(path) {
                return path + '?lang=' + handleLanguage.getLangCookie() + '&version=' + window.version;
            }
        }
    }
})();