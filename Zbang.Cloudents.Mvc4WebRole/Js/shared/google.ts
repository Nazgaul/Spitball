
module app {
    "use strict";
    var clientId = '997823384046-ddhrphigu0hsgkk1dglajaifcg2rggbm.apps.googleusercontent.com',
        apiKey = 'AIzaSyAZ9mDzvTjIUvFPMdv3BXmXFeoAZihFKQk',
        scopes = [
            'https://www.google.com/m8/feeds/contacts/default/full',
            'https://www.googleapis.com/auth/drive.readonly'
        ],
        access_token,
        contacts = [],
        clientLoaded = false,
        clientLoading = false,
        driveLoaded = false
    export interface IGoogleService {
        initDrive(): angular.IPromise<any>;
        initGApi(): angular.IPromise<any>;
        initAuth(): angular.IPromise<any>;
        checkAuth(isImmediate: boolean): angular.IPromise<any>;
        picker(): angular.IPromise<any>;
        isAuthenticated(): boolean;
        contacts(isImmediate: boolean): angular.IPromise<any>;
        login(): angular.IPromise<any>;
    }

    class GoogleService implements IGoogleService {
        constructor(private $document: angular.IDocumentService,
            private $q: angular.IQService,
            private $timeout: angular.ITimeoutService,
            private $filter: angular.IFilterService,
            private shareService: IShareService) {
        }

