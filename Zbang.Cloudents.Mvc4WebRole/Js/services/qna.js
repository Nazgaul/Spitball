    mBox.factory('sQnA',
        ['ajaxService',

        function (ajaxService) {
            function buildPath(path) {
                return '/qna/' + path + '/';
            }

            return {
                list: function (data) {
                    return ajaxService.get('/qna/', data);
                },
                post: {
                    question: function (data) {
                        return ajaxService.post(buildPath('addquestion'), data);
                    },
                    answer: function (data) {
                        return ajaxService.post(buildPath('addanswer'), data);
                    },
                },
                'delete': {
                    question: function (data) {
                        return ajaxService.post(buildPath('deletequestion'), data);
                    },
                    answer: function (data) {
                        return ajaxService.post(buildPath('deleteanswer'), data);
                    },
                    attachment: function (data) {
                        return ajaxService.post(buildPath('removefile'), data);
                    }
                }
            };
        }
        ]);
