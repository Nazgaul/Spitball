var appf = angular.module("myApp", []);
var MyApp;
(function (MyApp) {
    "use strict";
    var HomeController = (function () {
        function HomeController($scope, $http) {
            this.$scope = $scope;
            this.$http = $http;
            this.jobTypes = [{ code: "full", name: "full-time" }, { code: "part", name: "part-time" }, { code: "con", name: "contractor" }];
            this.sourceList = [
                { code: "spitball.com", name: "spitball.com" }, { code: "Quizlet.com", name: "Quizlet.com" }
            ];
            this.relDatsSort = [{ code: "relevance", name: "relevance" }, { code: "date", name: "date" }];
            this.tutorSort = [{ code: "relevance", name: "relevance" }, { code: "price", name: "price" }, { code: "distance", name: "distance" }, { code: "rating", name: "rating" }];
            this.jobSort = [{ code: "distance", name: "distance" }].concat(this.relDatsSort);
            this.myCourses = [{ code: "1234", name: "Phsico" }, { code: "425", name: "calcus" }];
            this.jobOptions = [
                { name: "all", code: "", data: [] }, { name: "job type", code: "job_type", data: this.jobTypes },
                { name: "paid", code: "paid", data: [] }
            ];
            this.tutorOption = [
                {
                    name: "all",
                    code: "all",
                    data: []
                }, {
                    name: "Online",
                    code: "online",
                    data: []
                }, {
                    name: "in person",
                    code: "person",
                    data: []
                }
            ];
            this.flashcardOptions = [
                {
                    name: "all",
                    code: "all",
                    data: []
                },
                {
                    name: "Sources",
                    code: "source",
                    data: this.sourceList
                }
            ];
            this.purchaseOptions = [
                {
                    name: "all",
                    code: "all",
                    data: []
                },
                {
                    name: "open now",
                    code: "open",
                    data: this.sourceList
                }
            ];
            this.docOption = [
                { name: "all", code: "", data: [] }, { name: "my courses", code: "my_course", data: this.myCourses }
            ];
            this.optionDic = {
                'note': { filter: this.docOption, sort: this.relDatsSort }, 'flashcard': {
                    filter: this.flashcardOptions,
                    sort: this.relDatsSort
                }, 'tutor-copy': {
                    filter: this.tutorOption,
                    sort: this.tutorSort
                },
                'jobs': { filter: this.jobOptions, sort: this.jobSort }
            };
            this.isOpen = false;
            this.sec = "ask-copy";
        }
        HomeController.prototype.changeSection = function (item) {
            this.isOpen = false;
            this.sec = item;
            this.options = [];
            console.log("change");
            this.sublist = [];
            this.options = this.optionDic[item];
            document.getElementById('qFilter').focus();
        };
        HomeController.prototype.search = function () {
            //this.$http.get("/search", {'term':this.term}).then(function(response) {
            // console.log(response);
            //})
        };
        return HomeController;
    }());
    HomeController.$inject = ["$scope", "$http"];
    MyApp.HomeController = HomeController;
    appf.controller("HomeController", HomeController);
})(MyApp || (MyApp = {}));
//}
//    homeController.$inject = ['$location'];
//    function homeController($location) {
//        /* jshint validthis:true */
//        var vm = this;
//        vm.title = 'homeController';
//        vm.sec = 'ask-copy';
//        function changeSection(item) {
//            vm.isOpen = false;
//            vm.sec = item;
//        }
//    }
//}
//# sourceMappingURL=home.controller.js.map