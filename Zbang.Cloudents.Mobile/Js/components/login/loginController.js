angular.module('login', ['ajax','social']).
    controller('LoginController',
    ['loginService', function (loginService) {
        "use strict";

        var login = this;

        login.formData = {};

        login.submit = function (isValid) {

            login.submitted = true

            if (!isValid) {
                return;
            }

            loginService.login(login.formData);

        };

        login.facebook = function () {
            loginService.facebookLogin();

        };
    }]
);