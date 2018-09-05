import * as routes from "../../../routeTypes";
import { bannerData } from './campaign';
import { LanguageService } from "../../language/languageService";

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
            filter:[],
            id: routes.questionRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_ask"),
            icon: "sbf-ask-q", //TODO do we need this.....
            show: true
        }
    },
    note: {
        banner: getPromoQueryFromRoute,
        data:{
            id: routes.notesRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_note"),
            needLocation:false,
            filter: [ { id: "source", name: "sources" }],
            sort: [],
            icon: "sbf-note",
            show: false
        }
     },
    flashcard: {
        banner: getPromoQueryFromRoute,
        data:{
            id: routes.flashcardRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_flashcards"),
            needLocation: false,
            filter: [],
            sort: [],
            icon: "sbf-flashcards"
        }
    },
    tutor: {
        banner: getPromoQueryFromRoute,
        data:{
            id: routes.tutorRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_tutor"),
            needLocation: true,
            filter: [],
            sort: [],
            icon: "sbf-tutor"
        }
    },
    book: {
        banner: getPromoQueryFromRoute,
        data:{
            id: routes.bookRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_book"),
            icon: "sbf-textbooks"
        },


    },
    job: {
        banner: getPromoQueryFromRoute,
        data:{
            id: routes.jobRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_job"),
            needLocation: true,
            filter: [],
            sort: [],
            icon: "sbf-job"
        }
    }
};

export let details = {
    bookDetails: {
        filter: [
            LanguageService.getValueByKey("navigation_nav_bookDetails_filter_new"),
            LanguageService.getValueByKey("navigation_nav_bookDetails_filter_rental"),
            LanguageService.getValueByKey("navigation_nav_bookDetails_filter_used"),
          ],
        sort: [
            "price",
        ]
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