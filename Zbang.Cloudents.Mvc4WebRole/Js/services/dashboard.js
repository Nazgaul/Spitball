//define('dashboard',['app'], function (app) {
mDashboard.factory('sDashboard',
    ['ajaxService',
     function (ajaxService) {
         var Item = '/Dashboard/';
         function buildPath(path) {
             return '/Dashboard/' + path + '/';
         }

         return {

             boxList: function () {
                 return ajaxService.get(buildPath('/BoxList/'));
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