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
                 return ajaxService.get(buildPath('BoxList'));
             },
             recommendedCourses: function () {
                 return ajaxService.get(buildPath('RecommendedCourses'));                 
             },
             disableFirstTime: function () {
                 return ajaxService.post('/Account/FirstTime/',{ firstTime: 'Dashboard' });                 
             }
         }
     }
    ]);
//});