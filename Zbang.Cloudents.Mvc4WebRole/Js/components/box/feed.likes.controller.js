(function () {
    'use strict';
    angular.module('app.box.feed').controller('likesController', likesController);
    likesController.$inject = ['$mdDialog', 'users', '$rootScope'];
    function likesController($mdDialog, users, $rootScope) {
        var lc = this;
        lc.users = users;
        //lc.users.push({ name: 'r' });
        //lc.users.push({ name: '1' });
        //lc.users.push({ name: '2' });
        //lc.users.push({ name: 'r3' });
        //lc.users.push({ name: 'r54' });
        //lc.users.push({ name: 'r2' });
        //lc.users.push({ name: 'r3244' });
        //lc.users.push({ name: 'rf' });
        //lc.users.push({ name: 'rs' });
        //lc.users.push({ name: 'ra' });
        //lc.users.push({ name: 'rx' });
        //lc.users.push({ name: 'rd' });
        //lc.users.push({ name: 'rf' });
        //lc.users.push({ name: 'rh' });
        //lc.users.push({ name: 'rc' });
        //lc.users.push({ name: 'rn' });
        //lc.users.push({ name: 'rg' });
        //lc.users.push({ name: 'rs' });
        //lc.users.push({ name: 'ra' });
        lc.close = close;

        function close() {
            $mdDialog.hide();
        }

        $rootScope.$on('$stateChangeStart', function () {
            $mdDialog.hide();
        });
    }
})();