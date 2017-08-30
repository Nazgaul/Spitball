var appf = angular.module("myApp", []);
var MyApp;
(function (MyApp) {
    "use strict";
    var HomeController = (function () {
        function HomeController($scope, $http, $sce) {
            this.$scope = $scope;
            this.$http = $http;
            this.$sce = $sce;
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
                }, 'tutor': {
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
            this.results = {};
            console.log("change");
            this.sublist = [];
            this.options = this.optionDic[item];
            document.getElementById('qFilter').focus();
        };
        HomeController.prototype.search = function () {
            var _this = this;
            this.$http.get("home/search", { params: { category: this.sec, term: this.term } })
                .then(function (response) {
                _this.results = {};
                if (_this.sec == 'ask') {
                    _this.results.video = _this.$sce.trustAsResourceUrl('https://www.youtube.com/embed/EqolSvoWNck');
                    _this.results.items = [
                        { title: "title", content: "content", source: "spitball.com", img: "https://thumbs.dreamstime.com/z/smiley-emoticon-happy-face-72284393.jpg", url: "www.google.com" },
                        { title: "title2", content: "content2", source: "spitball.com", url: "www.google.com" }
                    ];
                }
                console.log("hello");
            });
        };
        HomeController.prototype.resultTemplate = function () {
            if (this.sec === 'ask') {
                return 'item-template.html';
            }
        };
        HomeController.$inject = ["$scope", "$http", "$sce"];
        return HomeController;
    }());
    MyApp.HomeController = HomeController;
    appf.controller("HomeController", HomeController);
})(MyApp || (MyApp = {}));
