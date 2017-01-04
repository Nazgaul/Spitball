var app;
(function (app) {
    "use strict";
    var DesktopNotification = (function () {
        function DesktopNotification($document, $mdToast, resManager) {
            this.$document = $document;
            this.$mdToast = $mdToast;
            this.resManager = resManager;
            this.requestPermission();
        }
        DesktopNotification.prototype.requestPermission = function () {
            if (!('Notification' in window)) {
                return;
            }
            Notification.requestPermission(function () {
            });
        };
        DesktopNotification.prototype.send = function (title, message, image, callback) {
            if (!('Notification' in window)) {
                this.showToaster();
                return;
            }
            image = image || '/Images/3rdParty/logo_120_4_google.png';
            if (Notification.permission === "granted") {
                var notification = new Notification(title, {
                    body: message || '', icon: image
                });
                notification.onclick = function () {
                    window.focus();
                    if (callback) {
                        callback();
                    }
                    notification.close();
                };
            }
            else {
                this.showToaster();
            }
        };
        DesktopNotification.prototype.showToaster = function () {
            var element = this.$document.find('header')[0];
            this.$mdToast.show(this.$mdToast.simple()
                .textContent(this.resManager.get('toasterChatMessage'))
                .capsule(true)
                .position('top right')
                .parent(element)
                .hideDelay(2000));
        };
        return DesktopNotification;
    }());
    DesktopNotification.$inject = ['$document', '$mdToast', 'resManager'];
    angular.module("app").service("notificationService", DesktopNotification);
})(app || (app = {}));
//# sourceMappingURL=notification.js.map