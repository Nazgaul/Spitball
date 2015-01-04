angular.module('register')
    .service('registerService',
    ['account', 'library', '$angularCacheFactory', '$analytics', '$state', '$window',
        function (account, library, $angularCacheFactory, $analytics, $state, $window) {
            "use strict";
            var service = this,
                cache = $angularCacheFactory.get('changeLanguage') || $angularCacheFactory('changeLanguage');


            service.signup = function (data) {
                return account.register(data).then(function (response) {
                    if (data.universityId) {
                        $state.go('dashboard');
                        return;
                    }
                    $state.go('libChoose');
                });
            };


            service.changeLanguage = function (language, data) {
                $analytics.eventTrack('Language Change', {
                    category: 'Register',
                    label: 'User changed language to ' + language
                });             
           
                if (Object.keys(data).length) {
                    cache.put('formData', JSON.stringify({
                        formData: data
                    }));
                }
                
                    account.changeLocale({ lang: language }).then(function () {
                        $window.location.reload();
                    });
            };

            service.getLangugeChangeForm = function () {

                var obj = cache.get('formData');

                if (!obj) {
                    return {};
                }

                obj = JSON.parse(obj);

                cache.removeAll();

                return obj.formData;
            };

            service.searchUnis = function (term, page) {
                return library.search({ term: term, page: page });
            }


        }]
);