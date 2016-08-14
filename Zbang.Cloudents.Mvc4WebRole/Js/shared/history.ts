

module app {
    'use strict';

    interface ISbHistoryObject {
        name: string;
        params: angular.ui.IStateParamsService;

    }

    export interface ISbHistory {
        popElement(): ISbHistoryObject;
        firstState(): boolean;
    }

    class SbHistory implements ISbHistory {
        static $inject = ['$rootScope', '$location'];

        url: string;
        skipState = false;
        arr: Array<ISbHistoryObject> = [];
        constructor(private $rootScope: angular.IRootScopeService, private $location: angular.ILocationService) {

            this.$rootScope.$on('$stateChangeStart', () => {
                this.url = this.$location.url();
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
                        params: fromParams

                    });
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





//(function () {
//    angular.module('app').service('history', h);
//    h.$inject = ['$rootScope', '$location'];
//    function h($rootScope, $location) {
//        var self = this, arr = [], skipState;
//        //self.arr = [];
//        var url;
//        $rootScope.$on('$stateChangeStart', () => {
//            url = $location.url();
//        });
//        $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
//            if (fromState.name === toState.name) {
//                return;
//            }
//            if (toParams.fromBack) {
//                return;
//            }
//            if (skipState) {
//                skipState = false;
//                return;
//            }
//            // to be used for back button //won't work when page is reloaded.
//            arr.push({
//                name: fromState.name,
//                params: fromParams

//            });
//            //arr.push($location.url());
//        });

//        $rootScope.$on('from-back', function () {
//            skipState = true;
//        });

//        //self.pushState = function () {
//        //    arr.push($location.url());
//        //}

//        self.popElement = function () {
//            if (arr.length === 1) {
//                return;
//            }
//            return arr.pop();
//        }
//        self.firstState = function () {
//            return arr.length === 0;
//        }

//    }
//})();