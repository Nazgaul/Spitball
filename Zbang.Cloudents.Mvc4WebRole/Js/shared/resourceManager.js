var app;
(function (app) {
    "use strict";
    var ResManager = (function () {
        function ResManager(ajaxService) {
            this.ajaxService = ajaxService;
            return this;
        }
        ResManager.prototype.get = function (value) {
            var result = '';
            if (!value) {
                return result;
            }
            var resource = window["JsResources"][value];
            if (!resource) {
                this.ajaxService.logError("missing resource", value);
                return result;
            }
            return resource;
        };
        ResManager.$inject = ["ajaxService2"];
        return ResManager;
    }());
    angular.module("app").service("resManager", ResManager);
})(app || (app = {}));
