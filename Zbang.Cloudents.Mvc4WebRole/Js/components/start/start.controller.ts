module app {
    "use strict;";

    class StartController {
        term;
        lang;
        luis;
        wit;
        //xintent;
        //data;
        static $inject = ["startService"];
        constructor(private startService: IStartService) {
            console.log("jere");
        }

        intent() {
            this.startService.intent(this.term).then(response => {
                this.lang = response.lang;
                this.luis = {};
                this.wit = {};
                this.luis.data = response.luis;
                this.luis.intent = response.luisIntent;

                this.wit.data = response.wit; 
                this.wit.intent = response.witIntent;
            });
        }
    }

    angular.module("app").controller("StartController", StartController);
}