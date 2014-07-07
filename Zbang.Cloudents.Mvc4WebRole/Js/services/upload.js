    mBox.factory('sUpload',
        ['$http',
         '$q',

        function ($http, $q) {
            var upload = '/Upload/';

            return {
                'link': function (data) {
                    var dfd = $q.defer();
                    $http.post(upload + 'Link/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                'dropbox': function (data) {
                    var dfd = $q.defer();
                    $http.post(upload + 'Dropbox/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                }
            };
        }
        ]);
