app.factory('sLogin',
    ['sModal',
    function (sModal) {
        "use strict";
        return {
            connect: function () {
                openModal();
            },
            register: function () {
                api.register();
            },
            registerAction: function () {
                api.registerAction();
            },
            reset: function () {
                api.reset();
            }
        };

        function openModal() {
            sModal.open('connectPopup', {

            });
        }

    }
    ]);
