module app {
    "use strict";



    class ShareDialog {
        static $inject = ["$mdDialog", "data"];

        url: string;

        constructor(
            private $mdDialog: angular.material.IDialogService,
            private data
        ) {

            //angularjs dont have origin in $location
            this.url = location.origin + "/" + data.what + "/" + encodeBase64(data.id);

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





    }

    angular.module("app").controller("ShareDialog", ShareDialog);
}