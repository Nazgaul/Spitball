    mBox.factory('sUpload',
        ['ajaxService',

        function (ajaxService) {
            function buildPath(path) {
                return '/upload/' + path + '/';
            }

            return {
                'link': function (data) {
                    return ajaxService.post(buildPath('link'), data);
                },
                'dropbox': function (data) {
                    return ajaxService.post(buildPath('dropbox'), data);
                }
            };
        }
        ]);
