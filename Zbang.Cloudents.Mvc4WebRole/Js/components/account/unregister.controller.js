(function () {
    'use strict';
    angular.module('app.account').controller('UnregisterShowController', unregister);
    unregister.$inject = ['facebookService', 'accountService',
        'googleService', '$state', '$rootScope', 'CacheFactory'];

    function unregister(facebookService, accountService, googleService, $state, $rootScope, cacheFactory) {
        if ($state.current.data && $state.current.data.staticPage) {
            return;
        }
        var ur = this;

        //var boxId = $stateParams.boxId;
        ur.facebook = facebook;
        ur.google = google;
        ur.googleDisabled = true;
        ur.close = close;


        googleService.initAuth().then(function () {
            ur.googleDisabled = false;
        });

        function close() {
            $rootScope.$broadcast('hide-unregisterd-box');
        }
        function facebook() {
            var boxId = $state.params.boxId;
            facebookService.loginFacebook().then(function (authToken) {
                accountService.facebookLogIn(authToken, boxId).then(reloadPage);
            });
        }

        function google() {
            var boxId = $state.params.boxId;
            googleService.login().then(function (authToken) {
                accountService.googleLogIn(authToken, boxId).then(reloadPage);
            });
        }
        function reloadPage() {
            cacheFactory.clearAll();
            location.reload();
        }
    }
})()