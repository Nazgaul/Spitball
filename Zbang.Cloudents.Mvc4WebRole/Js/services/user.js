//// Define you service here. Services can be added to same module as 'main' or a seperate module can be created.
//angular.module('apiService', ['jmdobry.angular-cache']).config(['$angularCacheFactoryProvider', function ($angularCacheFactoryProvider) {
//    $angularCacheFactoryProvider.setCacheDefaults({
//        maxAge: 300000,
//        deleteOnExpire: 'aggressive',
//        recycleFreq: 60000,
//        storageMode: 'sessionStorage'
//    });
//}]).
//  
//define('user', ['app'], function (app) {
    mUser.factory('sUser', [
        '$http',
        '$q',
        function ($http, $q) {
            return {
                friends: function (data) {
                    var dfd = $q.defer();
                    $http.get('/User/Friends/', { params: data }).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;
                },
                minProfile: function (data) {
                    var dfd = $q.defer();
                    $http.get('/User/MinProfile/', { params: data }).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;
                },
            };
        }
    ]);
//});
