'use strict';
(function () {
    angular.module('app.box.feed').controller('likesController', likesController);
    likesController.$inject = ['$mdDialog', 'users', '$rootScope'];
    function likesController($mdDialog, users, $rootScope) {
        var lc = this;
        lc.users = users;
        lc.close = close;

        function close() {
            $mdDialog.hide();
        }

        $rootScope.$on('$stateChangeStart', function () {
            $mdDialog.hide();
        });
    }
})();