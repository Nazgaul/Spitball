    mBox.factory('sQnA',
        ['$http',
         '$q',

        function ($http, $q) {
            var QnA = '/QnA';

            return {
                list: function (data) {
                    var dfd = $q.defer();
                    $http.get(QnA, { params: data }).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                }
            };
        }
        ]);
