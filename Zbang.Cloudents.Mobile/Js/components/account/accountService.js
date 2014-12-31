angular.module('account')
    .service('accountService',
    ['$window', '$q', 'account', function ($window, $q, account) {
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

            sAccount.changeLocale({ language: language }).then(function () {
                $window.location.reload();
            });
        }

    }]
);