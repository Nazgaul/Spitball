mItem.factory('sItem',
    ['ajaxService',
    
    function (ajaxService) {
        var Item = '/Item/';
        function buildPath(path) {
            return '/Item/' + path + '/';
        }

        return {
            'delete': function (data) {        
                return ajaxService.post(buildPath('Delete'),data);
            },
            load: function (data) {
                return ajaxRequest(data, $http.get, 'Load/');
            },
            preview: function (data) {
                return ajaxRequest(data, $http.get, 'Preview/');
            },
            addComment: function (data) {
                return ajaxRequest(data, $http.post, 'AddComment/');
            },
            deleteComment: function (data) {
                return ajaxRequest(data, $http.post, 'DeleteComment/');
            },
            replyComment: function (data) {
                return ajaxRequest(data, $http.post, 'ReplyComment/');

            },
            deleteReply: function (data) {
                return ajaxRequest(data, $http.post, 'DeleteCommentReply/');

            },
            rename: function (data) {
                return ajaxRequest(data, $http.post, 'Rename/');
            },
            flag: function (data) {
                return ajaxRequest(data, $http.post, 'FlagRequest/');
            },
            rate: function (data) {
                return ajaxRequest(data, $http.post, 'Rate/');
            }
        };
    }
    ]);
