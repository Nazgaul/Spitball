app.factory('sSearch',
    ['$http',
     '$q',
    function ($http, $q) {
        var Search = '/Search/';
        return {
            dropdown: function (data) {
                var dfd = $q.defer();
                $http.get(Search + 'Dropdown', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            }
        };
    }
    ]);
