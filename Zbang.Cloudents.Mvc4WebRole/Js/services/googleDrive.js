define('googleDrive', ['app'], function (app) {
    var googleDrive = app.factory('GoogleDrive',
        ['$document', '$q', '$timeout',
        function ($document, $q, $timeout) {
            var clientId = '616796621727-o9vr11gtr5p9v2t18co7f7kjuu0plnum.apps.googleusercontent.com',
                apiKey = 'AIzaSyBqnR38dm9S2E-eQWRj-cTgup2kGA7lmlg',
                scopes = ['https://www.google.com/m8/feeds/contacts/default/full', 'https://www.googleapis.com/auth/drive.readonly'],
                access_token, contacts = [], isImmediate = true, clientLoaded = false, driveLoaded = false;
            return {
                init: function () {
                    var defer = $q.defer();

                    if (driveLoaded) {
                        $timeout(function () { defer.resolve() });
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
                register: function () {
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
                            clientLoaded = true;

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
                            isImmediate = true;
                            return;
                        }

                        isImmediate = false;
                        checkAuth()
                    }

                },
                picker: function () {
                    var defer = $q.defer();

                    if (!access_token) {
                        this.register().then(load);
                        return defer.promise;
                    }

                    load();
                    return defer.promise;


                    function load(accessToken) {

                        picker = new google.picker.PickerBuilder().
                        addView(google.picker.ViewId.DOCS).
                        enableFeature(google.picker.Feature.NAV_HIDDEN).
                        enableFeature(google.picker.Feature.MULTISELECT_ENABLED).
                        setDeveloperKey('AIzaSyBqnR38dm9S2E-eQWRj-cTgup2kGA7lmlg').
                        setOAuthToken(gapi.auth.getToken().access_token).
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

                            defer.resolve(files);
                        }
                    }


                }
            }
        }
        ]);

    return googleDrive;
});