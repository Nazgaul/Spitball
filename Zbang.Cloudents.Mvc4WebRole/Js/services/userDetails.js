﻿app.factory('sUserDetails',
 ['$rootScope','sAccount', '$filter', '$timeout', '$q','$http',

 function ($rootScope,sAccount, $filter, $timeout, $q,$http) {
     "use strict";
     var isAuthenticated = false,
         userData;

     function setDetails(data) {
         data = data || {};

         $http.defaults.headers.common["RequestVerificationToken"] = data.token;

         if (!_.isUndefined(data.id)) {
             isAuthenticated = true;
             $rootScope.user = {
                 isAuthenticated: true
             };

             ga('create', 'UA-9850006-3', {
                 'userId': data.id,
                 'siteSpeedSampleRate': 70,
                 'cookieDomain': 'cloudents.com',
                 'alwaysSendReferrer': true
             });
             ga('set', 'dimension1', data.universityName);
             ga('set', 'dimension2', data.universityCountry);
             ga('set', 'dimension3', data.id);
         }


         userData = {
             id: data.id,
             name: data.name,
             image: $filter('defaultImage')(data.image, 'user'),
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
         setImage: function(image) {
             if (!image) {
                 return;
             }
             userData.image = image;

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