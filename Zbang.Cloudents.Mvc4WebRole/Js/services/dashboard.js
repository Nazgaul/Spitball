//define('dashboard',['app'], function (app) {
mDashboard.factory('sDashboard',
    ['$http',
     '$q',
     function ($http, $q) {
         function ajaxRequest(type, link, data) {
             var dfd = $q.defer();
             if (type === $http.get && data) {
                 data = { params: data };
             }
             type(link, data).success(function (response) {
                 dfd.resolve(response);
             }).error(function (response) {
                 dfd.reject(response);
             });
             return dfd.promise;
         }
         return {
             boxList: function () {
                 return ajaxRequest($http.get, '/Dashboard/BoxList/');
             },
             recommendedCourses: function () {
                 return ajaxRequest($http.get, '/Dashboard/RecommendedCourses/');
             },
             disableFirstTime: function () {
                 return ajaxRequest($http.post, '/Account/FirstTime/', { firstTime: 'Dashboard' });
             }
         }
     }
    ]);
//});