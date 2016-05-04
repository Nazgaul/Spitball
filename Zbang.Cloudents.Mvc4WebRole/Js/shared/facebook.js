'use strict';
(function () {
    angular.module('app').factory('facebookService', facebook);

    facebook.$inject = [ '$q',  '$timeout'];
    function facebook( $q,  $timeout) {
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
        function loginStatus() {
            var retries = 0,

                interval = setInterval(function () {
// ReSharper disable once UseOfImplicitGlobalInFunctionScope
                    if (!FB) {
                        if (retries > 100) {
                            clearInterval(interval);
                        }
                        return;
                    }
                    clearInterval(interval);

// ReSharper disable once UseOfImplicitGlobalInFunctionScope
                    FB.getLoginStatus(function (response) {
                        facebookInit = true;
                        if (response.status === 'connected') {
                            accessToken = response.authResponse.accessToken;
                            //isAuthenticated = true;
                            //$rootScope.$broadcast('FacebookAuth', true);
                            return;
                        }
                        //$rootScope.$broadcast('FacebookAuth', false);
                    });

                }, 20);
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

            },
            //registerFacebook: function (data) {
            //    data = data || {};
            //    $analytics.pageTrack('facebook/register');

            //    var dfd = $q.defer();

            //    FB.login(function (response) {
            //        if (!response.authResponse) {
            //            $analytics.pageTrack('facebook/register/noauth');
            //            dfd.reject();
            //            return;
            //        }

            //        accessToken = response.authResponse.accessToken;

            //        FB.api('/me/permissions', function () {

            //            $analytics.pageTrack('facebook/register/auth');
            //            login();

            //            $analytics.eventTrack('Facebook Signup', {
            //                category: 'Connect Popup',
            //                label: 'Successfull login using facebook'
            //            });

            //        });


            //    }, { scope: 'email,user_friends' });

            //    return dfd.promise;

            //    function login() {
            //        sAccount.facebookLogin({
            //            token: accessToken,
            //            boxId: data.boxId
            //        }).then(function (fbResponse) {
            //            var routeName = $route.current.$$route.params.type;

            //            if (fbResponse.isnew) {

            //                var cache = $angularCacheFactory('points', {
            //                    maxAge: 600000
            //                });

            //                cache.put('register', true);

            //                $analytics.pageTrack('facebook/register/success');


            //                if (data.boxId) {
            //                    $window.location.reload();
            //                    return;
            //                }

            //                if (routeName === 'account') {
            //                    window.location.href = '/library/choose/';
            //                    return;
            //                }

            //                return;
            //            }

            //            if (routeName === 'account') {
            //                window.location.href = '/dashboard/';
            //                return;
            //            }

            //            $window.location.reload();


            //            dfd.resolve();
            //        }).catch(function () {
            //            $analytics.pageTrack('facebook/register/failed');

            //        });
            //    }
            //},
            //isAuthenticated: function () {
            //    return isAuthenticated;
            //}


        }

    }

})();