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
            },
            searchByPage: function (data) {
                var dfd = $q.defer();
                $http.get(Search + 'Data', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            searchOtherUnis: function (data) {
                var dfd = $q.defer();
                $http.get(Search + 'OtherUniversities', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            }
            
        };
    }
    ]);
