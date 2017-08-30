mBox.factory('sQuiz', [
   'ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/quiz/' + path + '/';
        }
        return {
            data: function (data) {
                return ajaxService.get(buildPath('data'), data);
            },
            saveAnswers: function (data) {
                return ajaxService.post(buildPath('saveanswers'), data);
            },
            discussion: {
                getDiscussion: function (data) {
                    return ajaxService.get(buildPath('discussion'), data);
                },
                createDiscussion: function (data) {
                    return ajaxService.post(buildPath('creatediscussion'), data);
                },
                deleteDiscussion: function (data) {
                    return ajaxService.post(buildPath('deletediscussion'), data);
                }
            },
            create: function (data) {
                return ajaxService.post(buildPath('create'), data);
            },
            update: function (data) {
                return ajaxService.post(buildPath('update'), data);
            },
            'delete': function (data) {
                return ajaxService.post(buildPath('delete'), data);
            },
            save: function (data) {
                return ajaxService.post(buildPath('save'), data);
            },
            getDraft: function (data) {
                return ajaxService.get(buildPath('getdraft'), data);
            },
            question: {
                create: function (data) {
                    return ajaxService.post(buildPath('createquestion'), data);
                },
                update: function (data) {
                    return ajaxService.post(buildPath('updatequestion'), data);
                },
                'delete': function (data) {
                    return ajaxService.post(buildPath('deletequestion'), data);
                }
            },
            answer: {
                create: function (data) {
                    return ajaxService.post(buildPath('createanswer'), data);
                },
                update: function (data) {
                    return ajaxService.post(buildPath('updateanswer'), data);
                },
                'delete': function (data) {
                    return ajaxService.post(buildPath('deleteanswer'), data);
                },
                markCorrect: function (data) {
                    return ajaxService.post(buildPath('markcorrect'), data);

                }
            }          

        };
    }
]);
