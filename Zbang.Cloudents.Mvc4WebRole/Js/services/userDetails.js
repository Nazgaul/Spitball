﻿app.factory('sUserDetails',
 ['$rootScope','sAccount', '$filter', '$timeout', '$q','$http',

 function ($rootScope,sAccount, $filter, $timeout, $q,$http) {
     "use strict";
     var isAuthenticated = false,
         userData;

     function setDetails(data) {
         data = data || {};

         $http.defaults.headers.post['X-CSRFToken'] = data.token;

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
             ga('set', 'dimension1', data.university.name);
             ga('set', 'dimension2', data.university.country);
             ga('set', 'dimension3', data.id);
         }


         userData = {
             id: data.id,
             name: data.name,
             image: $filter('defaultImage')(data.image, 'user'),
             score: data.score,
             url: data.url,
             isAdmin: data.isAdmin,

             university: {
                 country: 'IL',// for google analytics
                 name: 'Open x', // for google analytics
                 id: 5 // just for the fun
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