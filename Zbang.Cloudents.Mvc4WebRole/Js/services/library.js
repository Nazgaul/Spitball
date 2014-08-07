mLibrary.factory('sLibrary', [
    '$http',
    '$q',
    function ($http, $q) {
        var Library = '/Library/';
        return {
            items: function (data) {
                var dfd = $q.defer();
                $http.get(Library + 'Nodes/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            box: {
                create: function (data) {
                    var dfd = $q.defer();
                    $http.post(Library + '/CreateBox', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;
                },                
            },
            department: {
                create: function (data) {
                    var dfd = $q.defer();
                    $http.post(Library + '/Create', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;
                },
                'delete': function (data) {
                    var dfd = $q.defer();
                    $http.post(Library + 'DeleteNode/', data).success(function (response) {
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
//});
