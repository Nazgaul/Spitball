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


export default nav