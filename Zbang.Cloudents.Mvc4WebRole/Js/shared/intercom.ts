/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../shared/userDetails.ts" />


declare var Intercom: any;
module app {
    "use strict";
    var started: boolean = false;
    export interface IIntercom {
        start(data: IUserData): void;
        stop(): void;
    }

    class IntercomComponent implements IIntercom {
        static $inject = ['userDetailsFactory', '$rootScope', '$mdMenu'];

        constructor(private userDetailsFactory: app.IUserDetailsFactory,
            private $rootScope: ng.IRootScopeService,
            private $mdMenu: any) {
            userDetailsFactory.init().then((data: IUserData) => {
                this.start(data);
            });
        }
        start(data: IUserData) {
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
                    this.$mdMenu.hide(); //closes menu
                });
            }
        }
        stop() {
            started = false;
            Intercom('shutdown');
        }

        static factory() {
            const factory = (userDetailsFactory, $rootScope, $mdMenu) => {
                return new IntercomComponent(userDetailsFactory, $rootScope, $mdMenu);
            };

            factory["$inject"] = ['userDetailsFactory', '$rootScope', '$mdMenu'];
            return factory;
        }

    }
    angular.module("app").run(IntercomComponent.factory());
}
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


