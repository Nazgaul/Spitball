const navDefault = {
    ask: {
        banner:{
            "lineColor": "#00c0fa",
            "title" : "Make money while helping others with their homework.",
            "textMain" : "Answer HW questions and cash out to Amazon Coupons. There’s no catch!",
            "boldText" : "cash out to Amazon Coupons",
            url: '',
            showOverlay: true
        },

    },
    note: {
        banner:{
            "lineColor": "#943bfd",
            "title" : "Notes, study guides, exams and more from the best sites.",
            "textMain" : "Filtered by your school, classes and preferences. Saving you time!",
            "boldText" : "your school, classes and preferences",
            url: '',
            showOverlay: true
        },

     },
    flashcard: {
        banner:{
            "lineColor": "#f14d4d",
            "title" : "Study from millions of flashcard sets to improve your grades.",
            "textMain" : "Filtered by your school, classes and preferences. Saving you time!" ,
            "boldText" : "your school, classes and preferences",
            url: '',
            showOverlay: true
        },

    },
    tutor: {
        banner: {
            "lineColor": "#52aa16",
            "title": "Find an expert to help you ace your classes in-person or online.",
            "textMain": "No matter the subject, a tutor is here to help you succeed.",
            "boldText" : "here to help you succeed.",
            url: '',
            showOverlay: true
        },

    },
    book: {
        banner:{
            "lineColor": "#a650e0",
            "title" : "Compare the best prices to buy, rent or sell your textbooks.",
            "textMain" : "Preview quotes from hundreds of sites simultaneously.",
            "boldText" : "hundreds of sites simultaneously.",
            url: '',
            showOverlay: true
        },

    },
    job: {
        banner:{
            "lineColor": "#f49c20",
            "title" : "Find jobs and internships catered specifically to students. ",
            "textMain" : "Filtered by your experience and location preference.",
            "boldText" : "experience and location preference.",
            url: '',
            showOverlay: true
        },
    },
};

const campaignX = {
    ask: {
        banner:{
            "lineColor": "#00c0fa",
            "title" : "aaaaMake money while helping others with their homework.",
            "textMain" : "aaaAnswer HW questions and cash out to Amazon Coupons. There’s no catch!",
            "boldText" : "aaacash out to Amazon Coupons",
            url: '',
            showOverlay: true,
            campaignClass: 'ask-campaignX',
        },
    },
    note: {
        banner:{
            "lineColor": "#943bfd",
            "title" : "Notes, study guides, exams and more from the best sites.",
            "textMain" : "Filtered by your school, classes and preferences. Saving you time!",
            "boldText" : "your school, classes and preferences",
            url: '',
            showOverlay: false,
            campaignClass: 'note-campaignX',

        }
     },
    flashcard: {
        banner:{
            "lineColor": "#f14d4d",
            "title" : "Study from millions of flashcard sets to improve your grades.",
            "textMain" : "Filtered by your school, classes and preferences. Saving you time!" ,
            "boldText" : "your school, classes and preferences",
            url: '',
            showOverlay: false,
            campaignClass: 'flashcard-campaignX',
        }
    },
    tutor: {
        banner: {
            "lineColor": "#52aa16",
            "title": "Find an expert to help you ace your classes in-person or online.",
            "textMain": "No matter the subject, a tutor is here to help you succeed.",
            "boldText" : "here to help you succeed.",
            url: '',
            showOverlay: false,
            campaignClass: 'tutor-campaignX',
        }
    },
    book: {
        banner:{
            "lineColor": "#a650e0",
            "title" : "Compare the best prices to buy, rent or sell your textbooks.",
            "textMain" : "Preview quotes from hundreds of sites simultaneously.",
            "boldText" : "hundreds of sites simultaneously.",
            url: '',
            showOverlay: false,
            campaignClass: 'book-campaignX',
        }
    },
    job: {
        banner:{
            "lineColor": "#f49c20",
            "title" : "Find jobs and internships catered specifically to students. ",
            "textMain" : "Filtered by your experience and location preference.",
            "boldText" : "experience and location preference.",
            url: '',
            showOverlay: false,
            campaignClass: 'job-campaignX',
        }
    },

};


export const bannerData = {
    navDefault,
    campaignX
};