import * as routes from "../../../routeTypes";

const nav = {
    ask: {
        banner:{
            "lineColor": "#00c0fa",
            "title" : "Make money while helping others with their homework.",
            "textMain" : "Answer HW questions and cash out to Amazon Coupons. There’s no catch!",
            "boldText" : "cash out to Amazon Coupons"
        },
        data:{
            filter:[{ id: "source", name: "subject" }],
            id: routes.questionRoute,
            name: "Homework Help",
            icon: "sbf-ask-q" //TODO do we need this.....
        }
    },
    note: {
        banner:{
            "lineColor": "#943bfd",
            "title" : "Notes, study guides, exams and more from the best sites.",
            "textMain" : "Filtered by your school, classes and preferences. Saving you time!",
            "boldText" : "your school, classes and preferences"
        },
        data:{
            id: routes.notesRoute,
            name: "Study Documents",
            needLocation:false,
            filter: [{ id: "course", name: "My Courses" }, { id: "source", name: "sources" }],
            sort: [
                { id: "relevance", name: "relevance" },
                { id: "date", name: "date" }
            ],
            icon: "sbf-note"
        }

     },
    flashcard: {
        banner:{
            "lineColor": "#f14d4d",
            "title" : "Study from millions of flashcard sets to improve your grades.",
            "textMain" : "Filtered by your school, classes and preferences. Saving you time!" ,
            "boldText" : "your school, classes and preferences"
        },
        data:{
            id: routes.flashcardRoute,
            name: "Flashcards",
            needLocation: false,
            filter: [
                { id: "course", name: "My Courses" },
                { id: "source", name: "sources" }
            ],
            sort: [
                { id: "relevance", name: "relevance" },
                { id: "date", name: "date" }
            ],
            icon: "sbf-flashcards"
        }
    },
    tutor: {
        banner: {
            "lineColor": "#52aa16",
            "title": "Find an expert to help you ace your classes in-person or online.",
            "textMain": "No matter the subject, a tutor is here to help you succeed.",
            "boldText" : "here to help you succeed."
        },
        data:{
            id: routes.tutorRoute,
            name: "Tutors",
            needLocation: true,
            filter: [
                { id: "online", name: "Online Lessons" },
                { id: "inPerson", name: "In Person" }
            ],
            sort: [
                { id: "relevance", name: "relevance" },
                { id: "price", name: "price" }
                // { id: "distance", name: "distance" }
            ],
            icon: "sbf-tutor"
        }
    },
    book: {
        banner:{
            "lineColor": "#a650e0",
            "title" : "Compare the best prices to buy, rent or sell your textbooks.",
            "textMain" : "Preview quotes from hundreds of sites simultaneously.",
            "boldText" : "hundreds of sites simultaneously."
        },
        data:{
            id: routes.bookRoute,
            name: "Textbooks",
            icon: "sbf-textbooks"
        },


    },
    job: {
        banner:{
            "lineColor": "#f49c20",
            "title" : "Find jobs and internships catered specifically to students. ",
            "textMain" : "Filtered by your experience and location preference.",
            "boldText" : "experience and location preference."
        },
        data:{
            id: routes.jobRoute,
            name: "Jobs",
            needLocation: true,
            filter: [{ id: "jobType", name: "job type" }],
            sort: [
                { id: "relevance", name: "relevance" },
                // { id: "distance", name: "distance" },
                { id: "date", name: "date" }
            ],
            icon: "sbf-job"
        }
    }
};
export let details = {
    bookDetails: {
        filter: [{id: "new", name: "new"}, {id: "rental", name: "rental"}, {id: "eBook", name: "eBook"}, {
            id: "used",
            name: "used"
        }],
        sort: [{id: "price", name: "price"}]
    }
};
export let verticalsList = [];
export let names = [];
export let page = [];
export let verticalsNavbar = [];
export let verticalsName = [];
for (let i in nav) {
    let item = nav[i].data;
    verticalsName.push(i);
    names.push({'id': item.id, 'name': item.name});
    verticalsNavbar.push(
        {
            'id': item.id,
            'name': item.name,
            'icon': item.icon
            //image: item.image
        });
    verticalsList.push(nav[i]);
    page[i] = {
        // title: item.resultTitle,
        //emptyText: item.emptyState,
        filter: item.filter,
        sort: item.sort
    }
}
for (let i in details) {
    let item = details[i];
    page[i] = {filter: item.filter, sort: item.sort}
}



export default nav