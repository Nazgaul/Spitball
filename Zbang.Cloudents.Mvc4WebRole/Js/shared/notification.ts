
declare var Notification: any;
module app {
    "use strict";

    export interface INotificationService {
        send(title: string, message: string, image: string, callback?: any): void,
    }

    class DesktopNotification implements INotificationService {
        static $inject = ['$document', '$mdToast', 'resManager'];
        constructor(private $document: angular.IDocumentService,
            private $mdToast: angular.material.IToastService,
            private resManager: IResManager) {
            this.requestPermission();
        }

        private requestPermission() {
            if (!('Notification' in window)) {
                return;
            }
            Notification.requestPermission(function () {
                //var notification = new Notification("Title", { body: 'HTML5 Web Notification API', icon: 'http://i.stack.imgur.com/Jzjhz.png?s=48&g=1', dir: 'auto' });
                //setTimeout(function () {
                //    notification.close();
                //}, 3000);
            });
        }

        send(title, message, image, callback) {
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
            } else {
                this.showToaster();
            }
        }

        private showToaster() {
            var element = this.$document.find('header')[0];
            console.log('showing toaster');
            this.$mdToast.show(
                this.$mdToast.simple()
                    .textContent(this.resManager.get('toasterChatMessage'))
                    .capsule(true)
                    .position('top right')
                    .parent(element)
                    .hideDelay(2000));
        }

    }


    angular.module("app").service("notificationService", DesktopNotification);
}


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