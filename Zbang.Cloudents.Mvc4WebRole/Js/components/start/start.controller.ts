module app {
    "use strict;";

    class StartController {
        term;
        lang;
        xintent;
        data;
        static $inject = ["startService"];
        constructor(private startService: IStartService) {
            console.log("jere");
        }

        intent() {
            this.startService.intent(this.term).then(response => {
                this.lang = response.lang;
                this.data = JSON.stringify(response.data);
                this.xintent = response.intent;
            });
        }
    }

    angular.module("app").controller("StartController", StartController);
}