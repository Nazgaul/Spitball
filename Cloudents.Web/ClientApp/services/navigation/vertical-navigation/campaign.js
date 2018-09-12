import { LanguageService } from "../../language/languageService";

const navDefault = {
    ask: {
        banner:{
            "lineColor": "#00c0fa",
            "title" : LanguageService.getValueByKey("navigation_campaign_default_ask_banner_title"),
            "textMain" :LanguageService.getValueByKey("navigation_campaign_default_ask_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_default_ask_banner_boldText"),
            url: '',
            showOverlay: true
        },

    },
    note: {
        banner:{
            "lineColor": "#943bfd",
            "title" : LanguageService.getValueByKey("navigation_campaign_default_note_banner_title"),
            "textMain" :LanguageService.getValueByKey("navigation_campaign_default_note_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_default_note_banner_boldText"),
             url: '',
            showOverlay: true
        },

     },
    flashcard: {
        banner:{
            "lineColor": "#f14d4d",
            "title" : LanguageService.getValueByKey("navigation_campaign_default_flashcard_banner_title"),
            "textMain" :LanguageService.getValueByKey("navigation_campaign_default_flashcard_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_default_flashcard_banner_boldText"),
            url: '',
            showOverlay: true
        },

    },
    tutor: {
        banner: {
            "lineColor": "#52aa16",
            "title" : LanguageService.getValueByKey("navigation_campaign_tutor_default_banner_title"),
            "textMain" :LanguageService.getValueByKey("navigation_campaign_tutor_default_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_default_tutor_banner_boldText"),
            url: '',
            showOverlay: true
        },

    },
    book: {
        banner:{
            "lineColor": "#a650e0",
            "title" : LanguageService.getValueByKey("navigation_campaign_book_default_banner_title"),
            "textMain" :LanguageService.getValueByKey("navigation_campaign_book_default_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_book_default_banner_boldText"),
            url: '',
            showOverlay: true
        },

    },
    job: {
        banner:{
            "lineColor": "#f49c20",
            "title" : LanguageService.getValueByKey("navigation_campaign_job_default_banner_title"),
            "textMain" :LanguageService.getValueByKey("navigation_campaign_job_default_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_job_default_banner_boldText"),
            url: '',
            showOverlay: true
        },
    },
};

const askaquestion= {
    ask: {
        banner:{
            "lineColor": "#00c0fa",
            "title" : LanguageService.getValueByKey("navigation_campaign_askaquestion_ask_banner_title"),
            "textMain" : LanguageService.getValueByKey("navigation_campaign_askaquestion_ask_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_askaquestion_ask_banner_boldText"),
            url: '/register',
            showOverlay: false,
            campaignClass: 'ask-askaquestion‎',
        },
    },
    note: {
        banner:{
            "lineColor": "#943bfd",
            "title" :    LanguageService.getValueByKey("navigation_campaign_askaquestion_note_banner_title"),
            "textMain" : LanguageService.getValueByKey("navigation_campaign_askaquestion_note_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_askaquestion_note_banner_boldText"),
            url: '/register',
            showOverlay: true,
            campaignClass: 'note-askaquestion‎',

        }
     },
    flashcard: {
        banner:{
            "lineColor": "#f14d4d",
            "title" :    LanguageService.getValueByKey("navigation_campaign_askaquestion_flashcard_banner_title"),
            "textMain" : LanguageService.getValueByKey("navigation_campaign_askaquestion_flashcard_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_askaquestion_flashcard_banner_boldText"),
            url: '/register',
            showOverlay: true,
            campaignClass: 'flashcard-askaquestion‎',
        }
    },
    tutor: {
        banner: {
            "lineColor": "#52aa16",
            "title" :    LanguageService.getValueByKey("navigation_campaign_askaquestion_tutor_banner_title"),
            "textMain" : LanguageService.getValueByKey("navigation_campaign_askaquestion_tutor_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_askaquestion_tutor_banner_boldText"),
            url: '/register',
            showOverlay: true,
            campaignClass: 'tutor-askaquestion‎',
        }
    },
    book: {
        banner:{
            "lineColor": "#a650e0",
            "title" :    LanguageService.getValueByKey("navigation_campaign_askaquestion_book_banner_title"),
            "textMain" : LanguageService.getValueByKey("navigation_campaign_askaquestion_book_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_askaquestion_book_banner_boldText"),
            url: '/register',
            showOverlay: true,
            campaignClass: 'book-askaquestion‎',
        }
    },
    job: {
        banner:{
            "lineColor": "#f49c20",
            "title" :    LanguageService.getValueByKey("navigation_campaign_askaquestion_job_banner_title"),
            "textMain" : LanguageService.getValueByKey("navigation_campaign_askaquestion_job_banner_textMain"),
            "boldText" : LanguageService.getValueByKey("navigation_campaign_askaquestion_job_banner_boldText"),
            url: '/register',
            showOverlay: true,
            campaignClass: 'job-askaquestion‎',
        }
    },

};
export const bannerData = {
    navDefault,
    askaquestion,

};