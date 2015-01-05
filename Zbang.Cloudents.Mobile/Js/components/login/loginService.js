angular.module('login')
    .service('loginService',
    ['$state', 'account', 'Facebook', function ($state, account, facebook) {
        "use strict";
        var service = this;

        service.login = function (data) {
            return account.login(data).then(function (response) {                
                $state.go('root.dashboard', {}, { reload: true });

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
    }]
);