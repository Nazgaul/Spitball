(function () {
    angular.module('app.chat').controller('chatIndicatorController', chatIndicator);
    chatIndicator.$inject = ['chatBus', '$scope'];

    function chatIndicator(chatBus, $scope) {
        var cc = this;

        cc.unread = chatBus.getUnread;
        cc.toggleChat = function () {
            $scope.app.chatDisplayState = $scope.app.chatDisplayState !== 2 ? 2 : 1;
        }
       
    }
})();