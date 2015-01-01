angular.module('login', ['ajax', 'social']).
    controller('LoginController',
    ['loginService', function (loginService) {
        "use strict";

        var login = this;

        login.formData = {
            rememberMe: true
        };

        login.submit = function (isValid) {


            if (!isValid) {
                return;
            }
            login.disabled = login.submitted = true


            loginService.login(login.formData).catch(function () {
                login.disabled = false;
            });

        };

        login.facebook = function () {
            loginService.facebookLogin();

        };
    }]
);