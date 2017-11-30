import recordingIcon from "./svg/recording.svg";
import classMaterialIcon from "./svg/class-material-icon-purple.svg";
import tutorIcon1 from "./svg/tutor-icon-green.svg";
import jobIcon from "./svg/jobs-icon-orange.svg";
import textBookIcon from "./svg/textbooks-icon-purple.svg";
import logo from '../../../wwwroot/Images/logo-spitball.svg';

import facebookIcon from "./svg/facebook-icon.svg";
import twitterIcon from "./svg/twitter-icon.svg";
import googleIcon from "./svg/google-icon.svg";
import youtubeIcon from "./svg/youtube-icon.svg";
import instegramIcon from "./svg/instagram-icon.svg";

import menuIcon from "./svg/menu-icon.svg";

import stripDocumentIcon from "./svg/strip-documents-icon.svg";
import bagIcon from "./svg/bag-icon.svg";
import notebookIcon from "./svg/notebook-icon.svg";
import laptopIcon from "./svg/laptop-icon.svg";

import stripTutorIcon from "./svg/strip-tutor-icon.svg"
import discussionIcon from "./svg/discussion-icon.svg"
import tutorIcon from "./svg/tutor-icon.svg"
import studentLaptopIcon from "./svg/student-laptop-icon.svg"

import stripFoodDealsIcon from "./svg/strip-food-deals-icon.svg"
import shoppingBagsIcon from "./svg/shopping-bags-icon.svg"
import headsetIcon from "./svg/headset-icon.svg"
import pizzaIcon from "./svg/pizza-icon.svg"

import stripTextbooksIcon from "./svg/strip-textbooks-icon.svg"
import bookClosedIcon from "./svg/book-closed-icon.svg"
import bookStackIcon from "./svg/book-stack-icon.svg"
import bookOpenIcon from "./svg/book-open-icon.svg"

import stripFlashcardsIcon from "./svg/strip-flashcards-icon.svg"
import flashcardPenIcon from "./svg/flashcard-pen-icon.svg"
import flashcardQuestionIcon from "./svg/flashcard-question-icon.svg"
import flashcardGroupIcon from "./svg/flashcard-group-icon.svg"

import stripAskQuestionIcon from "./svg/strip-ask-question-icon.svg"
import askQuestionMountainIcon from "./svg/ask-question-mountain-icon.svg"
import askQuestionHouseIcon from "./svg/ask-question-house-icon.svg"
import askQuestionRocketIcon from "./svg/ask-question-rocket-icon.svg"

import stripJobsIcon from "./svg/strip-jobs-icon.svg"
import jobsGraphIcon from "./svg/jobs-graph-icon.svg"
import jobsChemistryIcon from "./svg/jobs-chemistry-icon.svg"
import jobsSlideIcon from "./svg/jobs-slide-icon.svg"
import featureDocument from "./svg/feature-documents.svg";
import featureHomework from "./svg/feature-homework.svg";
import featureTextbook from "./svg/feature-textbook.svg";





import featuresSection from './features.vue';
import stripsSection from './horizontalStrip.vue';
import PageLayout from './layout.vue';
//import menuIcon from "./svg/menu-icon.svg";


//"img/quizlet"
//"img/chegg"
//"img/study-blue"
//"img/wyzant"
//"img/course-hero"
//"img/wayup"
import {features,bottomIcons,homeSuggest,strips,sites,testimonials} from './consts'


export default {
    components: {
        logo,PageLayout,
        "class-material-icon": classMaterialIcon,
        "tutor-icon": tutorIcon1,
        "job-icon": jobIcon,
        "text-book-icon": textBookIcon,
        "facebook-icon": facebookIcon,
        "twitter-icon": twitterIcon,
        "google-icon": googleIcon,
        "youtube-icon": youtubeIcon,
        "instegram-icon": instegramIcon, menuIcon, recordingIcon,
        featuresSection,
        stripsSection,
        featureDocument,
        featureHomework,
        featureTextbook,
        stripJobsIcon,
        stripDocumentIcon,
        stripAskQuestionIcon,
        stripFlashcardsIcon,
        stripTutorIcon,
        stripFoodDealsIcon,
        stripTextbooksIcon,
        jobsGraphIcon, jobsChemistryIcon, jobsSlideIcon,
        askQuestionMountainIcon, askQuestionHouseIcon, askQuestionRocketIcon,
        flashcardPenIcon, flashcardQuestionIcon, flashcardGroupIcon,
        bookClosedIcon, bookStackIcon, bookOpenIcon,
        shoppingBagsIcon, headsetIcon, pizzaIcon,
        tutorIcon, discussionIcon,studentLaptopIcon,
        laptopIcon, notebookIcon, bagIcon
    },
    data() {
        return {
            placeholder: "Find study documents, textbooks, deals, tutors and more…",
            items: homeSuggest,
            msg: "",
            bottomIcons:bottomIcons,
            recognition: false,
            isRecording: false,
            drawer: null,
            strips:strips,
            features:features,
            links: [
                {
                    name: "Spitball Guide",
                    link: "#"
                },
                {
                    name: "Key Features",
                    link: "#"
                },
                //{
                //    name: "Shared Documents",
                //    link: "#"
                //},
                {
                    name: "Mobile App",
                    link: "#"
                }
            ],
            sites: sites,
            testimonials: testimonials

        };
    },
    methods: {
        $_voiceDetection() {
            this.recognition.start();

        },
        search() {
            this.$router.push({ name:"result", query: {q:this.msg} });
        },
        selectos(item) {
            this.msg = item;
            this.search();
        },
        //$_imageUrl(image) { return require.context(`~/img/${image}.png`);}
    },

    created() {
        if (this.voiceEnable) {
            var SpeechRecognition = SpeechRecognition || webkitSpeechRecognition;
            this.recognition = new SpeechRecognition();
            this.recognition.lang = "en-US";
            this.recognition.interimResults = false;
            this.recognition.maxAlternatives = 5;
            let _self = this;

            this.recognition.onstart = function () {
                console.log("start");
                _self.isRecording = true;
            };
            this.recognition.onresult = function (event) {
                _self.msg = event.results[0][0].transcript;
                _self.isRecording = false;
            };
        }
    },
    props: {
        metaText: { type: String }
    },

    computed: {
        voiceEnable() { return "webkitSpeechRecognition" in window}
    }
};
