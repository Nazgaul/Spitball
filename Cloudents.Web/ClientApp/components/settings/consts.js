﻿import {LanguageService} from "../../services/language/languageService";

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



export let notRegMenu = [
    {
        id: typesPersonalize.aboutSpitball,
        title:  LanguageService.getValueByKey("settings_menu_item_about_spitball"),
        name:"about",
        icon:"sbf-about"

    },
    {
        id: typesPersonalize.help,
        title:  LanguageService.getValueByKey("settings_menu_item_help"),
        name:"faq",
        icon:"sbf-help"
    },
    {
        id: typesPersonalize.termsService,
        title:  LanguageService.getValueByKey("settings_menu_item_terms"),
        name:"terms",
        icon:"sbf-terms"
    },
    {
        id: typesPersonalize.privacyPolicy,
        title:  LanguageService.getValueByKey("settings_menu_item_privacy_policy"),
        name:"privacy",
        icon:"sbf-privacy"
    },
    {
        id:typesPersonalize.feedback,
        title:  LanguageService.getValueByKey("settings_menu_item_feedback"),
        name: 'feedback',
        icon:"sbf-feedbackNew",
        click:()=>Intercom('showNewMessage')

    }

   
];