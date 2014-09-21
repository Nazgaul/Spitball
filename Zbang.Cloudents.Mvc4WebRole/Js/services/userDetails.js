app.factory('sUserDetails',
 [

 function () {
     var userData = {
         id: null,
         name: null,
         image: $('body').data('pic'),
         score: 0,
         url: null,
         university: null,
         department: null
     };

     isAuthenticated = false;

     return {
         setDetails: function (data) {
             data = data || {};
             if (!data.id) {
                 return;
             }
             isAuthenticated = true;
             userData = {
                 id: parseInt(data.id, 10),
                 name: data.name,
                 image: data.image,
                 score: parseInt(data.score, 10),
                 url: data.url,
                 university: {
                     // id: data.universityId,
                     name: data.libName,
                     image: data.libImage
                 },
                 department: {
                     id: data.departmentId,
                     name: data.departmentName

                 }
             };

         },
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
         getDepartment: function () {
             if (!userData.department.id) {
                 return false;
             }
             //if (_.isEmpty(userData.department)) {
             //    return false;
             //}
             return userData.department;
         },
         setDepartment: function (department) {
             userData.department = department;
         }
     };
 }
 ]);