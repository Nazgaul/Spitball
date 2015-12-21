(function () {
    angular.module('app.account').controller('UnregisterShowController', unregister);
    unregister.$inject = ['facebookService', 'accountService', '$stateParams', 'googleService', '$state'];

    function unregister(facebookService, accountService, $stateParams, googleService, $state) {
        if ($state.current.data && $state.current.data.staticPage) {
            return;
        }
        var ur = this;
        var boxId = $stateParams.boxId;
        ur.facebook = facebook;
        ur.google = google;
        ur.googleDisabled = true;

        googleService.initAuth().then(function () {
            ur.googleDisabled = false;
        });
        function facebook() {
            facebookService.loginFacebook().then(function (authToken) {
                accountService.facebookLogIn(authToken, boxId).then(function () {
                    location.reload();
                });
            });
        }

        function google() {
            googleService.login().then(function (authToken) {
                accountService.googleLogIn(authToken, boxId).then(function () {
                    location.reload();
                });
            });
        }
    }
})()