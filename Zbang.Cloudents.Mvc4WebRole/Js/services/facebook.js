﻿app.factory('sFacebook',
   ['$document', '$q', '$window',
   function ($document, $q, $window) {
       var facebookLoaded,
           isAuthenticated = false,
           accessToken,
           contacts = [];

       window.fbAsyncInit = function () {
           FB.init({
               appId: '450314258355338',
               status: true,
               cookie: true,
               xfbml: true,
               oauth: true
           });
           facebookLoaded = true;
       };
       (function (d) {
           var js, id = 'facebook-jssdk';
           if (d.getElementById(id)) {
               return;
           }
           js = d.createElement('script');
           js.id = id;
           js.async = true;
           js.src = "//connect.facebook.net/en_US/all.js";
           d.getElementsByTagName('head')[0].appendChild(js);
       }(document));

       return {
           share: function (url, name, caption, description, picture) {
               if (!this.isAuthenticated()) {
                   return;
               }

               var defer = $q.defer();


               url = url || $window.location.href;
               FB.ui({
                   method: 'feed',
                   link: url,
                   name: name,
                   caption: caption,
                   description: description,
                   picture: location.origin + (picture || '/images/cloudents-share-FB.png'),
                   display: 'popup'
               }, function (response) {
                   if (response && response.post_id) {
                       //analytics.trackSocial(url, 'share');
                       //cd.pubsub.publish('addPoints', { type: 'shareFb' });
                       var postId = response.post_id.split('_')[1]; //takes the post id from *user_id*_*post_id*
                       //cd.data.fbRep({                                
                       //    data: { postId: postId }
                       //});
                       defer.resolve();
                       return;
                   }
                   defer.reject();
               });

               return defer.promise;
           },
           send: function (data) {
               var dfd = $q.defer();

               FB.ui({
                   method: 'send',
                   link: data.link,
                   to: data.to
               }, function (response) {
                   dfd.resolve();

                   ///dfd.reject();
               });

               return dfd.promise;
           },
           contacts: function (fields) {
               var dfd = $q.defer();
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
                           firstname: cd.escapeHtmlChars(friend.first_name),
                           middlename: cd.escapeHtmlChars(friend.middle_name),
                           lastname: cd.escapeHtmlChars(friend.last_name),
                           name: friend.first_name + ' ' + (friend.middle_name ? friend.middle_name + ' ' : '') + friend.last_name,
                           userImage: 'null',
                           username: friend.username,
                           defaultImage: friend.picture.data.url,
                           gender: friend.gender === 'male' ? 1 : 0
                       });
                   }
                   dfd.resolve(contacts)
               });

               return dfd.promise;



           },
           postFeed: function (text, link) {
               if (!this.isAuthenticated()) {
                   return;
               }

               FB.api('/me/feed', 'post', { message: text, link: link }, function () {
               });
           },
           getToken: function () {
               return accessToken;
           },
           login: function () {
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
                   dfd.resolve(true);
               });

               return dfd.promise;
           },
           isAuthenticated: function () {
               return isAuthenticated;
           }
       }
   }

   ]);



