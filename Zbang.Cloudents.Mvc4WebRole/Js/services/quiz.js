mBox.factory('sQuiz', [
   'ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/Quiz/' + path + '/';
        }
        return {
            data: function (data) {
                return ajaxService.get(buildPath('Data'), data);
            },
            saveAnswers: function (data) {
                return ajaxService.post(buildPath('SaveAnswers'), data);
            },
            discussion: {
                getDiscussion: function (data) {
                    return ajaxService.get(buildPath('Discussion'), data);
                },
                createDiscussion: function (data) {
                    return ajaxService.post(buildPath('CreateDiscussion'), data);
                },
                deleteDiscussion: function (data) {
                    return ajaxService.post(buildPath('DeleteDiscussion'), data);
                }
            },
            create: function (data) {
                return ajaxService.post(buildPath('Create'), data);
            },
            update: function (data) {
                return ajaxService.post(buildPath('Update'), data);
            },
            'delete': function (data) {
                return ajaxService.post(buildPath('Delete'), data);
            },
            save: function (data) {
                return ajaxService.post(buildPath('Save'), data);
            },
            getDraft: function (data) {
                return ajaxService.get(buildPath('GetDraft'), data);
            },
            question: {
                create: function (data) {
                    return ajaxService.post(buildPath('CreateQuestion'), data);
                },
                update: function (data) {
                    return ajaxService.post(buildPath('UpdateQuestion'), data);
                },
                'delete': function (data) {
                    return ajaxService.post(buildPath('DeleteQuestion'), data);
                }
            },
            answer: {
                create: function (data) {
                    return ajaxService.post(buildPath('CreateAnswer'), data);
                },
                update: function (data) {
                    return ajaxService.post(buildPath('UpdateAnswer'), data);
                },
                'delete': function (data) {
                    return ajaxService.post(buildPath('DeleteAnswer'), data);
                },
                markCorrect: function (data) {
                    return ajaxService.post(buildPath('MarkCorrect'), data);

                }
            }          

        };
    }
]);
