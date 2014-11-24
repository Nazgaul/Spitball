(function (api) {
    app.factory('sLogin',
        [
        function () {

            return {
                connect: function () {
                    api.connect();
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
        }
        ]);
})(window.connectApi);
