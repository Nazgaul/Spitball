﻿mItem.factory('sItem',
    ['$http',
     '$q',

    function ($http, $q) {
        var Item = '/Item/';
        function ajaxRequest(data, type, link) {
            var dfd = $q.defer();
            if (type === $http.get) {
                data = { params: data };
            }
            type(Item + link, data).success(function (response) {
                dfd.resolve(response);
            }).error(function (response) {
                dfd.reject(response);
            });
            return dfd.promise;
        }
        return {
            'delete': function (data) {
                var dfd = $q.defer();
                $http.post(Item + 'Delete/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            //nav : function (data) {
            //    return ajaxRequest(data, $http.get, 'Navigation/');
            //},
            load: function (data) {
                return ajaxRequest(data, $http.get, 'Load/');
            },
            preview:function(data) {
                return ajaxRequest(data, $http.get, 'Preview/');
            }
        };
    }
    ]);
