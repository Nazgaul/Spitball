let homeSuggest = [
    "Flashcards for financial accounting",
    "Class notes for my Calculus class",
    "When did World War 2 end?",
    "Difference between Meiosis and Mitosis",
    "Tutor for Linear Algebra",
    "Job in marketing in NYC",
    "The textbook - Accounting: Tools for Decision Making",
    "Where can I get a burger near campus?"
];
let bottomIcons = [
    {
        link: "https://www.facebook.com/spitballstudy/",
        img: "facebook",
        svg: () => import("./svg/facebook-icon.svg")
    },
    {
        link: "https://twitter.com/SpitballStudy",
        img: "twitter",
        svg: () => import("./svg/twitter-icon.svg")
    },
    {
        link: "https://plus.google.com/+Cloudents",
        img: "google",
        svg: () => import("./svg/google-icon.svg")
    },
    {
        link: "https://www.youtube.com/channel/UCamYabfxHUP3A9EFt1p94Lg",
        img: "youtube",
        svg: () => import("./svg/youtube-icon.svg")
    },
    {
        link: "https://www.instagram.com/spitballstudy/",
        img: "instegram",
        svg: () => import("./svg/instagram-icon.svg")
    },
];



let strips =
    {
        documents: {
            class: "documents",
            image: "strip-documents.png",
            floatingImages: {
                svg1: () => import("./svg/laptop-icon.svg"),
                svg2: () => import("./svg/notebook-icon.svg"),
                svg3: () => import("./svg/bag-icon.svg")
            },
            titleIcon: () => import("./img/document.svg"),
            title: "Study Documents",
            link:"/note",
            text: "Spitball curates study documents from the best sites on the web. Our notes, study guides and exams populate based on student ratings and are filtered by your school, classes and preferences."
        },

        flashcards: {
            class: "flashcards",
            image: "strip-flashcards.png",
            floatingImages: {
                svg1: () => import("./svg/flashcard-pen-icon.svg"),
                svg2: () => import("./svg/flashcard-question-icon.svg"),
                svg3: () => import("./svg/flashcard-group-icon.svg")
            },
            titleIcon: () => import("./img/flashcard.svg"),
            title: "Flashcards",
            link:"/flashcard",
            text: "Search millions of study sets and improve your grades by studying with flashcards."
        },

        tutors: {
            class: "tutors",
            image: "strip-tutors.png",
            floatingImages: {
                svg1: () => import("./svg/tutor-icon.svg"),
                svg2: () => import("./svg/discussion-icon.svg"),
                svg3: () => import("./svg/student-laptop-icon.svg")
            },
            titleIcon: () => import("./img/tutor.svg"),
            title: "Tutors",
            link:"/tutor",
            text: "Spitball has teamed up with the most trusted tutoring services to help you ace your classes."// All of our online and in-person tutors are highly qualified experts with educations from some of the best universities in the world."
        },

        textbooks: {
            class: "textbooks",
            image: "strip-textbook-icon.png",
            floatingImages: {
                svg1: () => import("./svg/book-closed-icon.svg"),
                svg2: () => import("./svg/book-stack-icon.svg"),
                svg3: () => import("./svg/book-open-icon.svg")
            },
            titleIcon: () => import("./img/book.svg"),
            title: "Textbooks",
            link:"/book",
            text: "Find the best prices to buy, rent and sell your textbooks by comparing hundreds of sites simultaneously."
        },

        askQuestion: {
            class: "ask-question",
            image: "strip-ask-question.png",
            floatingImages: {
                svg1: () => import("./svg/ask-question-mountain-icon.svg"),
                svg2: () => import("./svg/ask-question-house-icon.svg"),
                svg3: () => import("./svg/ask-question-rocket-icon.svg")
            },
            titleIcon: () => import("./img/ask.svg"),
            title: "Ask A Question",
            link:"/ask",
            text: "Ask any school related question and immediately get answers and information that relates specifically to you, your classes, and your university."
        },

        jobs: {
            class: "jobs",
            image: "strip-jobs.png",
            floatingImages: {
                svg1: () => import("./svg/jobs-graph-icon.svg"),
                svg2: () => import("./svg/jobs-chemistry-icon.svg"),
                svg3: () => import("./svg/jobs-slide-icon.svg")
            },
            titleIcon: () => import("./img/job.svg"),
            title: "Jobs",
            link:"/job",
            text: "Easily search and apply to paid internships, part-time jobs and entry-level opportunities from local businesses all the way to Fortune 500 companies."
        },

        foodDeals: {
            class: "food-deals",
            image: "strip-food-deals.png",
            floatingImages: {
                svg1: () => import("./svg/shopping-bags-icon.svg"),
                svg2: () => import("./svg/headset-icon.svg"),
                svg3: () => import("./svg/pizza-icon.svg")
            },
            titleIcon: () => import("../food/svg/food.svg"),
            title: "Food and Deals",
            link:"/food",
            text: "Discover exclusive deals to local businesses, restaurants and bars near campus."
        }


    };
let features =
    {

        document: {
            title: "Millions of Documents",
            text: "Find study guides, homework, practice exams and notes for courses at your school.",
            icon: () => import("./svg/notebook-icon.svg")
        },

        textbook: {
            title: "Save up to 50% on Textbooks",
            text: "A new course load doesn’t have to break the bank! Find the best prices to rent, buy or sell your textbooks.",
            icon: () => import("./svg/book-stack-icon.svg")
        },

        homework: {
            title: "Homework Help 24/7",
            text: "Find the answers to all of your questions with help from expert tutors in person or online.",
            icon: () => import("./svg/student-laptop-icon.svg")
        },
    }
let sites = [
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
    }];
let testimonials = [{
    name: "Donna Floyd",
    uni: "Sophomore at Rutgers",
    testimonial: "After a tough freshman year, I found myself looking for new resources to help me stay organized and do better in school. Then a friend told me about Spitball - practice exams, study guides and more from the actual classes at my school. You saved me Spitball!"
},
{
    name: "Jack Harris",
    uni: "Junior at Penn State",
    testimonial: "There have been too many times when I spent countless hours searching the Internet for online study tools and class material. Finally, I found Spitball and have everything I need for college all in one place."
},
{
    name: "Daniel Kaplan",
    uni: "Senior at Michigan",
    testimonial: "It’s so nice to have all of my notes, questions, tutors and more in one place and always available. Spitball comes in handy when I miss class and need to find notes or homework help. Thank you for making my life a whole lot easier!"
},
{
    name: "Sarah Friedman",
    uni: "Freshman at Syracuse",
    testimonial: "Spitball is such a great idea! No more searching 100 different sites for class notes and study material. Plus, you can’t beat the textbook prices. Sorry campus bookstore, you’re old news."
}];
export {
    homeSuggest, bottomIcons, strips, features, sites, testimonials
}