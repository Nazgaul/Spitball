﻿app.factory('sGoogle',
   ['$document', '$q', '$timeout', 'sShare', '$filter',
   function ($document, $q, $timeout, sShare, $filter) {
       "use strict";

       var clientId = '702268392183-bd7p3isrifipd6t2vr2eb4h38knvp9hb.apps.googleusercontent.com',
           apiKey = 'AIzaSyDWEv6u21Rzxlz6C2KsVSr5rLzGQzt5QHc',
           scopes = ['https://www.google.com/m8/feeds/contacts/default/full', 'https://www.googleapis.com/auth/drive.readonly'],
           access_token, contacts = [], clientLoaded = false, clientLoading = false, driveLoaded = false, pickerDefer,
           api = {
               initDrive: function () {
                   var defer = $q.defer();
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
               initGApi: function () {

                   if (clientLoading) {
                       return;
                   }

                   var defer = $q.defer();

                   if (clientLoaded) {

                       $timeout(function () {
                           defer.resolve(true);
                       });

                       return defer.promise;
                   }

                   clientLoading = true;


                   var js = document.createElement('script');
                   js.id = "jsGoogleContact";
                   js.src = "https://apis.google.com/js/client.js";
                   document.getElementsByTagName('head')[0].appendChild(js);
                   var breakLoop = 0;
                   var interval = window.setInterval(function () {
                       if (window.gapi !== undefined) {
                           window.clearInterval(interval);
                           window.gapi.load('client', function () {
                               gapi.client.setApiKey(apiKey);
                               clientLoaded = true;
                               clientLoading = false;
                               defer.resolve(true);
                           });

                           return;
                       }
                       breakLoop++;
                       if (breakLoop > 500) {
                           window.clearInterval(interval);
                           defer.reject(false);
                       }
                   }, 20);

                   return defer.promise;

               },
               checkAuth: function (isImmediate) {
                   var defer = $q.defer();

                   gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: isImmediate }, function (authResult) {
                       handleResult(authResult);
                   });

                   return defer.promise;

                   function handleResult(authResult) {
                       if (authResult && !authResult.error) {
                           access_token = gapi.auth.getToken().access_token;
                           defer.resolve(access_token);
                           return;
                       }

                       defer.reject();
                   }
               },
               picker: function () {
                   var pickerDefer = $q.defer();
                   var picker = new google.picker.PickerBuilder().
                   addView(google.picker.ViewId.DOCS).
                   enableFeature(google.picker.Feature.NAV_HIDDEN).
                   enableFeature(google.picker.Feature.MULTISELECT_ENABLED).
                   setDeveloperKey('AIzaSyDWEv6u21Rzxlz6C2KsVSr5rLzGQzt5QHc').
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

                       var doc, url, name, size;

                       for (var i = 0, l = data[google.picker.Response.DOCUMENTS].length; i < l ; i++) {
                           doc = data[google.picker.Response.DOCUMENTS][i];
                           if (doc.type === google.picker.Type.DOCUMENT) {
                                url = doc[google.picker.Document.URL];
                           } else {
                               url = 'https://drive.google.com/uc?id=' + doc.id;
                           }
                           

                           name = doc.name;
                           size = doc.sizeBytes;
                           if (!url) {
                               continue;
                           }

                           files.push({
                               name: name,
                               link: url,
                               size: size
                           });
                       }

                       pickerDefer.resolve(files);
                   }

                   return pickerDefer.promise;


               },
               isAuthenticated: function () {
                   if (access_token) {
                       return true;
                   }
                   return false;

               },
               contacts: function (isImmediate) {
                   var defer = $q.defer();



                   if (contacts.length) {
                       $timeout(function () {
                           defer.resolve(contacts);
                       });
                       return defer.promise;
                   }

                   sShare.googleFriends({ token: access_token }).then(function (data) {
                       var feed = JSON.parse(data).feed;
                       for (var i = 0 ; i < feed.entry.length; i++) {
                           var contact = {}, entry = feed.entry[i];
                           if (entry.gd$email) {
                               contact.id = entry.gd$email[0].address;

                               if (entry.title.$t !== '') {
                                   contact.name = $filter('escapeHtmlChars')(entry.title.$t);
                               } else {
                                   contact.name = entry.gd$email[0].address;
                               }

                               if (entry.link[0].gd$etag) {
                                   contact.image = decodeURIComponent(entry.link[0].href) + '&access_token=' + access_token;
                               } else {
                                   contact.image = '/images/user.svg';
                               }

                               contact.google = true;

                               contacts.push(contact);
                           }
                       }

                       defer.resolve(contacts);

                   }, function () {
                       defer.resolve([]);
                   });

                   return defer.promise;

               }

           }

       return api;
   }
   ]);
