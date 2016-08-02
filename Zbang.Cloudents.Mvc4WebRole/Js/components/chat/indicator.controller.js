(function () {
    angular.module('app.chat').controller('chatIndicatorController', chatIndicator);
    chatIndicator.$inject = ['chatBus', '$scope'];

    function chatIndicator(chatBus, $scope) {
        var cc = this;

        cc.unread = chatBus.getUnread;
        cc.openChat = function () {
            $scope.app.chatDisplayState = 2;
        }
       
    }
})();