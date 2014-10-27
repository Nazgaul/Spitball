    mBox.factory('sQnA',
        ['ajaxService',

        function (ajaxService) {
            function buildPath(path) {
                return '/QnA/' + path + '/';
            }

            return {
                list: function (data) {
                    return ajaxService.get('/qna/', data);
                },
                post: {
                    question: function (data) {
                        return ajaxService.post(buildPath('AddQuestion'), data);
                    },
                    answer: function (data) {
                        return ajaxService.post(buildPath('AddAnswer'), data);
                    },
                },
                'delete': {
                    question: function (data) {
                        return ajaxService.post(buildPath('DeleteQuestion'), data);
                    },
                    answer: function (data) {
                        return ajaxService.post(buildPath('DeleteAnswer'), data);
                    },
                    attachment: function (data) {
                        return ajaxService.post(buildPath('RemoveFile'), data);
                    }
                }
            };
        }
        ]);
