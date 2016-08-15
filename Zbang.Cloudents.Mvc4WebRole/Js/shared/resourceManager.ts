
module app {
    'use strict';
    declare var JsResources: any;

    export interface IResManager  {
        get(value: string): string;
    }
    class ResManager implements IResManager {
        static $inject = ['$location','ajaxService2'];
        private jsResources = JsResources;
        constructor(private $location: angular.ILocationService, private ajaxService: IAjaxService2) {
            return this;
        }

        get(value:string):string {
            const result = '';
            if (!value) {
                return result;
            }

            const resource = this.jsResources[value];

            if (!resource) {
                this.ajaxService.logError('missing resource', value);
                return result;
            }

            return resource;
        }

        //TODO: duplicate
        
    }
    angular.module('app').service('resManager', ResManager);
}