app.factory('sFacebook',
   ['$q', '$analytics', '$timeout', 'sShare',
   function ($q, $analytics, $timeout, sShare) {
       var isAuthenticated = false,
           accessToken,
           facebookInit,
           alreadySent,
           contacts = [];

       window.fbAsyncInit = function () {
           FB.init({
               appId: '450314258355338',
               status: true,
               cookie: true,
               xfbml: true,
               oauth: true
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
           js.src = "//connect.facebook.net/en_US/all.js";
           d.getElementsByTagName('head')[0].appendChild(js);
       }(document));

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
                       if (response.status === 'connected') {
                           accessToken = response.authResponse.accessToken;
                           isAuthenticated = true;
                       }
                       facebookInit = true;
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
                   var defer = $q.defer();
                   url = url || window.location.href;
                   FB.ui({
                       method: 'feed',
                       link: url,
                       name: name,
                       caption: caption,
                       description: description,
                       picture: location.origin + (picture || '/images/cloudents-share-FB.png'),
                       display: 'iframe'
                   }, function (response) {
                       if (response && response.post_id) {
                           $analytics.trackSocial('Facebook', {
                               category: 'Share',
                               label: url
                           });
                           //cd.pubsub.publish('addPoints', { type: 'shareFb' });
                           var postId = response.post_id.split('_')[1]; //takes the post id from *user_id*_*post_id*
                           sShare.facebookReputation({ postId: postId })
                           cd.pubsub.publish('addPoints', { type: 'shareFb' });
                       }
                   });
               }

           },
           send: function (data) {
               var dfd = $q.defer();

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
                       path: data.path,
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
                           firstname: cd.escapeHtmlChars(friend.first_name),
                           middlename: cd.escapeHtmlChars(friend.middle_name),
                           lastname: cd.escapeHtmlChars(friend.last_name),
                           name: friend.first_name + ' ' + (friend.middle_name ? friend.middle_name + ' ' : '') + friend.last_name,
                           username: friend.username,
                           image: friend.picture.data.url,
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

               if (alreadySent) {
                   return;
               }

               alreadySent = true;

               FB.api('/me/feed', 'post', { message: text, link: link }, function () {
               });

               $timeout(function () {
                   alreadySent = false;
               }, 6000000);



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
               },20);


               return defer.promise;
           },
           loginStatus: loginStatus,
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
           isAuthenticated: function () {
               return isAuthenticated;
           }
       }

   }
   ]);