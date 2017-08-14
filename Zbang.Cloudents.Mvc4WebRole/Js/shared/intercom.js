var app;
(function (app) {
    "use strict";
    var started = false;
    var IntercomComponent = (function () {
        function IntercomComponent(userDetailsFactory, $rootScope, $mdMenu) {
            var _this = this;
            this.userDetailsFactory = userDetailsFactory;
            this.$rootScope = $rootScope;
            this.$mdMenu = $mdMenu;
            userDetailsFactory.init().then(function (data) {
                _this.start(data);
            });
        }
        IntercomComponent.prototype.start = function (data) {
            var _this = this;
            if (started) {
                return;
            }
            started = true;
            if (data.id) {
                Intercom('boot', {
                    app_id: "njmpgayv",
                    name: data.name,
                    email: data.email,
                    created_at: Math.round(data.createTime.getTime() / 1000),
                    user_id: data.id,
                    user_image: data.image,
                    university_id: data.university.id,
                    university_name: data.university.name,
                    reputation: data.score,
                    language: data.culture,
                    university_country: data.university.country,
                    widget: {
                        activator: '#Intercom'
                    }
                });
                Intercom('onActivatorClick', function () {
                    _this.$mdMenu.hide();
                });
            }
        };
        IntercomComponent.prototype.stop = function () {
            started = false;
            Intercom('shutdown');
        };
        IntercomComponent.factory = function () {
            var factory = function (userDetailsFactory, $rootScope, $mdMenu) {
                return new IntercomComponent(userDetailsFactory, $rootScope, $mdMenu);
            };
            factory["$inject"] = ['userDetailsFactory', '$rootScope', '$mdMenu'];
            return factory;
        };
        return IntercomComponent;
    }());
    IntercomComponent.$inject = ['userDetailsFactory', '$rootScope', '$mdMenu'];
    angular.module("app").run(IntercomComponent.factory());
})(app || (app = {}));
//# sourceMappingURL=intercom.js.map