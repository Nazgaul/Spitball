angular.module('login')
    .service('loginService',
    ['$state', 'account', 'facebook', function ($state, account, facebook) {
        "use strict";
        var service = this;

        service.login = function (data) {
            return account.login(data).then(function (response) {
                $state.go('root.dashboard');
            });
        };

        service.facebookLogin = function () {
            facebook.login();
        };
    }]
);