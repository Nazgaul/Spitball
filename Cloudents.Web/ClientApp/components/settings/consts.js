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

export let settingMenu = [
    {
        id: typesPersonalize.university,
        name:  LanguageService.getValueByKey("settings_university_choose"),
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
        }

    }
];