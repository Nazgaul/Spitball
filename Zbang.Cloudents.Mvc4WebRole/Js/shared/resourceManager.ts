module app {
    "use strict";
    //declare var JsResources: any;
    export interface IResManager  {
        get(value: string): string;
    }
    class ResManager implements IResManager {
        static $inject = ["ajaxService2"];
        //private jsResources = window['JsResources'];
        constructor(private ajaxService: IAjaxService2) {
            return this;
        }

        get(value:string):string {
            const result = '';
            if (!value) {
                return result;
            }

            const resource = window["JsResources"][value];

            if (!resource) {
                this.ajaxService.logError('missing resource', value);
                return result;
            }

            return resource;
        }

        //TODO: duplicate
        
    }
    angular.module("app").service("resManager", ResManager);
}