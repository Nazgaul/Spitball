import * as routes from "../../../routeTypes";
import { bannerData } from './campaign';


function getPromoQueryFromRoute(path, query){
    if(!!query && query.hasOwnProperty("promo")){
        if(!!bannerData[query.promo]){
            return bannerData[query.promo][path].banner;
        }else{
            return bannerData["navDefault"][path].banner;
        }
    }else{
        return bannerData["navDefault"][path].banner;
    }
}


const nav = {
    ask: {
        banner: getPromoQueryFromRoute,
        data:{
            filter:[], //TODO filters refactor check if we still need this
            id: routes.questionRoute,
            name: "Homework Help",
            icon: "sbf-ask-q", //TODO do we need this.....
            show: true
        }
    },
    note: {
        banner: getPromoQueryFromRoute,
        data:{
            id: routes.notesRoute,
            name: "Study Documents",
            needLocation:false,
            //TODO Beny refactor filter
            // filter: [{ id: "course", name: "My Courses" }, { id: "source", name: "sources" }],
            filter: [ { id: "source", name: "sources" }],
            sort: [
                { id: "relevance", name: "relevance" },
                { id: "date", name: "date" }
            ],
            icon: "sbf-note",
            show: false
        }
     },
    flashcard: {
        banner: getPromoQueryFromRoute,
        data:{
            id: routes.flashcardRoute,
            name: "Flashcards",
            needLocation: false,
            filter: [
                //TODO Beny refactor filter
                // { id: "course", name: "My Courses" },
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
        banner: getPromoQueryFromRoute,
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
        banner: getPromoQueryFromRoute,
        data:{
            id: routes.bookRoute,
            name: "Textbooks",
            icon: "sbf-textbooks"
        },


    },
    job: {
        banner: getPromoQueryFromRoute,
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