(function () {
    'use strict';
    angular.module('app').factory('facebookService', facebook);

    facebook.$inject = ['$q', '$timeout', "$interval"];
    function facebook($q, $timeout, $interval) {
        "use strict";
        var accessToken,
            facebookInit;


        window.fbAsyncInit = function () {
            // ReSharper disable once UseOfImplicitGlobalInFunctionScope
            FB.init({
                appId: '450314258355338',
                status: true,
                cookie: true,
                xfbml: true,
                version: 'v2.4'
            });
            loginStatus();
        };
        (function (d, w) {
            function load() {
                var id = 'facebook-jssdk';
                if (d.getElementById(id)) {
                    return;
                }
                var js = d.createElement('script');
                js.id = id;
                js.async = true;
                js.src = "//connect.facebook.net/en_US/sdk.js";
                d.getElementsByTagName('head')[0].appendChild(js);
            }

            if (document.readyState === "complete") {
                load();
            } else {
                w.addEventListener("load", load, false);
            }

        }(document, window));
        function loginStatus() {
            var interval = $interval(function () {
                if (!FB) {
                    return;
                }
                $interval.cancel(interval);

                FB.getLoginStatus(function (response) {
                    facebookInit = true;
                    if (response.status === 'connected') {
                        accessToken = response.authResponse.accessToken;
                        return;
                    }
                });
            }, 20, 100);
        }

        return {
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
                    dfd.resolve(response.authResponse.accessToken);
                }, { scope: 'email,user_friends' });
                return dfd.promise;

            }
        }

    }

})();