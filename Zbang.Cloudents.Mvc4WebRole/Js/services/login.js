app.factory('sLogin',
    ['$rootScope','$route','sModal','$angularCacheFactory',
    function ($rootScope,$route,sModal,$angularCacheFactory) {
        "use strict";

        $rootScope.$on('viewContentLoaded', function () {
            var cache,obj, state;

            cache = $angularCacheFactory.get('changeLanguage') || $angularCacheFactory('changeLanguage');

            if (!cache) {
                return;
            }

            obj = cache.get('formData');

            if (!obj) {
                return;
            }
            obj = JSON.parse(obj);

            cache.destroy();

            openModal(obj.currentState, obj.formData);
        });


        return {
            connect: function () {
                openModal(2);
            },
            register: function () {
                openModal(1);
            },
            registerAction: function () {
                openModal(0);
            }
        };


        

        function openModal(state,formData) {
            var params = {
                data: {
                    state: state
                }
            };

            if (formData) {
                params.data.formData = formData;
            }

            sModal.open('connectPopup', params);
        }

    }
    ]);
