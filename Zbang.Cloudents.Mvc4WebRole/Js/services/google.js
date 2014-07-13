app.factory('sGoogle',
   ['$document', '$q', '$timeout', 'sShare',
   function ($document, $q, $timeout, Share) {
       var clientId = '616796621727-o9vr11gtr5p9v2t18co7f7kjuu0plnum.apps.googleusercontent.com',
           apiKey = 'AIzaSyBqnR38dm9S2E-eQWRj-cTgup2kGA7lmlg',
           scopes = ['https://www.google.com/m8/feeds/contacts/default/full', 'https://www.googleapis.com/auth/drive.readonly'],
           access_token, contacts = [], clientLoaded = false, driveLoaded = false, pickerDefer;
       var api = {
           initDrive: function () {
               var defer = $q.defer();
               pickerDefer = $q.defer();
               if (driveLoaded) {
                   $timeout(function () { defer.resolve(); });
                   return defer.promise;
               }
               var js = document.createElement('script');
               js.id = "googleDrive";
               js.src = "https://apis.google.com/js/api.js";
               document.getElementsByTagName('head')[0].appendChild(js);

               var interval = window.setInterval(function () {
                   if (window.gapi !== undefined && window.gapi) {
                       gapi.load('picker', {
                           'callback': function () {
                               defer.resolve();
                               driveLoaded = true;
                           }
                       });
                       window.clearInterval(interval);
                   }
               }, 20);

               return defer.promise;
           },
           register: function (isImmediate) {
               if (clientLoaded) {
                   return;
               }
               var defer = $q.defer(),
                   js = document.createElement('script');
               js.id = "jsGoogleContact";
               js.src = " https://apis.google.com/js/client.js";
               document.getElementsByTagName('head')[0].appendChild(js);
               var interval = window.setInterval(function () {
                   if (window.gapi !== undefined && window.gapi.client !== undefined) {
                       window.clearInterval(interval);
                       gapi.client.setApiKey(apiKey);
                       checkAuth();
                   }
               }, 20);

               return defer.promise;

               function checkAuth() {
                   gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: isImmediate }, function (authResult) {
                       handleResult(authResult);
                   });
               }

               function handleResult(authResult) {
                   if (authResult && !authResult.error) {
                       access_token = gapi.auth.getToken().access_token;
                       defer.resolve(access_token);
                       return;
                   }

                   defer.reject();
               }

           },
           picker: function (isImmediate) {

               if (!api.isAuthenticated()) {
                   api.register(isImmediate).then(load, function () {
                       api.picker(false); //we run the loop again witrh isImmediate to popup google login 
                   });
                   return pickerDefer.promise;
               }

               load();
               return pickerDefer.promise;


               function load() {

                   var picker = new google.picker.PickerBuilder().
                   addView(google.picker.ViewId.DOCS).
                   enableFeature(google.picker.Feature.NAV_HIDDEN).
                   enableFeature(google.picker.Feature.MULTISELECT_ENABLED).
                   setDeveloperKey('AIzaSyBqnR38dm9S2E-eQWRj-cTgup2kGA7lmlg').
                   setOAuthToken(access_token).
                   setCallback(pickerCallback).
                   build();

                   picker.setVisible(true);


                   // A simple callback implementation.
                   function pickerCallback(data) {
                       var files = [];
                       if (data[google.picker.Response.ACTION] !== google.picker.Action.PICKED) {
                           return;
                       }

                       var doc, url, name;

                       for (var i = 0, l = data[google.picker.Response.DOCUMENTS].length; i < l ; i++) {
                           doc = data[google.picker.Response.DOCUMENTS][i];
                           url = doc[google.picker.Document.URL];
                           name = doc.name;
                           if (!url) {
                               continue;
                           }

                           files.push({
                               name: name,
                               link: url,
                               size: '??'
                           });
                       }

                       pickerDefer.resolve(files);
                   }
               }


           },
           isAuthenticated: function () {
               if (access_token) {
                   return true;
               }

               return false;
           },
           contacts: function (isImmediate) {
               var defer = $q.defer();

               if (!api.isAuthenticated()) {
                   api.register(isImmediate).then(getContacts);
                   return defer.promise;
               }

               getContacts();

               function getContacts() {
                   if (contacts.length) {
                       defer.resolve(contacts);
                       return;
                   }

                   Share.googleFriends({ token: access_token }).then(function (data) {
                       if (!data.success) {
                           defer.resolve([]);
                           return;
                       }
                       var feed = JSON.parse(data.payload).feed;
                       for (var i = 0 ; i < feed.entry.length; i++) {
                           var contact = {}, entry = feed.entry[i];
                           if (entry.gd$email) {
                               contact.id = entry.gd$email[0].address;

                               if (entry.title.$t !== '') {
                                   contact.name = cd.escapeHtmlChars(entry.title.$t);
                               } else {
                                   contact.name = entry.gd$email[0].address;
                               }

                               if (entry.link[0].gd$etag) {
                                   contact.image = decodeURIComponent(entry.link[0].href) + '&access_token=' + access_token;
                               } else {
                                   contact.image = '/Images/user-gmail-pic.jpg';
                               }

                               contact.google = true;

                               contacts.push(contact);
                           }
                       }

                       defer.resolve(contacts);

                   });
               }

               return defer.promise;
           }

       }

       return api;
   }
   ]);
