app.factory('sUserDetails',
 ['$rootScope','sAccount', '$filter', '$timeout', '$q',

 function ($rootScope,sAccount, $filter, $timeout, $q) {
     "use strict";
     var isAuthenticated = false,
         userData;

     function setDetails(data) {
         data = data || {};

         if (!_.isEmpty(data)) {
             isAuthenticated = true;
             $rootScope.user = {
                 isAuthenticated: true
             };
         }


         userData = {
             id: data.id,
             name: data.name,
             image: $filter('defaultImage')(data.image, 'user'),
             score: data.score,
             url: data.url,
             isAdmin: data.isAdmin,
             university: {
                 // id: data.universityId,
                 name: data.libName,
                 image: data.libImage
             }
             //department: {
             //    id: data.departmentId,
             //    name: data.departmentName

             //}
         };

         if (userData.name) {
             userData.firstName = data.name.split(' ')[0];
             userData.lastName = data.name.split(' ')[1];
         }


     }
     return {
         getDetails: function () {
             return userData;
         },

         isAuthenticated: function () {
             return isAuthenticated;
         },

         getUniversity: function () {
             if (_.isEmpty(userData.university)) {
                 return false;
             }
             return userData.university;

         },
         initDetails: function () {
             if (this.isAuthenticated()) {
                 var defer = $q.defer();
                 $timeout(function () {
                     defer.resolve();
                 });
                 return defer.promise;
             }

             var promise = sAccount.details();

             promise.then(function (response) {
                 setDetails(response);
             });

             return promise;
         }
         //setUniversity: function (uniName) {
         //    if (uniName) {
         //        userData.university = uniName;
         //    }
         //},
         //getDepartment: function () {
         //    if (!userData.department.id) {
         //        return false;
         //    }
         //    //if (_.isEmpty(userData.department)) {
         //    //    return false;
         //    //}
         //    return userData.department;
         //},
         //setDepartment: function (department) {
         //    userData.department = department;
         //}
     };
 }
 ]);