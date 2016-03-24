/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="userDetails.ts" />
declare var inline_manual_player: any;
declare var inlineManualTracking: any;
(() => {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document','userDetailsFactory'];
    function inlineManual($rootScope: ng.IRootScopeService,
        $timeout: ng.ITimeoutService,
        $document: ng.IDocumentService,
        userDetailsFactory: IUserDetailsFactory) {

        userDetailsFactory.init().then((userData) => {
            console.log(userData.createTime.getTime());
            inlineManualTracking = {
                uid: userData.id,
                email: userData.email,
                username: userData.name,
                created: userData.createTime.getTime(),
                //updated: 1433168312,
                //roles: ["administrator", "editor"],
                //group: "Doe & Partners",
                //plan: "Standard"
            }
            !function () { var e = document.createElement("script"), t = document.getElementsByTagName("script")[0]; e.async = true, e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.js", e.charset = "UTF-8", t.parentNode.insertBefore(e, t) } ();            
        });
        
        $rootScope.$on("$stateChangeSuccess", () => {
            // Inline manual fix for angular
            var element = $document.find('[ui-view][animation-class]');
            if (angular.isDefined(inline_manual_player)) {
                $timeout(() => {
                    inline_manual_player.manualReinit(element[0]);
                }, 1000);
            }
        });
    }


})()