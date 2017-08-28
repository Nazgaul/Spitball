/// <reference path="userDetails.ts" />
'use strict';
(function () {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document', 'userDetailsFactory'];
    function inlineManual($rootScope, $timeout, $document, userDetailsFactory) {
        var registeredUser;
        userDetailsFactory.init().then(function (userData) {
            if (!userData.id) {
                return;
            }
            registeredUser = true;
            inlineManualTracking = {
                uid: userData.id,
                email: userData.email,
                username: userData.name,
                created: Math.round(userData.createTime.getTime() / 1000)
            };
        });
    }
})();
//# sourceMappingURL=inlineManual.js.map