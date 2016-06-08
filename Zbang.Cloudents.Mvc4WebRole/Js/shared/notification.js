(function () {


    angular.module('app').service('notificationService', desktopNotification);
    desktopNotification.$inject = ['$document', '$mdToast', 'resManager'];

    function desktopNotification($document, $mdToast, resManager) {
        if (!('Notification' in window)) {
            return;
        }
        Notification.requestPermission(function (permission) {
            //var notification = new Notification("Title", { body: 'HTML5 Web Notification API', icon: 'http://i.stack.imgur.com/Jzjhz.png?s=48&g=1', dir: 'auto' });
            //setTimeout(function () {
            //    notification.close();
            //}, 3000);
        });

        this.send = function(title, message, image)
        {
            if (!('Notification' in window)) {
                showToaster();
                return;
            }
            image = image || '/Images/3rdParty/logo_120_4_google.png';
            if (Notification.permission === "granted") {
                var notification = new Notification(title, { body: message || '', icon: image });
            } else {
                showToaster();
            }
        }

        function showToaster() {
            var element = $document.find('header')[0];

            $mdToast.show(
                 $mdToast.simple()
                 .textContent(resManager.get('toasterChatMessage'))
                 .capsule(true)
                 .position('top right')
                 .parent(element)
                 .hideDelay(2000));
        }

    }

})()