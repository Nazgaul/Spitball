app.factory('sFacebook',
   ['$rootScope', '$q', '$analytics', '$timeout', '$angularCacheFactory', 'sShare', 'sGmfcnHandler', '$filter', 'sAccount', '$route', '$window',
   function ($rootScope, $q, $analytics, $timeout, $angularCacheFactory, sShare, sGmfcnHandler, $filter, sAccount, $route, $window) {
       "use strict";
       var isAuthenticated = false,
           accessToken,
           facebookInit,
           contacts = [];

       window.fbAsyncInit = function () {
           FB.init({
               appId: '786786954776091',
               status: true,
               cookie: true,
               xfbml: true,
               version: 'v2.4'
           });
           loginStatus();
       };
       (function (d) {
           var js, id = 'facebook-jssdk';
           if (d.getElementById(id)) {
               return;
           }
           js = d.createElement('script');
           js.id = id;
           js.async = true;
           js.src = "//connect.facebook.net/en_US/sdk.js";
           d.getElementsByTagName('head')[0].appendChild(js);
       }(document));

       var cache = $angularCacheFactory('facebookPost', {
           recycleFreq: 10000
       });

       function loginStatus() {
           var retries = 0,

               interval = setInterval(function () {
                   if (!window.FB) {
                       if (retries > 100) {
                           clearInterval(interval);
                       }
                       return;
                   }
                   clearInterval(interval);

                   FB.getLoginStatus(function (response) {
                       facebookInit = true;

                       if (response.status === 'connected') {
                           accessToken = response.authResponse.accessToken;
                           isAuthenticated = true;
                           $rootScope.$broadcast('FacebookAuth', true);
                           return;
                       }
                       $rootScope.$broadcast('FacebookAuth', false);
                   });

               }, 20);
       }

       return {
           share: function (url, name, caption, description, picture) {

               if (!this.isAuthenticated()) {
                   this.loginFacebook().then(function () {
                       share();
                   });
                   return;
               }

               share();

               function share() {
                   //var defer = $q.defer();
                   url = url || window.location.href;
                   FB.ui({
                       method: 'feed',
                       link: url,
                       name: name,
                       caption: caption,
                       description: description,
                       picture: picture || location.origin + '/images/cloudents-share-FB.png?v=1',
                       display: 'iframe'
                   }, function (response) {
                       if (response && response.post_id) {
                           var postId = response.post_id.split('_')[1]; //takes the post id from *user_id*_*post_id*
                           sShare.facebookReputation({ postId: postId });

                           $analytics.socialTrack('Facebook', {
                               category: 'Share',
                               label: url
                           });
                           //cd.pubsub.publish('addPoints', { type: 'shareFb' });

                           sGmfcnHandler.addPoints({ type: 'shareFb' });
                       }
                   });
               }

           },
           send: function (data) {
               var dfd = $q.defer(),

                url = (data.path.indexOf('http') === -1) ? window.location.origin + data.path : data.path;

               if (!this.isAuthenticated()) {
                   this.loginFacebook().then(function () {
                       fSend();
                   });
                   return;
               }

               fSend();
               function fSend() {
                   FB.ui({
                       method: 'send',
                       link: encodeURI(url),
                       to: data.to
                   }, function (response) {
                       if (!response || response.error_code) {
                           dfd.reject();
                           return;
                       }

                       dfd.resolve();

                   });

               }

               return dfd.promise;
           },
           contacts: function (fields) {
               if (!this.isAuthenticated()) {
                   return;
               }

               var dfd = $q.defer(),
               friend;

               if (contacts.length) {
                   $timeout(function () {
                       dfd.resolve(contacts);
                   });
                   return dfd.promise;  
               }

               //id,first_name,middle_name,last_name,gender,username,picture.height(64).width(64)'
               FB.api('/me/friends?fields=' + fields.toString(), function (response) {
                   for (var i = 0, l = response.data.length; i < l; i++) {
                       if (!response.data) {
                           dfd.reject();
                           return;
                       }
                       friend = response.data[i];
                       contacts.push({
                           id: friend.id,
                           firstname: $filter('escapeHtmlChars')(friend.first_name),
                           middlename: $filter('escapeHtmlChars')(friend.middle_name),
                           lastname: $filter('escapeHtmlChars')(friend.last_name),
                           name: friend.first_name + ' ' + (friend.middle_name ? friend.middle_name + ' ' : '') + friend.last_name,
                           username: friend.username,
                           image: friend.picture.data.url,
                           gender: friend.gender === 'male' ? 1 : 0
                       });
                   }
                   dfd.resolve(contacts);
               });

               return dfd.promise;             
           },
           postFeed: function () {
               if (!this.isAuthenticated()) {
                   return;
               }


               var isSent = cache.get('isSent');

               if (isSent) {                  
                   return;
               }

               cache.put('isSent',true);

               //FB.api('/me/feed', 'post', { message: text, link: encodeURI(link) }, function () {
               //});

               
           },
           getToken: function () {
               var defer = $q.defer();

               if (accessToken) {
                   $timeout(function () {
                       defer.resolve(accessToken);
                   }, 0);

                   return defer.promise;
               }

               var interval = setInterval(function () {
                   if (!facebookInit) {
                       return;
                   }
                   clearInterval(interval);

                   if (accessToken) {
                       defer.resolve(accessToken);
                       return;
                   }

                   defer.reject();
               }, 20);


               return defer.promise;
           },
           loginFacebook: function () {

               var dfd = $q.defer();

               FB.login(function (response) {
                   if (response.status !== 'connected') {
                       dfd.reject();
                       return;
                   }
                   if (!response.authResponse.accessToken) {
                       dfd.reject();
                       return;
                   }

                   isAuthenticated = true;
                   accessToken = response.authResponse.authToken;
                   dfd.resolve();
               });
               return dfd.promise;

           },
           registerFacebook: function (data) {
               data = data || {};
               $analytics.pageTrack('facebook/register');

               var dfd = $q.defer();

               FB.login(function (response) {
                   if (!response.authResponse) {
                       $analytics.pageTrack('facebook/register/noauth');
                       dfd.reject();
                       return;
                   }

                   accessToken = response.authResponse.accessToken;

                   FB.api('/me/permissions', function () {
                      
                           $analytics.pageTrack('facebook/register/auth');
                           login();
                           
                           $analytics.eventTrack('Facebook Signup', {
                               category: 'Connect Popup',
                               label: 'Successfull login using facebook'
                           });
                      
                   });


               }, { scope: 'email,user_friends' });

               return dfd.promise;

               function login() {
                   sAccount.facebookLogin({
                       token: accessToken,
                       boxId: data.boxId
                   }).then(function (fbResponse) {
                       var routeName = $route.current.$$route.params.type;

                       if (fbResponse.isnew) {
                         
                           var cache = $angularCacheFactory('points', {
                               maxAge: 600000
                           });

                           cache.put('register', true);

                           $analytics.pageTrack('facebook/register/success');


                           if (data.boxId) {
                               $window.location.reload();
                               return;
                           }

                           if (routeName === 'account') {
                               window.location.href = '/library/choose/';
                               return;
                           }

                           return;
                       }

                       if (routeName === 'account') {
                           window.location.href = '/dashboard/';
                           return;
                       }

                       $window.location.reload();


                       dfd.resolve();
                   }).catch(function () {
                       $analytics.pageTrack('facebook/register/failed');

                   });
               }
           },
           isAuthenticated: function () {
               return isAuthenticated;
           }


       }

   }
   ]);