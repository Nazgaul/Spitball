import recordingIcon from "./svg/recording.svg";
import searchIcon from "./svg/search-icon.svg";
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



let homeSuggest = [
    "Flashcards for financial accounting",
    "Class notes for my Calculus class",
    "When did World War 2 end?",
    "Difference between meiosis and mitosis",
    "Tutor for Linear Algebra",
    "Job in marketing in NYC",
    "The textbook - Accounting: Tools for Decision Making",
    "Where can I get a burger near campus?"
];
let bottomIcons=["facebook","twitter","google","youtube","instegram"];
let strips =
    {

        documents: {
            class: "documents",
            image: "strip-documents.png",
            floatingImages: ['laptopIcon', 'notebookIcon', 'bagIcon'],
            titleIcon: 'stripDocumentIcon',
            title: "Study documents",
            titleColor: "#320161",
            text: "Spitball curates content for you from the best sites on the web. The documents populate based on student ratings and are filtered by your school, classes, and preferences."
        },

        tutors: {
            class: "tutors",
            image: "strip-tutors.png",
            floatingImages: ['tutorIcon', 'discussionIcon','studentLaptopIcon'],
            titleIcon: 'stripTutorIcon',
            title: "Tutors",
            titleColor: "#52ab15",
            text: "Spitball has teamed up with the most trusted tutoring services. All of our online and in-person tutors are highly qualified experts with educations from the best universities around the world."
        },

        foodDeals: {
            class: "food-deals",
            image: "strip-food-deals.png",
            floatingImages: ['shoppingBagsIcon', 'headsetIcon', 'pizzaIcon'],
            titleIcon: "stripFoodDealsIcon",
            title: "Food and Deals",
            titleColor: "#2cbfa5",
            text: "Find and discover exclusive, short-term offers from restaurants around your campus."
        },

        textbooks: {
            class: "textbooks",
            image: "strip-textbook-icon.png",
            floatingImages: ['bookClosedIcon', 'bookStackIcon', 'bookOpenIcon'],
            titleIcon: "stripTextbooksIcon",
            title: "Textbooks",
            titleColor: "#8c3fbf",
            text: "Spitball finds you textbooks at the best prices by simultaneously searching multiple websites. Compare prices to buy, rent, and sell back your books."
        },


        flashcards: {
            class: "flashcards",
            image: "strip-flashcards.png",
            floatingImages: ['flashcardPenIcon', 'flashcardQuestionIcon', 'flashcardGroupIcon'],
            titleIcon: 'stripFlashcardsIcon',
            title: "Flashcards",
            titleColor: "#f14d4d",
            text: "Search millions of study sets and improve your grades by studying with flashcards."
        },


        askQuestion: {
            class: "ask-question",
            image: "strip-ask-question.png",
            floatingImages: ['askQuestionMountainIcon', 'askQuestionHouseIcon', 'askQuestionRocketIcon'],
            titleIcon: 'stripAskQuestionIcon',
            title: "Ask a Question",
            titleColor: "#0455a8",
            text: "Ask any school related question and immediately get answers and information that relates specifically to you, your classes, and your university."
        },

        jobs: {
            class: "jobs",
            image: "strip-jobs.png",
            floatingImages: ['jobsGraphIcon', 'jobsChemistryIcon', 'jobsSlideIcon'],
            titleIcon: 'stripJobsIcon',
            title: "Jobs",
            titleColor: "#f5a623",
            text: "Easily find and apply to paid internships, part-time jobs and entry-level opportunities at thousands of Fortune 500 companies and startups."
        }

    };
let features =
    {

        document: {
            title: "Millions of Documents",
            text: "Find study guides, homework, and notes for courses at your school.",
            mobileText: "study guides and notes."
        },

        textbook: {
            title: "Save up to 50% on Textbooks",
            text: "A new course load doesn’t have to break the bank! Find the best prices to rent, buy, or sell your textbooks.",
            mobileText: "Best prices to rent, buy, or sell."
        },

        homework: {
            title: "Homework Help 24/7",
            text: "Find the answers to all your questions with step-by-step help from expert tutors, in person or online.",
            mobileText: "Step-by-step help from expert tutors."
        },
    }


export default {
    components: {
        "search-icon": searchIcon,
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
            sites: [
                {
                    name: "Quizlet",
                    image: 'quizlet.png'
                },
                {
                    name: "Chegg",
                    image: "chegg.png"
                },
                {
                    name: "Study blue",
                    image: "study-blue.png"
                },
                {
                    name: "Wyzant",
                    image: "wyzant.png"
                },
                {
                    name: "CourseHero",
                    image: "course-hero.png"
                },
                {
                    name: "Wayup",
                    image: "wayup.png"
                }],
            testimonials: [{
                name: "Donna Floyd",
                uni: "Sophomore at UC Davis",
                image: "testimonial-1.jpg",
                testimonial: "As a sophomore, I found myself looking for a new way to communicate and study with my classmates. After searching online, I was shocked that there was no real solution that fit my lifestyle (nothing I could easily access on my phone or computer). Then someone invited me to Spitball... it saved me."
            },
            {
                name: "Jack Harris",
                uni: "Junior at UC Berkeley",
                image: "testimonial-1.jpg",
                testimonial: "There have been many times when I would try to share files or a document with classmates over the school’s portal or other online studying tools, but it was a huge hassle! Then I started using Spitball and everything changed - I was easily able to upload all of my files, even video and audio files!"
            },
            {
                name: "Daniel Kaplan",
                uni: "Senior at UCLA",
                image: "testimonial-1.jpg",
                testimonial: "It’s so nice to have all my notes, questions, answers, lectures, etc. in one place and always available online and in the palm of my hand! Spitball comes in handy when I need to compare class notes. Thank you Spitball for making my life a whole lot easier! I already know whom I have to thank first on graduation day :)"
            },
            {
                name: "Sarah Friedman",
                uni: "Freshman at UC Santa Barbara",
                image: "testimonial-1.jpg",
                testimonial: "Spitball is such a great idea! I love the quizzes - such a great way to interactively test my knowledge of the coursework! The chat feed is really helpful if you miss a class or don't understand the material. During my last semester, I feel like all I did was talk about Spitball! It was funny- people actually thought that it was my job."
            }]

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
