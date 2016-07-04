'use strict';
(function () {
    angular.module('app.chat').factory('chatBus', chatBus);
    chatBus.$inject = ['ajaxService'];

    function chatBus(ajaxService) {
        var unreadCount = 0;
        var chatService = {};

        chatService.setUnread = function (count) {
            unreadCount = count;
        };
        chatService.getUnread = function () {
            return unreadCount;
        };

        chatService.messages = function (q,page) {
            return ajaxService.get('/chat/conversation', { q: q ,page: page}, false);
        }
        chatService.chat = function (id, userIds, dateTime, top) {
            console.log('loading from time: ' + dateTime);
            return ajaxService.get('/chat/messages', {
                chatRoom: id,
                userIds: userIds,
                startTime: dateTime,
                //fromId: fromId, // instead dateTime
                top: top, 
                //skip: skip // remove

            },false);
        }
        chatService.preview = function(blob, i) {
            return ajaxService.get('/chat/Preview', {
                blobName: blob,
                index : i
            });
        }
        //chatService.unread = function () {
        //
        //    //return ajaxService.get('chat/unreadcount');
        //}

        chatService.read = function(id) {
            return ajaxService.post('chat/markread', {
                chatRoom: id
            });
        }


        return chatService;
    }
})()