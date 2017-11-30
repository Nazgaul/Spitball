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
        }];
    export{
        homeSuggest,bottomIcons,strips,features,sites,testimonials
    }