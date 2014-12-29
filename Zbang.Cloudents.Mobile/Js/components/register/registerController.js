angular.module('register', ['ajax', 'ngMessages']).
    controller('RegisterController',
    ['registerService', function (registerService) {
        var register = this;

        register.pattern = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;

        register.formData = {};

        register.submit = function (isValid) {
            if (!isValid) {
                return;
            }

            registerService.signup(register.formData);

        };

        register.changeLanguage = function () {
            registerService.changeLanguage(register.language,register.formData);
        };

        register.searchFocus = function () {
            register.searching = true;
        };

        register.searchBlur = function () {
            register.searching = false;
        };

    }]
);