'use strict';
(function () {
    angular.module('app').factory('resManager', resManager);
    resManager.$inject = ['$location'];
    function resManager($location) {
        var jsResources = window.JsResources;

        function get(value) {
            var result = '';
            if (!value) {
                return result;
            }

            var resource = jsResources[value];

            if (!resource) {
                logError('missing resource', value);
                return result;
            }

            return resource;
        }

        return {
            get: get
        };

        //TODO: duplicate
        function logError(cause, value, params) {
            $.ajax({
                type: 'POST',
                url: '/Error/JsLog',
                contentType: 'application/json',
                data: angular.toJson({
                    errorUrl: $location.absUrl(),
                    errorMessage: value + ';' + params,
                    cause: 'jsResources ' + cause,
                    stackTrace: ''
                })
            });
        }
    }

})()