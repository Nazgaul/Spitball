'use strict';
(function () {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document', 'userDetailsFactory'];
    function inlineManual($rootScope, $timeout, $document, userDetailsFactory) {
        var registeredUser = false;
        userDetailsFactory.init().then(function (userData) {
        });
    }
})();
