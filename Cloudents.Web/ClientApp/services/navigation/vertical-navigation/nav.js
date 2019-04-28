import * as routes from "../../../routeTypes";
import { LanguageService } from "../../language/languageService";

let nonIsraeliUser = global.country.toUpperCase() !== 'IL';

const nav = {
    note: {
        data:{
            id: routes.notesRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_note"),
            needLocation:false,
            filter: [ { id: "source", name: "sources" }],
            sort: [],
            icon: "sbf-note",
            visible: true,
            soon: false
        }
     },
     ask: {
        data:{
            filter:[],
            id: routes.questionRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_ask"),
            icon: "sbf-ask-q", //TODO do we need this.....
            visible: true,
            soon: false
        }
    },
    // flashcard: {
    //     data:{
    //         id: routes.flashcardRoute,
    //         name: LanguageService.getValueByKey("navigation_nav_name_flashcards"),
    //         needLocation: false,
    //         filter: [],
    //         sort: [],
    //         icon: "sbf-flashcards",
    //         visible: true,
    //         soon: !nonIsraeliUser
    //     }
    // },
    tutor: {
        data:{
            id: routes.tutorRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_tutor"),
            needLocation: true,
            filter: [],
            sort: [],
            icon: "sbf-tutor",
            visible: true,
            soon: !nonIsraeliUser
        }
    },
    // book: {
    //     data:{
    //         id: routes.bookRoute,
    //         name: LanguageService.getValueByKey("navigation_nav_name_book"),
    //         icon: "sbf-textbooks",
    //         visible: true,
    //         soon: !nonIsraeliUser
    //     },
    // },
    // job: {
    //     data:{
    //         id: routes.jobRoute,
    //         name: LanguageService.getValueByKey("navigation_nav_name_job"),
    //         needLocation: true,
    //         filter: [],
    //         sort: [],
    //         icon: "sbf-job",
    //         visible: nonIsraeliUser,
    //         soon: !nonIsraeliUser
    //     }
    // }
};

let strNew = LanguageService.getValueByKey("navigation_nav_bookDetails_filter_new");
let strRental = LanguageService.getValueByKey("navigation_nav_bookDetails_filter_rental");
let strUsed = LanguageService.getValueByKey("navigation_nav_bookDetails_filter_used");
let strBuy = LanguageService.getValueByKey("book_sort_buy");
let strSell = LanguageService.getValueByKey("book_sort_sell");
export let details = {
    bookDetails: {
        filter: [
            {key: strNew, value: strNew},
            {key: strRental, value: strRental},
            {key: strUsed, value:strUsed}
    ],
        sort: [
            {key: strBuy, value: strBuy},
            {key: strSell,value: strSell}
        ]
    }
};
export let names = [];
export let page = [];
export let verticalsNavbar = [];
export let verticalsName = [];


for (let i in nav) {
    let item = nav[i].data;
    if(!item.visible) continue;
    verticalsName.push(i);
    names.push({'id': item.id, 'name': item.name});
    if(!!item.visible){
        let isRtl = document.getElementsByTagName("html")[0].getAttribute("dir") === "rtl";
        let isMobile = global.innerWidth < 600;
        let navObj = {
            'id': item.id,
            'name': item.name,
            'icon': item.icon,
            'visible': item.visible,
            'soon': item.soon
            //image: item.image
        }
        if(isRtl && isMobile){
            verticalsNavbar.unshift(navObj);
        }else{
            verticalsNavbar.push(navObj);
        }
    }
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