/// <reference path="userDetails.ts" />
'use strict';
declare var inline_manual_player: any;
declare var inlineManualTracking: any;
(() => {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document', 'userDetailsFactory'];
    function inlineManual($rootScope: ng.IRootScopeService,
        $timeout: ng.ITimeoutService,
        $document: ng.IDocumentService,
        userDetailsFactory: app.IUserDetailsFactory) {
        var registeredUser: boolean;
        userDetailsFactory.init().then((userData) => {
            if (!userData.id) {
                return;
            }
            registeredUser = true;
            inlineManualTracking = {
                uid: userData.id,
                email: userData.email,
                username: userData.name,
                created: Math.round(userData.createTime.getTime() / 1000)
            }
        });
    }
})()