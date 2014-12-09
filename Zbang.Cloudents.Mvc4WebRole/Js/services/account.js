mAccount.factory('sAccount',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/account/' + path + '/';
        }
        return {
            details: function (data) {
                return ajaxService.get(buildPath('details'), data);
            },
            settings: {
                data: function () {
                    return ajaxService.get(buildPath('settingsdata'));
                }
            },
            firstTime: function (data) {
                return ajaxService.post(buildPath('firsttime'),data);
            },            
            changePassword: function (data) {
                return ajaxService.post(buildPath('changepassword'),data);
            },
            changeEmail: function (data) {
                return ajaxService.post(buildPath('changeemail'), data);
            },
            changeProfile: function (data) {
                return ajaxService.post(buildPath('changeprofile'), data);
            },
            changeLanguage: function (data) {
                return ajaxService.post(buildPath('changelanguage'), data);
            },
            submitCode: function(data) {
                return ajaxService.post(buildPath('entercode'), data);
            },
            facebookLogin: function (data) {
                return ajaxService.post(buildPath('FacebookLogin'), data);
            },

        };


    }]
);