module app {
    "use strict";

    class ShareDialog {
        static $inject = ["$mdDialog", "data", "$window","Analytics"];

        url: string;
        whatappLink: string;
        what;
        socialAction: string;

        constructor(
            private $mdDialog: angular.material.IDialogService,
            private data,
            private $window: angular.IWindowService,
            private analytics: angular.google.analytics.AnalyticsService
        ) {

            this.what = data.what;
            switch (this.what) {
                case 'f':
                    this.socialAction = 'share-flashcard'
                    break;
                case 'b':
                    this.socialAction = 'share-box'
                    break
                default:
                    this.socialAction = 'share'
            }
            this.socialAction = this.what == 'f' ? 'share-flashcard' : 'share-box'
            //angularjs dont have origin in $location
            this.url = location.origin + "/" + data.what + "/" + encodeBase64(data.id);
            this.whatappLink = "whatsapp://send?text=" + encodeURIComponent(this.url);
            function encodeBase64(integer: number) {
                const characterSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                if (integer === 0) { return '0'; }
                let s = '';
                while (integer > 0) {
                    s = characterSet[integer % 62] + s;
                    integer = Math.floor(integer / 62);
                }
                return s;
            };
        }

        close() {
            this.$mdDialog.cancel();
        }
        facebook() {
            this.analytics["send"]('social', "facebook", this.socialAction, this.url);
            
            const shareFb = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(this.url);
            this.$window.open(shareFb, "pop", "width=600, height=400, scrollbars=no");
        }

        whatsapp() {
            this.analytics["send"]('social', "whatsApp", this.socialAction, this.url);
            //ga('send', 'social', "whatsApp", "share", this.url);

            //this.$window.open("whatsapp://send?text=" + this.url, "pop", "width=600, height=400, scrollbars=no");
        }
        twitter() {
            this.analytics["send"]('social', "twitter", this.socialAction, this.url);
            //ga('send', 'social', "twitter", "share", this.url);
            const shareTwiiter = "https://twitter.com/intent/tweet?text=" + encodeURIComponent(this.url);
            this.$window.open(shareTwiiter, "pop", "width=600, height=400, scrollbars=no");
        }
        onSuccess() {
            this.analytics["send"]('social', "copied-link", this.socialAction, this.url);
        }




    }

    angular.module("app").controller("ShareDialog", ShareDialog);
}