var appf = angular.module("myApp", []);

module MyApp {
	"use strict";

	export interface IStoreListController {
		isOpen: boolean;
		sec: string;
	}

	interface IOptionItemVal {
		code: string;
		name: string;
	}

	interface IOptionItem {
		code: string;
		name: string;
        data: IOptionItemVal[];
	}

	export class HomeController {
		static $inject = ["$scope", "$http","$sce"];
		isOpen: boolean;
        sec: string;
        term: string;
        results;
		sublist: IOptionItemVal[];
        private jobTypes: IOptionItemVal[] = [{ code: "full", name: "full-time" }, { code: "part", name: "part-time" }, { code: "con", name: "contractor" }];
		private sourceList: IOptionItemVal[] = [
            { code: "spitball.com", name: "spitball.com" }, { code: "Quizlet.com", name: "Quizlet.com" }
        ];
        private relDatsSort: IOptionItemVal[]=[{code:"relevance",name:"relevance"}, {code:"date",name:"date"}];
        private tutorSort: IOptionItemVal[] = [{ code: "relevance", name: "relevance" }, { code: "price", name: "price" }, { code: "distance", name: "distance" }, { code: "rating", name: "rating" }];
        private jobSort: IOptionItemVal[] = [{ code: "distance", name: "distance" }].concat(this.relDatsSort);
        private myCourses: IOptionItemVal[] = [{ code: "1234", name: "Phsico" }, { code: "425", name: "calcus" }];
		private jobOptions: IOptionItem[] = [
			{ name: "all", code: "", data: [] }, { name: "job type", code: "job_type", data: this.jobTypes },
			{ name: "paid", code: "paid", data: [] }
		];
		private tutorOption: IOptionItem[] = [
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
		private flashcardOptions: IOptionItem[] = [
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
        private purchaseOptions: IOptionItem[] = [
	        {
		        name: "all",
		        code: "all",
		        data: []
	        },
	        {
		        name: "open now",
		        code: "open",
		        data: this.sourceList
	        }];
		private docOption: IOptionItem[] = [
			{ name: "all", code: "", data: [] }, { name: "my courses", code: "my_course", data: this.myCourses }
		];


        private optionDic: any = {
            'note': { filter: this.docOption, sort: this.relDatsSort}, 'flashcard': {
                filter: this.flashcardOptions,
                sort: this.relDatsSort
            }, 'tutor': {
	            filter: this.tutorOption,
	            sort: this.tutorSort
            },
            'jobs': { filter: this.jobOptions, sort: this.jobSort}
        };

		options: IOptionItem[];

        constructor(private $scope: angular.IScope, private $http: angular.IHttpService,private $sce: ng.ISCEService) {
			this.isOpen = false;
            this.sec = "ask";
		}

		changeSection(item) {
			this.isOpen = false;
			this.sec = item;
            this.options = [];
            this.results = {};
			console.log("change");
			this.sublist = [];
			this.options = this.optionDic[item];
			document.getElementById('qFilter').focus();
        }

		search() {
			this.$http.get("home/search", { params: { category: this.sec, term: this.term } })
                .then(response => {
                    this.results = {};
                    if (this.sec === "purchase") {
                        this.results.items = [
                            { name: "burger", location: "פארק המדע HaMada St 1, Rehovot ", rate: 3.4, status: "Open", img: "http://www.kaminarieducation.net/interclass/img/purchase_s2.jpg", url: "http://www.interclass.us/purchase" },
                            { name: "cafe luiz", location: "פארק המדע מאיר ויסגל 2, Rehovot", rate: 4.2, status: "closed", url: "www.google.com" }
                        ];

                    } else {
                        //if (this.sec == 'ask') {
                        this.results.video = this.$sce.trustAsResourceUrl('https://www.youtube.com/embed/EqolSvoWNck');
                        this.results.items = [
                            { title: "title", content: "content", source: "spitball.com", img: "https://thumbs.dreamstime.com/z/smiley-emoticon-happy-face-72284393.jpg", url: "www.google.com" },
                            { title: "title2", content: "content2", source: "spitball.com", url: "www.google.com" }

                        ]
                        //}
                    }
					console.log("hello");
				});
        }
        resultName() {
            var sec_items = ['ask', 'flashcard', 'note'];
            if (sec_items.indexOf(this.sec) > -1) {
                return 'item';
            } else {
                return this.sec;
            }
        }
	}
    appf.filter('firstLetters', function () {
        return function (input, scope) {
            return input.match(/\b(\w)/g).join('');
        }
    });
	appf.controller("HomeController", HomeController as any);
}

