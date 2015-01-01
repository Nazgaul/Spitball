angular.module('account')
    .service('accountService',
    ['$window', '$q', '$analytics', 'account', function ($window, $q, $analytics, account) {
        "use strict";
        var service = this;

        service.facebookLogin = function () {
            var dfd = $q.defer();


            facebook.getToken();

            account.facebookLogin({ token: token }).then(function () {

            });
        };

        service.changeLanguage = function (language) {
            $analytics.eventTrack('Language Change', {
                category: 'Homepage',
                label: 'User changed language to ' + language
            });

            account.changeLocale({ lang: language }).then(function () {
                $window.location.reload();
            });
        }

    }]
);