module app {
    "use strict;";

    class StartController {
        term;
        wit;
        loading = false;
        sentence;
        //xintent;
        //data;
        static $inject = ["startService"];
        constructor(private startService: IStartService) {
        }

        intent() {
            this.loading = true;
            this.startService.intent(this.term).then(response => {
                this.loading = false;

                //if (!response.data) {
                //    this.sentence = "I don't understand what you want";
                //    return;
                //}

                //this.sentence = response.sentence;
                //this.term = "";
                this.sentence = JSON.stringify(response, null, "\t");

            });
        }
    }

    angular.module("app").controller("StartController", StartController);
}