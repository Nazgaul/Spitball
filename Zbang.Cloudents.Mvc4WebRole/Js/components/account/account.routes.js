(function () {
    'use strict';
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
                        controller: 'AccountSettingsController as a',
                        resolve: {
                            userData: [
                                'accountService', function (accountService) {
                                    return accountService.getAccountDetails();

                                }
                            ]
                            //loadMyCtrl: [
                            //       '$ocLazyLoad', function ($ocLazyLoad) {
                            //           return $ocLazyLoad.load('upload');
                            //       }
                            //]
                        }
                    },
                    templateUrl: '/account/settingpartial/'
                },
                {
                    state: 'settings.profile',
                    config: {
                        url: 'info/',
                        controller: 'AccountSettingsInfoController as i',
                        parent: 'settings'
                    },
                    templateUrl: '/account/info/'
                },
                {
                    state: 'settings.password',
                    config: {
                        url: 'password/',
                        controller: 'AccountSettingsPasswordController as p',
                        parent: 'settings'
                    },
                    templateUrl: '/account/password/'
                }, {
                    state: 'settings.notification',
                    config: {
                        url: 'notification/',
                        controller: 'AccountSettingsNotificationController as n',
                        parent: 'settings'
                    },
                    templateUrl: '/account/notification/'
                }, {
                    state: 'settings.department',
                    config: {
                        url: 'department/',
                        controller: 'AccountSettingsDepartmentController as ad',
                        parent: 'settings'
                    },
                    templateUrl: '/account/department/'
                }

            ];
        }
    }
})();