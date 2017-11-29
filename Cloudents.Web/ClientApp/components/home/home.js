import recordingIcon from "./svg/recording.svg";
import micIcon from "./svg/mic.svg";
import searchIcon from "./svg/search-icon.svg";
import classMaterialIcon from "./svg/class-material-icon-purple.svg";
import tutorIcon from "./svg/tutor-icon-green.svg";
import jobIcon from "./svg/jobs-icon-orange.svg";
import textBookIcon from "./svg/textbooks-icon-purple.svg";
import logo from '../../../wwwroot/Images/logo-spitball.svg';
import facebookIcon from "./svg/facebook-icon.svg";
import twitterIcon from "./svg/twitter-icon.svg";
import googleIcon from "./svg/google-icon.svg";
import youtubeIcon from "./svg/youtube-icon.svg";
import instegramIcon from "./svg/instagram-icon.svg";
import menuIcon from "./svg/menu-icon.svg";
import featuresSection from './features.vue';
import stripsSection from './horizontalStrip.vue';
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

export default {
    components: {
        "mic-icon": micIcon, "search-icon": searchIcon,
        logo,
        "class-material-icon": classMaterialIcon,
        "tutor-icon": tutorIcon,
        "job-icon": jobIcon,
        "text-book-icon": textBookIcon,
        "facebook-icon": facebookIcon,
        "twitter-icon": twitterIcon,
        "google-icon": googleIcon,
        "youtube-icon": youtubeIcon,
        "instegram-icon": instegramIcon, menuIcon, recordingIcon,
        featuresSection,
        stripsSection
    },
    data() {
        return {
            placeholder: "Find study documents, textbooks, deals, tutors and more…",
            items: homeSuggest,
            msg: "",
            recognition: false,
            isRecording: false,
            drawer: null,
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
