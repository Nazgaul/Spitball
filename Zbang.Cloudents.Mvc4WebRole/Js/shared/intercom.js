"use strict";
/// <reference path="../shared/userDetails.ts" />
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
            // var dateCreate = new Date(data.createTime);
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
                    _this.$mdMenu.hide(); //closes menu
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
/*
declare var Intercom: any;

(() => {
    //TODO: class
    'use strict';

    angular.module('app').run(intercom);
    intercom.$inject = ['userDetailsFactory', '$rootScope', '$mdMenu'];
    function intercom(userDetailsFactory: app.IUserDetailsFactory, $rootScope: ng.IRootScopeService, $mdMenu: any) {
        var started = false;
        userDetailsFactory.init().then(data => {
            start(data);
        });
        function start(data) {
            if (started) {
                return;
            }
            started = true;
            // var dateCreate = new Date(data.createTime);
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
                Intercom('onActivatorClick', () => {
                    $mdMenu.hide(); //closes menu
                });
            }
        }
        function stop() {
            started = false;
            Intercom('shutdown');
        }


    }
})();
*/
//# sourceMappingURL=intercom.js.map