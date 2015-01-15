angular.module('login')
    .service('loginService',
    ['$state', 'account', 'Facebook', '$rootScope', '$analytics', function ($state, account, facebook, $rootScope, $analytics) {
        "use strict";
        var service = this;

        service.login = function (data) {
            return account.login(data).then(function () {
                $analytics.eventTrack('Login', {
                    category: 'Login page',
                    label: 'User logged in with email'
                });
                //we have an issue - we need to resubmit culture to the dom so we need a refresh
                window.open('dashboard', '_self');
                //$state.go('root.dashboard', {}, { reload: true });
            });
        };

        service.facebookLogin = function () {

            $analytics.eventTrack('Login', {
                category: 'Login page',
                label: 'User logged in with facebook'
            });

            facebook.login(function (response) {
                if (response.authResponse) {
                    account.facebookLogin({ token: response.authResponse.accessToken }).then(function () {
                        //we have an issue - we need to resubmit culture to the dom so we need a refresh
                        window.open('dashboard', '_self');
                        //$state.go($state.current, {}, { reload: true });
                    });
                }
            }, { scope: 'email,user_education_history,user_friends' });
        };

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };
    }]
);