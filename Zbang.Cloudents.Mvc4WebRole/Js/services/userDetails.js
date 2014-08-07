app.factory('sUserDetails',
 [

 function () {
     var userData = {};
         isAuthenticated = false;

     return {
         setDetails: function (id, name, image, score, url) {
             if (id) {
                 isAuthenticated = true;
             }
             userData = {
                 id: parseInt(id, 10),
                 name: name,
                 image: image,
                 score: parseInt(score, 10),
                 url: url
             };

         },
         getDetails: function () {
             return userData;
         },
         isAuthenticated: function () {
             return isAuthenticated;
         }
     };
 }
 ]);