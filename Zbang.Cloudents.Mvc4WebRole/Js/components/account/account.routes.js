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
                        templateUrl: function () {
                            return routerHelper.buildUrl('/account/settingpartial/');
                        },
                        controller: 'AccountSettingsController as a'
                    }
                },
                {
                    state: 'settings.profile',
                    config: {
                        url: '#info',
                        templateUrl: function () {
                            return routerHelper.buildUrl('/account/info/');
                        },
                        controller: 'AccountSettingsInfoController as i'
                    }
                },
                {
                    state: 'settings.password',
                    config: {
                        url: '#password',
                        templateUrl: function () {
                            return routerHelper.buildUrl('/account/password/');
                        },
                        controller: 'AccountSettingsPasswordController as p'
                    }
                }, {
                    state: 'settings.notification',
                    config: {
                        url: '#notification',
                        templateUrl: function () {
                            return routerHelper.buildUrl('/account/notification/');
                        },
                        controller: 'AccountSettingsNotificationController as n'
                    }
                }

            ];
        }
    }
})();