//define('dashboard',['app'], function (app) {
mDashboard.factory('sDashboard',
    ['ajaxService',
     function (ajaxService) {
         function buildPath(path) {
             return '/dashboard/' + path + '/';
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