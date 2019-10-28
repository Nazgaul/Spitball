import * as routes from "../../../routeTypes";
import { LanguageService } from "../../language/languageService";

const nav = {
    feed: {
        data:{
            filter:[],
            id: routes.feedRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_feed"),
            icon: "sbf-ask-q", //BLITZ TODO do we need this.....
        }
    },
    tutor: {
        data:{
            id: routes.tutorRoute,
            name: LanguageService.getValueByKey("navigation_nav_name_tutor"),
            needLocation: true,
            filter: [],
            sort: [],
            icon: "sbf-tutor",//BLITZ TODO do we need this.....
        }
    },

};

export let names = [];
export let page = [];
export let verticalsNavbar = [];
export let verticalsName = [];

for (let i in nav) {
    let item = nav[i].data;

    verticalsName.push(i);
    names.push({'id': item.id, 'name': item.name});

    let isRtl = global.isRtl;// document.getElementsByTagName("html")[0].getAttribute("dir") === "rtl";
    let isMobile = global.innerWidth < 600;
    let navObj = {
        'id': item.id,
        'name': item.name,
        'icon': item.icon,
    }

    if(isRtl && isMobile) {
        verticalsNavbar.unshift(navObj);
    }else {
        verticalsNavbar.push(navObj);
    }

    page[i] = {
        filter: item.filter,
        sort: item.sort
    };
}


export default nav