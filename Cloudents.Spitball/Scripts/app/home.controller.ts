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
		static $inject = ["$scope", "$http"];
		isOpen: boolean;
        sec: string;
        term: string;
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
            }, 'tutor-copy': {
	            filter: this.tutorOption,
	            sort: this.tutorSort
            },
            'jobs': { filter: this.jobOptions, sort: this.jobSort}
        };

		options: IOptionItem[];

        constructor(private $scope: angular.IScope, private $http: angular.IHttpService) {
			this.isOpen = false;
            this.sec = "ask-copy";
		}

		changeSection(item) {
			this.isOpen = false;
			this.sec = item;
			this.options= [];
			console.log("change");
			this.sublist = [];
			this.options = this.optionDic[item];
			document.getElementById('qFilter').focus();
        }
		search() {
            //this.$http.get("/search", {'term':this.term}).then(function(response) {
	           // console.log(response);
            //})
		}
	}

	appf.controller("HomeController", HomeController as any);
}

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
