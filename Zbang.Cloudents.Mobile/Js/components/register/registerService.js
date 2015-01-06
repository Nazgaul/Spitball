angular.module('register')
    .service('registerService',
    ['account', 'library', '$angularCacheFactory', '$analytics', '$state', '$window', '$rootScope',
        function (account, library, $angularCacheFactory, $analytics, $state, $window, $rootScope) {
            "use strict";
            var service = this,
                cache = $angularCacheFactory.get('changeLanguage') || $angularCacheFactory('changeLanguage');


            service.signup = function (data) {
                return account.register(data).then(function (response) {
                    if (data.universityId) {
                        $state.go('root.dashboard', {}, { reload: true });
                        return;
                    }
                    $state.go('root.libChoose', {}, { reload: true });
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
            };

            service.doneLoad = function () {
                $rootScope.$broadcast('$stateLoaded');
            };
        }]
);