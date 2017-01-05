module app {
    "use strict";
    export interface IStartService {
        intent(term: string): angular.IPromise<any>;
    }
    class Start implements IStartService {
        static $inject = ["ajaxService2"];

        constructor(private ajaxService2: IAjaxService2) {
            
        }

        intent(term: string) {
            return this.ajaxService2.get("start/intent/", { term: term });

        }
    }
    angular.module("app").service("startService", Start);
}