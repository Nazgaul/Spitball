<<<<<<< HEAD
﻿//// Define you service here. Services can be added to same module as 'main' or a seperate module can be created.
//angular.module('apiService', ['jmdobry.angular-cache']).config(['$angularCacheFactoryProvider', function ($angularCacheFactoryProvider) {
//    $angularCacheFactoryProvider.setCacheDefaults({
//        maxAge: 300000,
//        deleteOnExpire: 'aggressive',
//        recycleFreq: 60000,
//        storageMode: 'sessionStorage'
//    });
//}]).
//  
mUser.factory('sUser', [
    '$http',
    '$q',
    'ajaxService',
    function ($http, $q, ajaxService) {
        var User = '/User/';
=======
﻿mUser.factory('sUser', [
    'ajaxService',
   
    function (ajaxService) {
        function buildPath(path) {
            return '/user/' + path + '/';
        }
>>>>>>> 891123d018e44bcdd6054f85b134badeebeb43be
        return {
            friends: function (data) {
                return ajaxService.get(buildPath('Friends'), data);
            },
            minProfile: function (data) {
<<<<<<< HEAD
                var dfd = $q.defer();
                $http.get(User + 'MinProfile/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
=======
                return ajaxService.get(buildPath('MinProfile'), data);
            },         
>>>>>>> 891123d018e44bcdd6054f85b134badeebeb43be
            adminFriends: function (data) {
                return ajaxService.get(buildPath('AdminFriends'), data);
               
            },
            boxes: function (data) {
                return ajaxService.get(buildPath('Boxes'), data);
              
            },
            invites: function (data) {
                return ajaxService.get(buildPath('OwnedInvites'), data);
            },
            activity: function (data) {
                return ajaxService.get(buildPath('Activity'), data);
            },
            departments: function (data) {
<<<<<<< HEAD
                var dfd = $q.defer();
                $http.get(User + 'AdminBoxes/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            notification: function () {
                return ajaxService.get(User + 'Notification/');
            },
        };
    }
=======
                return ajaxService.get(buildPath('AdminBoxes'), data);
            }
    };
}
>>>>>>> 891123d018e44bcdd6054f85b134badeebeb43be
]);
