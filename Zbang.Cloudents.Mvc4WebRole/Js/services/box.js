define(['app'], function (app) {

    app.factory('Box',
        ['$http',
         '$q',

        function ($http, $q) {
            return {
                create: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Dashboard/Create/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                remove: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Box/Delete2/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;
                },
                update: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Box/UpdateInfo/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                items: function (data) {
                    var dfd = $q.defer();
                    $http.get('/Box/Items/', { params: data }).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                info: function (data) {
                    var dfd = $q.defer();
                    $http.get('/Box/Data/', { params: data }).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                QnA: {
                    list: function (data) {
                        var dfd = $q.defer();
                        $http.get('/QnA/', { params: data }).success(function (response) {
                            dfd.resolve(response);
                        }).error(function (response) {
                            dfd.reject(response);
                        });

                        return dfd.promise;
                    }
                }
            };
        }
    ]);
});