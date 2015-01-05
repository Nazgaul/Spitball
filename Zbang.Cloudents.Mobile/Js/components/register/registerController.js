﻿angular.module('register', ['ajax', 'ngMessages']).
    controller('RegisterController',
    ['registerService', function (registerService) {
        "use strict";

        var register = this,
            noResults = false,
            isSearching = false;

        register.pattern = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;

        register.page = 0;


        register.formData = registerService.getLangugeChangeForm();

        register.submit = function (event, isValid) {
            if (register.searching) {
                event.preventDefault();
                return;
            }

            register.submitted = true

            if (!isValid) {
                return;
            }
            register.disabled = true;


            registerService.signup(register.formData).catch(function (response) {
                response = response || [];

                register.disabled = false;

                if (response.length) {
                    register.serverError = response[0].value[0];
                }
            });
        };

        register.changeLanguage = function () {
            registerService.changeLanguage(register.language, register.formData);
        };

        register.searchIn = function (event) {
            if (register.searching) {
                return;
            }
            register.searching = true;
            var target = event.target;                        
        };

        register.searchOut = function (event) {
            register.searching = false;

            event.target.blur(); //close keyboard
        };

        register.selectUniversity = function (university) {
            register.formData.universityId = university.id;
            register.formData.universityName = university.name;
            register.searching = false;
        };
        
        register.searchUnis = function (isAppend) {
            var term = register.formData.universityName;


            if (isSearching) {
                return;
            }

            if (term && term.length < 2) {
                return;
            }

            if (!term) {
                register.universities = [];
                return;
            }

            register.noResults = false;

            if (!isAppend) {
                noResults = false;
                register.page = 0;
                get();
                return;
            }

            if (noResults) {
                return;
            }
            
            get();

            function get() {           
                
;
                isSearching = true;

                registerService.searchUnis(term, register.page).then(function (response) {
                    response = response || [];

                    register.page++;

                    if (!isAppend) {
                        register.universities = response;
                        if (!response.length) {
                            register.noResults = true;

                        }
                        return;
                    }                    

                    if (!response.length) {
                        noResults = true;
                    }
                    register.universities = register.universities.concat(response);

                }).finally(function () {
                    isSearching = false;
                });
            }

        };


    }]
);