app.factory('Store',
    ['$http',
     '$q',

    function ($http, $q) {
        var Store = '/Store/';
        return {
            products: function (data) {
                var dfd = $q.defer();
                $http.get(Store + 'Products/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            search: function (data) {
                var dfd = $q.defer();
                $http.get(Store + 'Search/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            order: function (data) {
                var dfd = $q.defer();
                $http.post(Store + 'Order/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            validateCoupon: function(data) {
                var dfd = $q.defer(0);
                $http.get(Store + 'ValidCodeCoupon/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            }
        };
    }
    ]);
