/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="userDetails.ts" />
declare var inline_manual_player: any;
declare var inlineManualTracking: any;
(() => {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document', 'userDetailsFactory'];
    function inlineManual($rootScope: ng.IRootScopeService,
        $timeout: ng.ITimeoutService,
        $document: ng.IDocumentService,
        userDetailsFactory: IUserDetailsFactory) {

        userDetailsFactory.init().then((userData) => {
            inlineManualTracking = {
                uid: userData.id,
                email: userData.email,
                username: userData.name,
                created: Math.round(userData.createTime.getTime() / 1000),
                //updated: 1433168312,
                //roles: ["administrator", "editor"],
                //group: "Doe & Partners",
                //plan: "Standard"
            }
            !function () { var e = document.createElement("script"), t = document.getElementsByTagName("script")[0]; e.async = true, e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.js", e.charset = "UTF-8", t.parentNode.insertBefore(e, t); } ();
        });

        $rootScope.$on("$stateChangeSuccess", () => {
            // Inline manual fix for angular
            //var element = $document.find('[ui-view][animation-class]');

            $timeout(() => {
                if (angular.isDefined(inline_manual_player)) {
                    inline_manual_player.manualReinit(/*element[0]*/);
                }
            }, 1000);

        });
    }


})()