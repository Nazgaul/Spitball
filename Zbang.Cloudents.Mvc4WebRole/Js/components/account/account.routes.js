(function () {
    angular.module('app.account').run(appRun);

    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());


        function getStates() {
            return [
                {
                    state: 'settings',
                    config: {
                        url: '/account/settings/',
                        controller: 'AccountSettingsController as a'
                    },
                    templateUrl: '/account/settingpartial/'
                },
                {
                    state: 'settings.profile',
                    config: {
                        url: '#info',
                        controller: 'AccountSettingsInfoController as i'
                    },
                    templateUrl: '/account/info/'
                },
                {
                    state: 'settings.password',
                    config: {
                        url: '#password',
                        controller: 'AccountSettingsPasswordController as p'
                    },
                    templateUrl: '/account/password/'
                }, {
                    state: 'settings.notification',
                    config: {
                        url: '#notification',
                        controller: 'AccountSettingsNotificationController as n'
                    },
                    templateUrl: '/account/notification/'
                }

            ];
        }
    }
})();