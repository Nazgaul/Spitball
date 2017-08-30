mItem.factory('sItem',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/item/' + path + '/';
        }

        return {
            'delete': function (data) {        
                return ajaxService.post(buildPath('delete'),data);
            },
            load: function (data) {
                return ajaxService.get(buildPath('load'), data);
            },
            preview: function (data) {
                if (!data.blobName || !(_.isNumber(data.index)) || !(_.isNumber(data.id))  ||   !(_.isNumber(data.boxId))  ) {
                    //TODO: add component to do reload    
                }
                return ajaxService.get(buildPath('preview'), data);
            },
            addComment: function (data) {
                return ajaxService.post(buildPath('addcomment'), data);
            },
            deleteComment: function (data) {
                return ajaxService.post(buildPath('deletecomment'), data);
            },
            replyComment: function (data) {
                return ajaxService.post(buildPath('replycomment'), data);

            },
            deleteReply: function (data) {
                return ajaxService.post(buildPath('deletecommentreply'), data);

            },
            rename: function (data) {
                return ajaxService.post(buildPath('rename'), data);
            },
            flag: function (data) {
                return ajaxService.post(buildPath('flagrequest'), data);
            },
            rate: function (data) {
                return ajaxService.post(buildPath('rate'), data);
            }
        };
    }
    ]);
