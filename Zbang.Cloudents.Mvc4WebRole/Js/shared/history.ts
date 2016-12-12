

module app {
    "use strict";

    interface ISbHistoryObject {
        name: string;
        params: angular.ui.IStateParamsService;
       // pageYOffset: number;

    }

    export interface ISbHistory {
        popElement(): ISbHistoryObject;
        firstState(): boolean;
    }

    class SbHistory implements ISbHistory {
        static $inject = ["$rootScope", "$window"];

        pageYOffset: number;
        //url: string;
        skipState = false;
        arr: Array<ISbHistoryObject> = [];
        constructor(private $rootScope: angular.IRootScopeService,
            //private $location: angular.ILocationService,
            private $window: angular.IWindowService) {

            this.$rootScope.$on('$stateChangeStart', (event: angular.IAngularEvent) => {
                this.pageYOffset = $window.pageYOffset;
            });
            this.$rootScope.$on('$stateChangeSuccess',
                (event, toState: angular.ui.IState, toParams: any, fromState: angular.ui.IState, fromParams: angular.ui.IStateParamsService) => {
                    if (fromState.name === toState.name) {

                        return;
                    }

                    if (toParams.fromBack) {
                        return;
                    }
                    if (this.skipState) {
                        this.skipState = false;
                        return;
                    }
                    // to be used for back button //won't work when page is reloaded.
                    this.arr.push({
                        name: fromState.name,
                        params: angular.extend({}, fromParams, { pageYOffset: this.pageYOffset })
                       

                    });
                    this.pageYOffset = 0;
                });
            this.$rootScope.$on('from-back', () => {
                this.skipState = true;
            });
        }

        popElement = () => {
            if (this.arr.length === 1) {
                return;
            }
            return this.arr.pop();
        }
        firstState = () => {
            return this.arr.length === 0;
        }
        
    }
    angular.module('app').service('sbHistory', SbHistory);
}