//define('dashboard',['app'], function (app) {
mDashboard.factory('sDashboard',
    ['ajaxService',
     function (ajaxService) {
         function buildPath(path) {
             return '/dashboard/' + path + '/';
         }

         return {

             boxList: function () {
                 return ajaxService.get(buildPath('boxlist'));
             },             
             sideBar: function () {
                 return ajaxService.get(buildPath('sidebar'));
             }             
         }
     }
    ]);
//});