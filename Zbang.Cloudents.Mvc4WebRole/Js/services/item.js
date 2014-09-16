mItem.factory('sItem',
    ['$http',
     '$q',

    function ($http, $q) {
        var Item = '/Item/';

        return {

            'delete': function (data) {
                var dfd = $q.defer();
                $http.post(Item + 'Delete/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            }
        };
    }
    ]);
