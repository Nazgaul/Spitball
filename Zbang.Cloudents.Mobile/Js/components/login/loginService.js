angular.module('login')
    .service('loginService',
    ['account', 'facebook', function (account, facebook) {
        "use strict";
        var service = this;

        service.login = function (data) {
            return account.login(data);
        };

        service.facebookLogin = function () {
            facebook.login();
        };
    }]
);