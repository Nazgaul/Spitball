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
