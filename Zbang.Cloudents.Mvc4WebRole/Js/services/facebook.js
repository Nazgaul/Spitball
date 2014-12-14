app.factory('sFacebook',
   ['$rootScope', '$q', '$analytics', '$timeout', '$angularCacheFactory', 'sShare', 'sGmfcnHandler', '$filter', 'sAccount', '$route', '$window',
   function ($rootScope, $q, $analytics, $timeout, $angularCacheFactory, sShare, sGmfcnHandler, $filter, sAccount, $route, $window) {
       "use strict";
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

       var cache = $angularCacheFactory('facebookPost', {
           maxAge: 300000
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
                           sShare.facebookReputation({ postId: postId });
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
           postFeed: function (text, link) {
               if (!this.isAuthenticated()) {
                   return;
               }


               var isSent = cache.get('isSent');

               if (isSent) {
                   setTimeout();
                   return;
               }



               setTimeout();

               FB.api('/me/feed', 'post', { message: text, link: encodeURI(link) }, function () {
               });

               function setTimeout() {
                   $timeout(function () {
                       cache.put('isSent', true);
                   }, 1000);
               }
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
           registerFacebook: function (data) {
               data = data || {};
               var facebookText = {
                   en: "I have just signed up to Cloudents.\nCloudents is a free online and mobile social studying platform. With a large collection of study material, course notes, summaries and Q&As Cloudents makes my studying easier.",
                   he: 'התחברתי ל-Cloudents, המאגר האקדמי של הסטודנטים.\nמחפשים חומרי לימוד?\nסיכומים, מבחנים,מאמרים.\nמעל ל - 50 אלף קבצים במאגר.\nפשוט לחצו כאן, והירשמו. \nהדרך אל התואר, לא הייתה קלה יותר.',
                   ar: 'أنا أيضا قمت بالاتصال بشبكة Cloudents. حيث أن Cloudentsلديها أكبر مجموعة من مذكرات المقررات الدراسية، والامتحانات في مدرستك. أنضم إلى Cloudents، كلما زاد عدد الطلاب المنضمين، كلما أصبحت الدراسة أسهل.',
                   ru: 'Я тоже подключился к Cloudents. В Cloudents есть крупнейшее собрание конспектов и экзаменов вашего учебного заведения. Присоединяйтесь к Cloudents; чем больше обучающихся присоединятся, тем легче будет учиться.',
                   zh: '我也连接到 Cloudents 了。Cloudents 拥有最丰富的课程笔记以及贵校的考卷。加入 Cloudents 吧，越多人参加，学习就变得越容易。',
                   nl: 'Check Cloudents! Een plek om samen te werken aan opdrachten, studiemateriaal te vinden, proeftentamens te doen of ideeën en teksten te bespreken.'
               };

               var dfd = $q.defer();

               FB.login(function (response) {
                   accessToken = response.authResponse.accessToken;
                   FB.api('/me/permissions', function (response2) {
                       var perms = response2.data[0];

                       if (perms.email) {
                           login();
                           // User has permission
                           $analytics.eventTrack('Facebook Signup', {
                               category: 'Connect Popup',
                               label: 'Successfull login using facebook'
                           });
                       } else {
                           alert('you need to give email permission');
                           $analytics.eventTrack('Facebook Signup', {
                               category: 'Connect Popup',
                               label: 'Failed login using facebook'
                           });

                           dfd.reject();
                       }
                   });


               }, { scope: 'email,publish_stream,user_friends' });

               return dfd.promise;

               function login() {
                   sAccount.facebookLogin({
                       token: accessToken,
                       boxId: data.boxId
                   }).then(function (fbResponse) {
                       var routeName = $route.current.$$route.params.type;

                       if (fbResponse.isnew) {
                           FB.api('/me', function (response) {
                               var locale = response.locale.substr(0, response.locale.indexOf('_')),
                                   text = facebookText[locale];
                               if (!text) {
                                   text = facebookText.en;
                               }
                               this.postFeed(text, 'https://www.cloudents.com');
                           });


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
                   });
               }
           },
           isAuthenticated: function () {
               return isAuthenticated;
           }


       }

   }
   ]);