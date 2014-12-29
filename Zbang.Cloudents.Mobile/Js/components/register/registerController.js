angular.module('register', ['ajax', 'ngMessages']).
    controller('RegisterController',
    ['registerService', function (registerService) {
        var register = this;

        register.formData = {};

        register.submit = function (isValid) {

        }
    }]
);