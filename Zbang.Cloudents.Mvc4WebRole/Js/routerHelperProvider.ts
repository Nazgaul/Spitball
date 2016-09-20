/// <reference path="../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../scripts/typings/angular-ui-router/angular-ui-router.d.ts" />
/// <reference path="shared/ajaxservice2.ts" />
'use strict';

declare var handleLanguage: any;
declare var version: any;
interface IRouterHelper {
    configureStates(states: Array<any>, otherwisePath?: string) : void;
    getStates(): any;
    buildUrl(path: string):string;
}
(() => {
    angular.module('app').provider('routerHelper', routerHelperProvider);

    routerHelperProvider.$inject = ['$locationProvider', '$stateProvider', '$urlRouterProvider'];

    /* @ngInject */
    function routerHelperProvider($locationProvider: angular.ILocationProvider, $stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider): angular.IServiceProvider {
        /* jshint validthis:true */
        this.$get = routerHelper;

        $locationProvider.html5Mode(true);

        routerHelper.$inject = ['$state'];

        /* @ngInject */
        function routerHelper($state) {
            var hasOtherwise = false;


            const service = {
                configureStates: configureStates,
                getStates: getStates,
                buildUrl: buildUrl
            };

            return service;


            function configureStates(states:Array<any>, otherwisePath:string) {
                states.forEach(state => {
                    if (!state.config.parent) {
                        state.config.parent = 'root';
                    }

                    
                    if (state.templateUrl) {
                        const template = [
                            'ajaxService2', (ajaxService2: app.IAjaxService2) => ajaxService2
                            .getHtml(state.templateUrl)
                        ];
                        if (state.config.views) {
                            state.config.views[''] = {
                                templateProvider: template,
                                controller: state.config.controller
                            };
                        } else {
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
            function buildUrl(path:string) {
                return path + '?lang=' + handleLanguage.getLangCookie() + '&version=' + version;
            }
        }

        return this;
    }
})();