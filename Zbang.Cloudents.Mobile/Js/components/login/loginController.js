angular.module('login', ['ajax', 'social']).
    controller('LoginController',
    ['loginService', function (loginService) {
        "use strict";

        var login = this;

        login.formData = {
            rememberMe: true
        };

        login.submit = function (isValid) {

            login.disabled = login.submitted = true

            if (!isValid) {
                return;
            }

            loginService.login(login.formData).then(null, function () {
                login.disabled = false;
            });

        };

        login.facebook = function () {
            loginService.facebookLogin();

        };
    }]
);