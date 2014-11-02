mItem.factory('sItem',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/item/' + path + '/';
        }

        return {
            'delete': function (data) {        
                return ajaxService.post(buildPath('Delete'),data);
            },
            load: function (data) {
                return ajaxService.get(buildPath('Load'), data);
            },
            preview: function (data) {
                if (!data.blobName || !(_.isNumber(data.index)) || !(_.isNumber(data.id))  ||   !(_.isNumber(data.boxId))  ) {
                    //TODO: add component to do reload    
                }
                return ajaxService.get(buildPath('Preview'), data);
            },
            addComment: function (data) {
                return ajaxService.post(buildPath('AddComment'), data);
            },
            deleteComment: function (data) {
                return ajaxService.post(buildPath('DeleteComment'), data);
            },
            replyComment: function (data) {
                return ajaxService.post(buildPath('ReplyComment'), data);

            },
            deleteReply: function (data) {
                return ajaxService.post(buildPath('DeleteCommentReply'), data);

            },
            rename: function (data) {
                return ajaxService.post(buildPath('Rename'), data);
            },
            flag: function (data) {
                return ajaxService.post(buildPath('FlagRequest'), data);
            },
            rate: function (data) {
                return ajaxService.post(buildPath('Rate'), data);
            }
        };
    }
    ]);
