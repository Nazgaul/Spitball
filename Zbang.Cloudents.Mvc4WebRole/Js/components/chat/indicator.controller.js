(function () {
    angular.module('app.chat').controller('chatIndicatorController', chatIndicator);
    chatIndicator.$inject = ['chatBus', '$mdSidenav'];

    function chatIndicator(chatBus, $mdSidenav) {
        var cc = this;

        cc.unread = chatBus.getUnread;
        cc.openChat = function () {
            $mdSidenav('chat').open();
        }
       
    }
})();