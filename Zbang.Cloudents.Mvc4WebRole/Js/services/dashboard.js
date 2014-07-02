define('dashboard',['app'], function (app) {
    app.factory('Dashboard',
        ['$http',
         '$q',

         function ($http, $q) {
             return {
                 boxList: function () {
                     var dfd = $q.defer();
                     $http.get('/Dashboard/BoxList/').success(function (response) {
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