define('share', ['app'], function (app) {
    app.factory('Share',
        ['$http',
         '$q',

        function ($http, $q) {
            return {
                cloudentsFriends: function (data) {
                    var dfd = $q.defer();
                    $http.get('/User/Friends', { params: data }).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                googleFriends: function (data) {
                    var dfd = $q.defer();
                    $http.post('/User/GoogleContacts', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },               
                message: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Share/Message', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                }
            };
        }
    ]);
});