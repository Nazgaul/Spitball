define('item',['app'], function (app) {
    var Item = '/Item/';
    app.factory('Item',
        ['$http',
         '$q',

        function ($http, $q) {
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
});