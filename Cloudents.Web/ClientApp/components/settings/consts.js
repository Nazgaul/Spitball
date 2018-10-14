import {LanguageService} from "../../services/language/languageService";

export const typesPersonalize = {
    university: "univeristy",
    course: "course",
    whoWeAre: "whoWeAre",
    whatWeUpTo: "whatWeUpTo",
    myWallet: "myWallet",
    trustSafety: "trustSafety",
    aboutSpitball: "aboutSpitball",
    help: "help",
    termsService: "termsService",
    feedback:"feedback",
    privacyPolicy: "privacyPolicy",
    hebrew: "hebrew"

};
export let searchObjects = {
    course: {
        id: typesPersonalize.course,
        placeholder: 'What class are you taking?',
        closeText: "done",
        searchApi: "getCourses",
        defaultFilter: 'all',
        filters: [{id: 'all', name: 'ALL COURSES'}, {id: 'myCourses', name: 'MY COURSES'}],
        click: function () {
            this.val = "";
            this.$refs.searchText.focus();
        },
        action: "add"
    }, 
    university: {
        id: typesPersonalize.university,
        searchApi: "getUniversities",
        placeholder: LanguageService.getValueByKey('result_where_school'),
        closeText: "X",
        click: function (keep = true) {
            if (!keep) {
                this.$parent.$parent.showDialog = false
            } else {
                this.currentType = "course";
            }
        },
        defaultFilter: '',
        filters: []
    }
};

export let settingMenu = [
    {
        id: typesPersonalize.university,
        name:  LanguageService.getValueByKey("settings_university_choose"),
       // name: "Choose university",
        click: function () {
            this.showDialog = true;
            this.type = "";
            this.$nextTick(() => this.type = "university");
            this.keep = true; //keep dialog open after university select
            this.isSearch = true;
        }
    },
    {
        id: typesPersonalize.course,
        name:  LanguageService.getValueByKey("settings_my_courses"),
        // name: "My courses",
        click: function (type) {
            this.showDialog = true;
            this.type = type;
            this.keep = true; //only relevant for university select
            this.isSearch = true;
        }
    },
    {
        //id: "intercom",
        name: "Feedback",
        click: function () {
            Intercom('showNewMessage', '');
        },

    }
];



export let notRegMenu = [
    // {
    //     id: typesPersonalize.hebrew,
    //     title: "עברית",
    //     name:"hebrew",
    //     icon:"sbf-profile"
    // },
    {
        id: typesPersonalize.aboutSpitball,
        title:  LanguageService.getValueByKey("settings_menu_item_about_spitball"),
        // title: "About Spitball",
        name:"about",
        icon:"sbf-about",

    },
    {
        id: typesPersonalize.help,
        title:  LanguageService.getValueByKey("settings_menu_item_help"),
        //title: "Help",
        name:"faq",
        icon:"sbf-help"
    },
    {
        id: typesPersonalize.termsService,
        title:  LanguageService.getValueByKey("settings_menu_item_terms"),
        //title: "Terms of Service",
        name:"terms",
        icon:"sbf-terms"
    },
    {
        id: typesPersonalize.privacyPolicy,
        title:  LanguageService.getValueByKey("settings_menu_item_privacy_policy"),
        //title: "Privacy Policy",
        name:"privacy",
        icon:"sbf-privacy"
    },
    {
        id:typesPersonalize.feedback,
        title:  LanguageService.getValueByKey("settings_menu_item_feedback"),
        // title:'Feedback',
        name: 'feedback',
        icon:"sbf-feedbackNew",
        click:()=>Intercom('showNewMessage'),

    },

   
];