//define('dashboard',['app'], function (app) {
    mDashboard.factory('sDashboard',
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
                 },
                 recommendedCourses: function () {
                     var dfd = $q.defer();
                     $http.get('/Dashboard/RecommendedCourses/').success(function (response) {
                         dfd.resolve(response);
                     }).error(function (response) {
                         dfd.reject(response);
                     });
                     return dfd.promise;
                 }
             };
         }
    ]);
//});