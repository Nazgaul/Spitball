app.factory('sUserDetails',
 [

 function () {
     var userData = {
         id: null,
         name: null,
         image : $('body').data('pic'),
         score: 0,
         url: null,
         university: null,
         department: null
     };

     isAuthenticated = false;

     return {
         setDetails: function (data) {
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
                 university: data.university,
                 department: data.department
             };

         },
         getDetails: function () {
             return userData;
         },

         isAuthenticated: function () {
             return isAuthenticated;
         },

         getUniversity: function () {
             if (userData.university) {
                 return userData.university;
             }

             return false;
         },
         setUniversity: function (uniName) {
             if (uniName) {
                 userData.university = uniName;
             }
         },
         getDepartment: function () {
             if (userData.department) {
                 return userData.university;
             }

             return false;
         },
         setDepartment: function (depName) {
             if (depName) {
                 userData.university = depName;
             }
         }
     };
 }
 ]);