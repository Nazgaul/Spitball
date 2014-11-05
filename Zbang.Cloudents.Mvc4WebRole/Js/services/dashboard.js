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
             sideBar: function () {
                 return ajaxService.get(buildPath('Sidebar'));
             },
             disableFirstTime: function () {
                 return ajaxService.post('/Account/FirstTime/',{ firstTime: 'Dashboard' });                 
             }
         }
     }
    ]);
//});