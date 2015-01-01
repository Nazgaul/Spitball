﻿angular.module('register')
    .service('registerService',
    ['account', 'library', '$angularCacheFactory', '$analytics', '$state',
        function (account, library, $angularCacheFactory, $analytics, $state) {
            "use strict";
            var service = this,
                cache = $angularCacheFactory.get('changeLanguage') || $angularCacheFactory('changeLanguage');


            service.signup = function (data) {
                return account.register(data).then(function (response) {
                    $state.go('libChoose');
                                        
                });
            };


            service.changeLanguage = function (language, data) {
                $analytics.eventTrack('Language Change', {
                    category: 'Register',
                    label: 'User changed language to ' + language
                });


                cache.put('formData', JSON.stringify({
                    formData: data
                }));

                account.changeLanguage({ language: register.langauge }).then(function () {
                    $window.location.reload();
                });

            };

            service.getLangugeChangeForm = function () {

                obj = cache.get('formData');

                if (!obj) {
                    return {};
                }

                obj = JSON.parse(obj);

                cache.destroy();

                return obj;
            };

            service.registerUnis = function (term) {
                return library.search({ term: term });
            }


        }]
);