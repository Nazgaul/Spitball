angular.module('register', ['ajax', 'ngMessages']).
    controller('RegisterController',
    ['registerService', function (registerService) {
        "use strict";

        var register = this;

        register.pattern = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;

        register.page = 0;

        register.formData = {};

        register.submit = function (event, isValid) {
            if (register.searching) {
                event.preventDefault();
                return;
            }

            if (!isValid) {
                return;
            }
            register.disabled = register.submitted = true


            register.signup(register.formData).then(null, null, function () {
                register.disabled = false;
            });
        };

        register.changeLanguage = function () {
            registerService.changeLanguage(register.language, register.formData);
        };

        register.searchIn = function () {
            register.searching = true;
        };

        register.searchOut = function (event) {
            register.searching = false;
            event.target.blur(); //close keyboard
        };

        register.selectUniversity = function (university) {
            register.formData.universityId = universityId;
            register.universityName = university.name;
            register.searching = false;
        };

        register.searchUnis = function (isAppend) {
            register.noResults = false;

            if (!isAppend) {
                register.page = 0;
                get();
                return;
            }

            if (noResults) {
                return;
            }

            register.page++;
            get();

            function get() {
                registerService.searchUnis(register.universityName, register.page).then(function (response) {
                    if (!isAppend) {
                        register.universities = response;
                        register.noResults = true;
                        return;
                    }

                    if (!response.length) {
                        noResulsts = true;
                    }
                    register.universities = register.universities.concat(response);

                });
            }

        };

        register.testScroll = function () {

            register.searchUnis(true);
        };

    }]
);