"use strict";
//declare var Notification: any;
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
                //var notification = new Notification("Title", { body: 'HTML5 Web Notification API', icon: 'http://i.stack.imgur.com/Jzjhz.png?s=48&g=1', dir: 'auto' });
                //setTimeout(function () {
                //    notification.close();
                //}, 3000);
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
//(function () {
//    'use strict';
//    angular.module('app').service('notificationService', desktopNotification);
//    desktopNotification.$inject = ['$document', '$mdToast', 'resManager'];
//    function desktopNotification($document, $mdToast, resManager) {
//        requestPermission();
//        function requestPermission() {
//            if (!('Notification' in window)) {
//                return;
//            }
//            Notification.requestPermission(function () {
//                //var notification = new Notification("Title", { body: 'HTML5 Web Notification API', icon: 'http://i.stack.imgur.com/Jzjhz.png?s=48&g=1', dir: 'auto' });
//                //setTimeout(function () {
//                //    notification.close();
//                //}, 3000);
//            });
//        }
//        this.send = function (title, message, image, callback) {
//            if (!('Notification' in window)) {
//                showToaster();
//                return;
//            }
//            image = image || '/Images/3rdParty/logo_120_4_google.png';
//            if (Notification.permission === "granted") {
//                var notification = new Notification(title, {
//                    body: message || '', icon: image
//                });
//                notification.onclick = function () {
//                    window.focus();
//                    if (callback) {
//                        callback();
//                    }
//                    notification.close();
//                };
//            } else {
//                showToaster();
//            }
//        }
//        function showToaster() {
//            var element = $document.find('header')[0];
//            $mdToast.show(
//                $mdToast.simple()
//                    .textContent(resManager.get('toasterChatMessage'))
//                    .capsule(true)
//                    .position('top right')
//                    .parent(element)
//                    .hideDelay(2000));
//        }
//    }
//})() 
//# sourceMappingURL=notification.js.map