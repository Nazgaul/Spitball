app.factory('sUserDetails',
 ['$rootScope','sAccount', '$filter', '$timeout', '$q','$http',

 function ($rootScope,sAccount, $filter, $timeout, $q,$http) {
     "use strict";
     var isAuthenticated = false,
         userData;

     function setDetails(data) {
         data = data || {};

         $http.defaults.headers.common["RequestVerificationToken"] = data.token;

         var analyticsObj = {          
             'siteSpeedSampleRate': 70,
             'cookieDomain': 'spitball.co',
             'alwaysSendReferrer': true
         }

         if (!_.isUndefined(data.id)) {
             isAuthenticated = true;
             $rootScope.user = {
                 isAuthenticated: true
             };

             analyticsObj.userId = data.id;
                        
         }

         ga('create', 'UA-9850006-3', analyticsObj);
         
         ga('set', 'dimension1', data.universityName || null);
         ga('set', 'dimension2', data.universityCountry || null);
         ga('set', 'dimension3', data.id || null);

         userData = {
             id: data.id,
             name: data.name,
             image: data.image,
             score: data.score,
             url: data.url,
             isAdmin: data.isAdmin,
             culture: data.culture,
             firstTimeDashboard : data.firstTimeDashboard,
             firstTimeBox : data.firstTimeBox,
             firstTimeLibrary : data.firstTimeLibrary,
             firstTimeItem: data.firstTimeItem,
             university: {
                 country: data.universityCountry,// for google analytics
                 name: data.universityName, // for google analytics
                 id: data.universityId // just for the fun
                 //    name: data.libName,
                 //    image: data.libImage
             }
         };

         if (userData.name) {
             var splitted = data.name.split(' ');
             userData.firstName = splitted[0];
             switch (splitted) {
                 case 2:
                     userData.lastName = splitted[1];
                     break;
                 case 3:
                     userData.middleName = splitted[1];
                     userData.lastName = splitted[2];
                     break;
             }
          }

     }
     return {
         getDetails: function () {
             return userData;
         },

         isAuthenticated: function () {
             return isAuthenticated;
         },
         setName: function(first,middle,last) {
             userData.firstName = first;
             userData.middleName = middle;
             userData.lastName = last;
         },
         setImage: function(image) {
             if (!image) {
                 return;
             }
             userData.image = image;
         },
         updateChange: function () {
             $rootScope.$broadcast('userDetailsChange');
         },

         //getUniversity: function () {
         //    if (_.isEmpty(userData.university)) {
         //        return false;
         //    }
         //    return userData.university;

         //},
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
     };
 }
 ]);