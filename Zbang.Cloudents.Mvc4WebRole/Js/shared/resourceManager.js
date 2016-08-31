var app;
(function (app) {
    'use strict';
    var ResManager = (function () {
        //private jsResources = window['JsResources'];
        function ResManager($location, ajaxService) {
            this.$location = $location;
            this.ajaxService = ajaxService;
            return this;
        }
        ResManager.prototype.get = function (value) {
            var result = '';
            if (!value) {
                return result;
            }
            var resource = window['JsResources'];
            if (!resource) {
                this.ajaxService.logError('missing resource', value);
                return result;
            }
            return resource;
        };
        ResManager.$inject = ['$location', 'ajaxService2'];
        return ResManager;
    }());
    angular.module('app').service('resManager', ResManager);
})(app || (app = {}));
//# sourceMappingURL=resourceManager.js.map