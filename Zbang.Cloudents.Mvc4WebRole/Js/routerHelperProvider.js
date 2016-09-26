/// <reference path="../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../scripts/typings/angular-ui-router/angular-ui-router.d.ts" />
/// <reference path="shared/ajaxservice2.ts" />
'use strict';
(function () {
    angular.module('app').provider('routerHelper', routerHelperProvider);
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
            //cookie in here
            function buildUrl(path) {
                return path + '?lang=' + handleLanguage.getLangCookie() + '&version=' + version;
            }
        }
        return this;
    }
})();