        initDrive() {
            var defer = this.$q.defer();
            if (driveLoaded) {
                this.$timeout(() => { defer.resolve(); });
                return defer.promise;
            }
            var js = document.createElement('script');
            js.id = "googleDrive";
            js.src = "https://apis.google.com/js/api.js";
            document.getElementsByTagName('head')[0].appendChild(js);

            var interval = window.setInterval(() => {
                if (window["gapi"] !== undefined && window["gapi"]) {
                    window["gapi"].load('picker',
                        {
                            'callback': () => {
                                defer.resolve();
                                driveLoaded = true;
                            }
                        });
                    window.clearInterval(interval);
                }
            },
                20);

            return defer.promise;
        };
        initGApi() {

            if (clientLoading) {
                return;
            }

            var defer = this.$q.defer();

            if (clientLoaded) {

                this.$timeout(() => {
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
            var interval = window.setInterval(() => {
                if (window["gapi"] !== undefined) {
                    window.clearInterval(interval);
                    window["gapi"].load('client',
                        () => {
                            window["gapi"].client.setApiKey(apiKey);
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
            },
                20);

            return defer.promise;

        };
        initAuth() {
            function load() {
                var js = document.createElement('script');
                js.src = "//apis.google.com/js/platform.js";
                //js.async = true;
                js.defer = true;
                document.getElementsByTagName('head')[0].appendChild(js);
                js.onload = () => { initAuthGapi(); };
            }

            if (document.readyState === "complete") {
                load();
            } else {
                window.addEventListener("load", load, false);
            }


            var defer = this.$q.defer();

            function initAuthGapi() {
                window["gapi"].load('auth2',
                    () => {
                        window["gapi"].auth2.init();
                        defer.resolve();
                    });

            }

            return defer.promise;
        };
        checkAuth(isImmediate: boolean) {
            var defer = this.$q.defer();

            window["gapi"].auth.authorize({ client_id: clientId, scope: scopes, immediate: isImmediate },
                (authResult) => {
                    handleResult(authResult);
                });

            return defer.promise;

            function handleResult(authResult) {
                if (authResult && !authResult.error) {
                    access_token = window["gapi"].auth.getToken().access_token;
                    defer.resolve(access_token);
                    return;
                }

                defer.reject();
            }
        };

        picker() {
            var self = this;
            var defer = this.$q.defer();
            if (!this.isAuthenticated()) {
                this.checkAuth(false)
                    .then(() => {
                        defer.resolve(loadPicker());
                        //return loadPicker();
                    });
                return defer.promise;
            }
            defer.resolve(loadPicker());
            //return loadPicker();
            return defer.promise;


            function loadPicker() {
                var pickerDefer = self.$q.defer();
                var picker = new window["google"].picker.PickerBuilder().addView(window["google"].picker.ViewId.DOCS)
                    .enableFeature(window["google"].picker.Feature.NAV_HIDDEN)
                    .enableFeature(window["google"].picker.Feature.MULTISELECT_ENABLED)
                    .setDeveloperKey(apiKey)
                    .setOAuthToken(access_token)
                    .setCallback(pickerCallback)
                    .build();

                picker.setVisible(true);


                // A simple callback implementation.
                function pickerCallback(data) {
                    var files = [];
                    if (data[window["google"].picker.Response.ACTION] !== window["google"].picker.Action.PICKED) {
                        return;
                    }

                    var doc, url, name, size;

                    for (var i = 0, l = data[window["google"].picker.Response.DOCUMENTS].length; i < l; i++) {
                        doc = data[window["google"].picker.Response.DOCUMENTS][i];
                        if (doc.type === window["google"].picker.Type.DOCUMENT) {
                            url = doc[window["google"].picker.Document.URL];
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
            }

        };
        isAuthenticated() {
            if (access_token) {
                return true;
            }
            return false;

        };
        contacts(isImmediate: boolean) {
            var defer = this.$q.defer();


            if (contacts.length) {
                this.$timeout(() => {
                    defer.resolve(contacts);
                });
                return defer.promise;
            }

            this.shareService.googleFriends(access_token)
                .then((data) => {
                    var feed = JSON.parse(data).feed;
                    for (var i = 0; i < feed.entry.length; i++) {
                        var contact = {}, entry = feed.entry[i];
                        if (entry.gd$email) {
                            contact["id"] = entry.gd$email[0].address;

                            if (entry.title.$t !== '') {
                                //contact.name = $filter('escapeHtmlChars')(entry.title.$t);
                                contact["name"] = entry.title.$t;
                            } else {
                                contact["name"] = entry.gd$email[0].address;
                            }

                            if (entry.link[0].gd$etag) {
                                contact["image"] = decodeURIComponent(entry.link[0].href) +
                                    '&access_token=' +
                                    access_token;
                            } else {
                                contact["image"] = '/images/user.svg';
                            }

                            contact["google"] = true;

                            contacts.push(contact);
                        }
                    }

                    defer.resolve(contacts);

                },
                function () {
                    defer.resolve([]);
                });

            return defer.promise;

        };

        login() {
            var defer = this.$q.defer();
            var authInstance = window["gapi"].auth2.getAuthInstance();
            authInstance.signIn()
                .then((googleUser) => {
                    var id_token = googleUser.getAuthResponse().id_token;
                    defer.resolve(id_token);

                });

            return defer.promise;
        }

        public static factory() {
            const factory = ($document: angular.IDocumentService,
                $q: angular.IQService,
                $timeout: angular.ITimeoutService,
                $filter: angular.IFilterService,
                shareService: IShareService) => {
                return new GoogleService($document, $q, $timeout, $filter, shareService);
            };

            factory["$inject"] = ['$document', '$q', '$timeout', '$filter', 'shareService'];
            ;
            return factory;
        }
    }
    angular.module("app")
        .factory("googleService", GoogleService.factory());
}



// #    OLD google.js
//(function () {
//    'use strict';
//    angular.module('app').factory('googleService', service);

//    service.$inject = ['$document', '$q', '$timeout', '$filter', 'shareService'];

//    function service($document, $q, $timeout, $filter, shareService) {
//        "use strict";

//        var clientId = '997823384046-ddhrphigu0hsgkk1dglajaifcg2rggbm.apps.googleusercontent.com',
//            apiKey = 'AIzaSyAZ9mDzvTjIUvFPMdv3BXmXFeoAZihFKQk',
//            scopes = [
//                'https://www.google.com/m8/feeds/contacts/default/full',
//                'https://www.googleapis.com/auth/drive.readonly'
//            ],
//            access_token,
//            contacts = [],
//            clientLoaded = false,
//            clientLoading = false,
//            driveLoaded = false,
//            api = {
//                initDrive: function () {
//                    var defer = $q.defer();
//                    if (driveLoaded) {
//                        $timeout(function () { defer.resolve(); });
//                        return defer.promise;
//                    }
//                    var js = document.createElement('script');
//                    js.id = "googleDrive";
//                    js.src = "https://apis.google.com/js/api.js";
//                    document.getElementsByTagName('head')[0].appendChild(js);

//                    var interval = window.setInterval(function () {
//                        if (window.gapi !== undefined && window.gapi) {
//                            gapi.load('picker',
//                                {
//                                    'callback': function () {
//                                        defer.resolve();
//                                        driveLoaded = true;
//                                    }
//                                });
//                            window.clearInterval(interval);
//                        }
//                    },
//                        20);

//                    return defer.promise;
//                },
//                initGApi: function () {

//                    if (clientLoading) {
//                        return;
//                    }

//                    var defer = $q.defer();

//                    if (clientLoaded) {

//                        $timeout(function () {
//                            defer.resolve(true);
//                        });

//                        return defer.promise;
//                    }

//                    clientLoading = true;


//                    var js = document.createElement('script');
//                    js.id = "jsGoogleContact";
//                    js.src = "https://apis.google.com/js/client.js";
//                    document.getElementsByTagName('head')[0].appendChild(js);
//                    var breakLoop = 0;
//                    var interval = window.setInterval(function () {
//                        if (window.gapi !== undefined) {
//                            window.clearInterval(interval);
//                            window.gapi.load('client',
//                                function () {
//                                    gapi.client.setApiKey(apiKey);
//                                    clientLoaded = true;
//                                    clientLoading = false;
//                                    defer.resolve(true);
//                                });

//                            return;
//                        }
//                        breakLoop++;
//                        if (breakLoop > 500) {
//                            window.clearInterval(interval);
//                            defer.reject(false);
//                        }
//                    },
//                        20);

//                    return defer.promise;

//                },
//                initAuth: function () {
//                    function load() {
//                        var js = document.createElement('script');
//                        js.src = "//apis.google.com/js/platform.js";
//                        //js.async = true;
//                        js.defer = true;
//                        document.getElementsByTagName('head')[0].appendChild(js);
//                        js.onload = function () { initAuthGapi(); };
//                    }

//                    if (document.readyState === "complete") {
//                        load();
//                    } else {
//                        window.addEventListener("load", load, false);
//                    }


//                    var defer = $q.defer();

//                    function initAuthGapi() {
//                        gapi.load('auth2',
//                            function () {
//                                gapi.auth2.init();
//                                defer.resolve();
//                            });

//                    }

//                    return defer.promise;
//                },
//                checkAuth: function (isImmediate) {
//                    var defer = $q.defer();

//                    gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: isImmediate },
//                        function (authResult) {
//                            handleResult(authResult);
//                        });

//                    return defer.promise;

//                    function handleResult(authResult) {
//                        if (authResult && !authResult.error) {
//                            access_token = gapi.auth.getToken().access_token;
//                            defer.resolve(access_token);
//                            return;
//                        }

//                        defer.reject();
//                    }
//                },

//                picker: function () {
//                    var defer = $q.defer();
//                    if (!this.isAuthenticated()) {
//                        this.checkAuth(false)
//                            .then(function () {
//                                defer.resolve(loadPicker());
//                                //return loadPicker();
//                            });
//                        return defer.promise;
//                    }
//                    defer.resolve(loadPicker());
//                    //return loadPicker();
//                    return defer.promise;


//                    function loadPicker() {
//                        var pickerDefer = $q.defer();
//                        var picker = new google.picker.PickerBuilder().addView(google.picker.ViewId.DOCS)
//                            .enableFeature(google.picker.Feature.NAV_HIDDEN)
//                            .enableFeature(google.picker.Feature.MULTISELECT_ENABLED)
//                            .setDeveloperKey(apiKey)
//                            .setOAuthToken(access_token)
//                            .setCallback(pickerCallback)
//                            .build();

//                        picker.setVisible(true);


//                        // A simple callback implementation.
//                        function pickerCallback(data) {
//                            var files = [];
//                            if (data[google.picker.Response.ACTION] !== google.picker.Action.PICKED) {
//                                return;
//                            }

//                            var doc, url, name, size;

//                            for (var i = 0, l = data[google.picker.Response.DOCUMENTS].length; i < l; i++) {
//                                doc = data[google.picker.Response.DOCUMENTS][i];
//                                if (doc.type === google.picker.Type.DOCUMENT) {
//                                    url = doc[google.picker.Document.URL];
//                                } else {
//                                    url = 'https://drive.google.com/uc?id=' + doc.id;
//                                }


//                                name = doc.name;
//                                size = doc.sizeBytes;
//                                if (!url) {
//                                    continue;
//                                }

//                                files.push({
//                                    name: name,
//                                    link: url,
//                                    size: size
//                                });
//                            }

//                            pickerDefer.resolve(files);
//                        }

//                        return pickerDefer.promise;
//                    }

//                },
//                isAuthenticated: function () {
//                    if (access_token) {
//                        return true;
//                    }
//                    return false;

//                },
//                contacts: function (isImmediate) {
//                    var defer = $q.defer();


//                    if (contacts.length) {
//                        $timeout(function () {
//                            defer.resolve(contacts);
//                        });
//                        return defer.promise;
//                    }

//                    shareService.googleFriends(access_token)
//                        .then(function (data) {
//                            var feed = JSON.parse(data).feed;
//                            for (var i = 0; i < feed.entry.length; i++) {
//                                var contact = {}, entry = feed.entry[i];
//                                if (entry.gd$email) {
//                                    contact.id = entry.gd$email[0].address;

//                                    if (entry.title.$t !== '') {
//                                        //contact.name = $filter('escapeHtmlChars')(entry.title.$t);
//                                        contact.name = entry.title.$t;
//                                    } else {
//                                        contact.name = entry.gd$email[0].address;
//                                    }

//                                    if (entry.link[0].gd$etag) {
//                                        contact.image = decodeURIComponent(entry.link[0].href) +
//                                            '&access_token=' +
//                                            access_token;
//                                    } else {
//                                        contact.image = '/images/user.svg';
//                                    }

//                                    contact.google = true;

//                                    contacts.push(contact);
//                                }
//                            }

//                            defer.resolve(contacts);

//                        },
//                        function () {
//                            defer.resolve([]);
//                        });

//                    return defer.promise;

//                },

//                login: function () {
//                    var defer = $q.defer();
//                    var authInstance = gapi.auth2.getAuthInstance();
//                    authInstance.signIn()
//                        .then(function (googleUser) {
//                            var id_token = googleUser.getAuthResponse().id_token;
//                            defer.resolve(id_token);

//                        });

//                    return defer.promise;
//                }

//            };

//        return api;
//    }
//})();