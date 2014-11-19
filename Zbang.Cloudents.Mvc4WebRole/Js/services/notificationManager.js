app.factory('sNotify',
 ['$timeout', '$q', 'resManager', function ($timeout, $q, resManager) {
     "use strict";

     var service = {
         alert:function (message) {
             alert(message);
         },
         tAlert: function (message) {
             var tMessage = getResource(message);
             if (!tMessage) {
                 return;
             }
             alert(tMessage);
         },
         confirm: function (message) {
             var defer = $q.defer(),
                 isConfirm = window.confirm(message);

             $timeout(function () {
                 isConfirm ? defer.resolve() : defer.reject();
             });

             return defer.promise;
         },
         tConfirm: function (message) {
             var tMessage = getResource(message),
                defer = $q.defer(),
                isConfirm = window.confirm(message);

             $timeout(function () {
                 isConfirm ? defer.resolve() : defer.reject();
             });

             return defer.promise;
         }
     };

     function getResource(res) {
         return resManager.get(res);
     }

     return service;
 }]
);

