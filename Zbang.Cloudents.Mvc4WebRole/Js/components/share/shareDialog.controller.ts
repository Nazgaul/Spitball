module app {
    "use strict";

    class ShareDialog {
        constructor() {
            console.log("here");
        }

        url = "hi";
    }

    angular.module("app").controller("ShareDialog", ShareDialog);
}