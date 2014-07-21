app.factory('Shopping',
    ['$http',
     '$q',

    function ($http, $q) {
        var Shopping = '/Store/';
        return {
            products: function (data) {
                var dfd = $q.defer();                
                $http.get(Shopping + 'Products/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            }
        };
    }
    ]);
