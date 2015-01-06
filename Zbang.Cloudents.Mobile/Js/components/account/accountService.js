angular.module('account')
    .service('accountService',
    ['$rootScope', '$window', '$q', '$state', '$analytics', 'account', 'Facebook', function ($rootScope, $window, $q, $state, $analytics, account, facebook) {
        "use strict";
        var service = this;

        service.changeLanguage = function (language) {
            $analytics.eventTrack('Language Change', {
                category: 'Homepage',
                label: 'User changed language to ' + language
            });

            account.changeLocale({ lang: language }).then(function () {
                $window.location.reload();
            });
        };

        service.facebookLogin = function () {
            facebook.login(function (response) {
                if (response.authResponse) {
                    account.facebookLogin({ token: response.authResponse.accessToken }).then(function (fbResposne) {
                        $state.go($state.current, {}, { reload: true });
                    });
                }
            }, { scope: 'email,user_education_history,user_friends' });
        };

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };

    }]
);