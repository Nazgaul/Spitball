app.factory('sUserDetails',
 ['sAccount', '$filter',

 function (sAccount, $filter) {
     "use strict";
     var isAuthenticated = false,
         userData;   

     function setDetails(data) {
         data = data || {};
         if (!data.id) {
             return;
         }
         isAuthenticated = true;
         userData = {
             id: data.id,
             name: data.name,
             firstName : data.name.split(' ')[0],
             lastName : data.name.split(' ')[1],
             image: $filter('defaultImage')(data.image,'user'),
             score: data.score,
             url: data.url,
             isAdmin: data.isAdmin,
             university: {
                 // id: data.universityId,
                 name: data.libName,
                 image: data.libImage
             },
             //department: {
             //    id: data.departmentId,
             //    name: data.departmentName

             //}
         };

     }
     return {
         getDetails: function () {
             if (!userData) {
                 var interval = setInterval(function () {

                     if (!userData) {
                         return;
                     }

                     clearData

                 }, 20);
             }
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
             sAccount.details().then(function (response) {

                 if (!response) {

                     return;
                 }

                 setDetails(response);
             });
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