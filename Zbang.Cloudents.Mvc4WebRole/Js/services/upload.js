    mBox.factory('sUpload',
        ['ajaxService',

        function (ajaxService) {
            function buildPath(path) {
                return '/Upload/' + path + '/';
            }

            return {
                'link': function (data) {
                    return ajaxService.post(buildPath('Link'), data);
                },
                'dropbox': function (data) {
                    return ajaxService.post(buildPath('Dropbox'), data);
                }
            };
        }
        ]);
