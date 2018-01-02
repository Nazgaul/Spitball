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
            title: "Study Documents",
            text: "Spitball curates study documents from the best sites on the web. Our notes, study guides and exams populate based on student ratings and are filtered by your school, classes and preferences."
        },

        flashcards: {
            class: "flashcards",
            image: "strip-flashcards.png",
            floatingImages: ['flashcardPenIcon', 'flashcardQuestionIcon', 'flashcardGroupIcon'],
            titleIcon: 'stripFlashcardsIcon',
            title: "Flashcards",
            text: "Search millions of study sets and improve your grades by studying with flashcards."
        },

        tutors: {
            class: "tutors",
            image: "strip-tutors.png",
            floatingImages: ['tutorIcon', 'discussionIcon','studentLaptopIcon'],
            titleIcon: 'stripTutorIcon',
            title: "Tutors",
            text: "Spitball has teamed up with the most trusted tutoring services to help you ace your classes."// All of our online and in-person tutors are highly qualified experts with educations from some of the best universities in the world."
        },

        textbooks: {
            class: "textbooks",
            image: "strip-textbook-icon.png",
            floatingImages: ['bookClosedIcon', 'bookStackIcon', 'bookOpenIcon'],
            titleIcon: "stripTextbooksIcon",
            title: "Textbooks",
            text: "Find the best prices to buy, rent and sell your textbooks by comparing hundreds of sites simultaneously."
        },

        askQuestion: {
            class: "ask-question",
            image: "strip-ask-question.png",
            floatingImages: ['askQuestionMountainIcon', 'askQuestionHouseIcon', 'askQuestionRocketIcon'],
            titleIcon: 'stripAskQuestionIcon',
            title: "Ask A Question",
            text: "Ask any school related question and immediately get answers and information that relates specifically to you, your classes, and your university."
        },

        jobs: {
            class: "jobs",
            image: "strip-jobs.png",
            floatingImages: ['jobsGraphIcon', 'jobsChemistryIcon', 'jobsSlideIcon'],
            titleIcon: 'stripJobsIcon',
            title: "Jobs",
            text: "Easily search and apply to paid internships, part-time jobs and entry-level opportunities from local businesses all the way to Fortune 500 companies."
        },
        
        foodDeals: {
            class: "food-deals",
            image: "strip-food-deals.png",
            floatingImages: ['shoppingBagsIcon', 'headsetIcon', 'pizzaIcon'],
            titleIcon: "stripFoodDealsIcon",
            title: "Food and Deals",
            text: "Discover exclusive deals to local businesses, restaurants and bars near campus."
        }


    };
let features =
    {

        document: {
            title: "Millions of Documents",
            text: "Find study guides, homework, practice exams and notes for courses at your school.",
            icon: "notebookIcon"
        },

        textbook: {
            title: "Save up to 50% on Textbooks",
            text: "A new course load doesn’t have to break the bank! Find the best prices to rent, buy or sell your textbooks.",
            icon: "bookStackIcon"
        },

        homework: {
            title: "Homework Help 24/7",
            text: "Find the answers to all of your questions with help from expert tutors in person or online.",
            icon: "studentLaptopIcon"
        },
    }
    let sites=[
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
    let testimonials=[{
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
            testimonial: "It’s so nice to have all of my notes, questions, tutors and more in one place and always available. Spitball comes in handy when I miss class and need to find notes or homework help. Thank you Spitball for making my life a whole lot easier!"
        },
        {
            name: "Sarah Friedman",
            uni: "Freshman at Syracuse",
            testimonial: "Spitball is such a great idea! No more searching 100 different sites for class notes and study material. Plus, you can’t beat the textbook prices. Sorry campus bookstore, you’re old news."
        }];
    export{
        homeSuggest,bottomIcons,strips,features,sites,testimonials
    }