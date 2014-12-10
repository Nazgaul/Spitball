app.factory('sLogin',
    ['$route','sModal',
    function ($route,sModal) {
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

        function openModal(state) {
            var params = {
                data: {
                    state: state
                }
            };

            var routeName = $route.current.$$route.params.type;
            if (routeName === 'item' || routeName === 'quiz') {
                params.windowClass = 'popupOffset'
            }

            sModal.open('connectPopup', params);
        }

    }
    ]);
