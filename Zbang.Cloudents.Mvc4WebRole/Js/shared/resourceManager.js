var app;
(function (app) {
    'use strict';
    var ResManager = (function () {
        function ResManager($location) {
            this.$location = $location;
            this.jsResources = JsResources;
        }
        ResManager.prototype.get = function (value) {
            var result = '';
            if (!value) {
                return result;
            }
            var resource = this.jsResources[value];
            if (!resource) {
                this.logError('missing resource', value);
                return result;
            }
            return resource;
        };
        ResManager.prototype.logError = function (cause, value) {
            $.ajax({
                type: 'POST',
                url: '/Error/JsLog',
                contentType: 'application/json',
                data: angular.toJson({
                    errorUrl: this.$location.absUrl(),
                    errorMessage: value,
                    cause: "jsResources " + cause,
                    stackTrace: ''
                })
            });
        };
        ResManager.$inject = ['$location'];
        return ResManager;
    }());
    angular.module('app').factory('resManager', ResManager);
})(app || (app = {}));
