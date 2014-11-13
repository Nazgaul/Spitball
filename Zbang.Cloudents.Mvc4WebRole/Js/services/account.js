mAccount.factory('sAccount',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/Account/' + path + '/';
        }
        return {
            details: function (data) {
                return ajaxService.get(buildPath('Details'), data);
            },
            settings: {
                data: function () {
                    return ajaxService.get(buildPath('SettingsData'));
                }
            },
            firstTime: function () {
                return ajaxService.post(buildPath('FirstTime'),data);
            },            
            changePassword: function (data) {
                return ajaxService.post(buildPath('ChangePassword'),data);
            },
            changeEmail: function (data) {
                return ajaxService.post(buildPath('ChangeEmail'), data);
            },
            changeProfile: function (data) {
                return ajaxService.post(buildPath('ChangeProfile'), data);
            },
            changeLanguage: function (data) {
                return ajaxService.post(buildPath('ChangeLanguage'), data);
            },
            submitCode: function(data) {
                return ajaxService.post(buildPath('EnterCode'), data);
            }
        };


    }]
);