app.factory('sLogin',
    ['sModal',
    function (sModal) {
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
            sModal.open('connectPopup', {
                data: {
                    state: state
                }
            });
        }

    }
    ]);
