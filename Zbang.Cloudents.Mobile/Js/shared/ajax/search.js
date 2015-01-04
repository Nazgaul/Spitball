﻿angular.module('ajax').factory('search',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/search/' + path + '/';
        }
        return {
            boxes: function (data) {
                return ajaxService.get(buildPath('boxes'), data);
            },
            items: function (data) {
                return ajaxService.get(buildPath('items'), data);
            }//,
            //members: function (data) {
            //    return ajaxService.post(buildPath('members'), data);
            //}
        };


    }]
);