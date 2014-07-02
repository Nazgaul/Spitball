define('qna', ['app'], function (app) {
    var QnA = '/QnA';
    app.factory('QnA',
        ['$http',
         '$q',

        function ($http, $q) {
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
});