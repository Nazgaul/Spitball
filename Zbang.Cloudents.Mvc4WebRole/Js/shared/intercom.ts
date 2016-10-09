/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../shared/userDetails.ts" />
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



