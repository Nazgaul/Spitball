angular.module('social', []).
    service('facebook', [function () {
        window.fbAsyncInit = function () {
            FB.init({
                appId: '450314258355338',
                xfbml: true,
                status: true,
                cookie: true,
                oauth: true,
                version: 'v2.2'
            });
        };

        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/sdk.js";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));

        var service = this;

        service.getToken = function () {

        };

        service.register = function () {

        };

        service.login = function () {

        };

    }]
);