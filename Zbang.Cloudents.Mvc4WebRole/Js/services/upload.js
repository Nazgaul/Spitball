define(['app'], function (app) {
    var Upload = '/Upload/';
    app.factory('Upload',
        ['$http',
         '$q',

        function ($http, $q) {
            return {
                'link': function (data) {
                    var dfd = $q.defer();
                    $http.post(Upload + 'Link/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                'dropbox': function (data) {
                    var dfd = $q.defer();
                    $http.post(Upload + 'Dropbox/', data).success(function (response) {
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