
module app {
    'use strict';
    declare var JsResources: any;

    export interface IResManager  {
        get(value: string): string;
    }
    class ResManager implements IResManager {
        static $inject = ['$location'];
        private jsResources = JsResources;
        constructor(private $location: angular.ILocationService) {
            return this;
        }

        get(value:string):string {
            const result = '';
            if (!value) {
                return result;
            }

            const resource = this.jsResources[value];

            if (!resource) {
                this.logError('missing resource', value);
                return result;
            }

            return resource;
        }

        //TODO: duplicate
        private logError(cause, value) {
            $.ajax({
                type: 'POST',
                url: '/Error/JsLog',
                contentType: 'application/json',
                data: angular.toJson({
                    errorUrl: this.$location.absUrl(),
                    errorMessage: value,
                    cause: `jsResources ${cause}`,
                    stackTrace: ''
                })
            });
        }
    }
    angular.module('app').service('resManager', ResManager);
}

//(function () {
//    angular.module('app').factory('resManager', resManager);
//    resManager.$inject = ['$location'];
//    function resManager($location) {
//        var jsResources = window.JsResources;

//        function get(value) {
//            var result = '';
//            if (!value) {
//                return result;
//            }

//            var resource = jsResources[value];

//            if (!resource) {
//                logError('missing resource', value);
//                return result;
//            }

//            return resource;
//        }

//        return {
//            get: get
//        };


//    }

//})()