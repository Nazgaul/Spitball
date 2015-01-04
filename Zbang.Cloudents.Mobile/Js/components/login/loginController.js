angular.module('login', ['ajax', 'social']).
    controller('LoginController',
    ['loginService', function (loginService) {
        "use strict";

        var login = this;

        login.formData = {
            rememberMe: true
        };

        login.submit = function (isValid) {

            login.submitted = true;

            if (!isValid) {
                return;
            }
            login.disabled = true


            loginService.login(login.formData).catch(function (response) {
                response = response || [];

                login.disabled = false;                

                if (response.length) {
                    login.serverError = response[0].value[0];
                }
                
            });

        };

        login.facebook = function () {
            loginService.facebookLogin();

        };
    }]
);