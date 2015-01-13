angular.module('app.resources', []).
    service('resService', ['$filter', function ($filter) {
        'use strict';
        var service = this;

        service.translate = getString();

        service.translateFormat = formatString;

        function getString(value) {
            if (angular.isUndefined(value)) {
                return '';
            }
            var translation = window.JsResources[value];

            if (angular.isUndefined(translation)) {
                return '';
            }

            return translation;
        }

        function formatString(key) {
            var format = getString(key);
            var args = Array.prototype.slice.call(arguments, 1);
            return format.replace(/{(\d+)}/g, function (match, number) {
                return typeof args[number] != 'undefined'
                  ? args[number]
                  : match
                ;
            });
        }

    }]);