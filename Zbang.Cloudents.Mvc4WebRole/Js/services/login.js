﻿app.factory('sLogin',
    ['$rootScope','$route','sModal','$angularCacheFactory',
    function ($rootScope,$route,sModal,$angularCacheFactory) {
        "use strict";
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


        $rootScope.$on('viewContentLoaded', function () {
            var cache = $angularCacheFactory.get('changeLanguage'),
                obj, state;

            if (cache) {
                obj = JSON.parse(cache.get('formData'));
            }

            if (!obj) {
                return;
            }

            openModal(obj.currentState,obj.formData);
        });

        function openModal(state,formData) {
            var params = {
                data: {
                    state: state
                }
            };

            if (formData) {
                params.data.formData = formData;
            }

            var routeName = $route.current.$$route.params.type;

            if (routeName === 'item' || routeName === 'quiz') {
                params.windowClass = 'popupOffset'
            }            

            sModal.open('connectPopup', params);
        }

    }
    ]);
