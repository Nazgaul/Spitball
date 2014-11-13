app.factory('sUserDetails',
 ['sAccount',

 function (sAccount) {
   
     isAuthenticated = false;

     sAccount.details().then(function (response) {

         if (!response) {

             return;
         }

         setDetails(response);
     });

     function setDetails(data) {
         data = data || {};
         if (!data.id) {
             return;
         }
         isAuthenticated = true;
         userData = {
             id: data.id,
             name: data.name,
             image: data.image || '/Images/Defs.svg?23.1.5#user',
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