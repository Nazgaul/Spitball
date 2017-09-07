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
            this.sec = "ask";
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
                if (_this.sec === "purchase") {
                    _this.results.items = [
                        { name: "burger", location: "פארק המדע HaMada St 1, Rehovot ", rate: 3.4, status: "Open", img: "http://www.kaminarieducation.net/interclass/img/purchase_s2.jpg", url: "http://www.interclass.us/purchase" },
                        { name: "cafe luiz", location: "פארק המדע מאיר ויסגל 2, Rehovot", rate: 4.2, status: "closed", url: "www.google.com" }
                    ];
                }
                else if (_this.sec === "tutor") {
                    _this.results.items = [
                        { firstName: "Yifat", place: "Burgas,Bulgaria", distance: "22.5", courseType: "online", lastName: "Biezuner", price: "15", img: "http://www.kaminarieducation.net/interclass/img/purchase_s2.jpg", url: "http://www.interclass.us/purchase" },
                        { firstName: "Ram", lastName: "Yaari", place: "Berlin,Germany", distance: "253305", courseType: "offline", price: "55", url: "www.google.com" }
                    ];
                }
                else if (_this.sec === "jobs") {
                    _this.results.items = [
                        { title: "College Students - Online Media Journalist", desc: "Publish high-quality news (topics of your choice), having the chance to use a variety of formats (written articles, videos). Share and promote the news through social networks to increae the number of the readers. If", company: "Microsoft", location: "Berlin,Germany", jobType: "paid", startTime: "Now", url: "http://www.interclass.us/purchase" },
                        { title: "Job num 2", desc: "mats (written articles, videos). Share and promote the news through social networks to increae the number of the readers. If", company: "Microsoft", location: "Berlin,Germany", jobType: "paid", startTime: "Now", url: "http://www.interclass.us/purchase" }
                    ];
                }
                else if (_this.sec === "book") {
                    _this.results.items = [
                        { title: "Harry Potter", binding: "Paperback", edition: "5th", isbn13: 9781305631946, isbn10: 1305631943, author: "J.K Rolling", url: "http://www.interclass.us/purchase", img: "http://www.publish.csiro.au/covers-high/7524.jpg" },
                        { title: "Economics in One Lesson: The Shortest and Surest Way to Understand Basic Economisc", binding: "Paperback", edition: "5th", isbn13: 9781305631946, isbn10: 1305631943, author: "J.K Rolling", url: "http://www.interclass.us/purchase" },
                    ];
                }
                else {
                    //if (this.sec == 'ask') {
                    _this.results.video = _this.$sce.trustAsResourceUrl('https://www.youtube.com/embed/EqolSvoWNck');
                    _this.results.items = [
                        { title: "title", content: "content", source: "spitball.com", img: "https://thumbs.dreamstime.com/z/smiley-emoticon-happy-face-72284393.jpg", url: "www.google.com" },
                        { title: "title2", content: "content2", source: "spitball.com", url: "www.google.com" }
                    ];
                    //}
                }
                console.log("hello");
            });
        };
        HomeController.prototype.resultName = function () {
            var sec_items = ['ask', 'flashcard', 'note'];
            if (sec_items.indexOf(this.sec) > -1) {
                return 'item';
            }
            else {
                return this.sec;
            }
        };
        HomeController.$inject = ["$scope", "$http", "$sce"];
        return HomeController;
    }());
    MyApp.HomeController = HomeController;
    appf.filter('firstLetters', function () {
        return function (input, scope) {
            return input.match(/\b(\w)/g).join('');
        };
    });
    appf.controller("HomeController", HomeController);
})(MyApp || (MyApp = {}));
//# sourceMappingURL=home.controller.js.map