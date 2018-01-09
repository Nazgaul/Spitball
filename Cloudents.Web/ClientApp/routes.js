const HomePage = () => import("./components/home/home.vue");
const homePageHeader = () => import("./components/home/header.vue");
import * as RouteTypes from "./routeTypes";
const resultContent = () => import("./components/results/Result.vue");
const foodDetails = () => import("./components/food/foodDetails.vue");
const foodHeader = () => import("./components/food/foodHeader.vue");
const showItem = () => import("./components/preview/Item.vue");
const showFlashcard = () => import("./components/preview/Flashcard.vue");
const personalize = () => import("./components/settings/ResultPersonalize.vue");
const pageHeader = () => import("./components/header/header.vue");
const boodDetailsHeader = () => import("./components/book/header.vue");
const bookDetails = () => import("./components/book/ResultBookDetails.vue");

const satelliteHeader = () => import("./components/satellite/header.vue");
const previewHeader = () => import("./components/preview/header.vue");
import { staticRoutes } from "./components/satellite/satellite-routes";
//const faqView = () => import("./components/satellite/faq.vue");


//const mobileDetailsFirstLine = () => import("./components/header/headerFirstLineMobile.vue");
import { page, verticalsName, verticalsNavbar, details } from './data'
const $_calcTerm = (name) => {
    return (name.includes('food') || name.includes('purchase')) ? 'foodTerm' : name.includes('job') ? 'jobTerm' : 'term'
};


function dynamicPropsFn(route) {
    let newName = route.path.slice(1);
    let filterOptions = [];
    if (route.query.filter) {
        filterOptions = filterOptions.concat(route.query.filter);
        if (route.query.jobType) {
            filterOptions = filterOptions.concat(route.query.jobType);
        }
    } else {
        if (route.query.source) {
            filterOptions = filterOptions.concat(route.query.source);
        }
        if (route.query.course ||
            (newName.includes("flashcard") || newName.includes("note"))
        ) {
            let list = [].concat(route.query.course ? route.query.course : []);
            list = list.concat(route.meta.myClasses ? route.meta.myClasses : []);
            filterOptions = filterOptions.concat([...new Set(list.map(i => Number(i)))]);
        }
        if (route.query.jobType) {
            filterOptions = filterOptions.concat(route.query.jobType);
        }
    }
    return {
        name: route.path.slice(1),
        query: route.query,
        filterSelection: filterOptions,
        sort: route.query.sort,
        userText: route.query.q,
        currentTerm: newName.includes("food")
            ? route.meta.foodTerm
            : newName.includes("job")
                ? route.meta.jobTerm
                : route.meta.term,
        params: route.params,
        hasExtra: newName.includes('food'),
        page: page[newName],
        $_calcTerm: $_calcTerm,
        getFacet: route.meta[`${newName}Facet`],
        currentSuggest: verticalsName.filter(i => i !== newName)[(Math.floor(Math.random() * (verticalsName.length - 2)))],
        vertical: route.meta.vertical
    }
}
function dynamicDetailsPropsFn(route) {
    return {
        name: route.name,
        query: route.query,
        filterOptions: [{ title: "Book Type", modelId: "filter", data: details.bookDetails.filter }],
        sort: "price",
        id: route.params.id,
        params: route.params,
        page: page[route.name],
        $_calcTerm: $_calcTerm
    }
}
function moreInfoFn(route) {
    return {
        actions: route.path.includes("SubjectOrCourse") ? [{ name: "edit Subject" }, { name: "Select Course" }] : [{ name: "edit Search" }]
    };
}
function verticalsLinkFun(route) {
    let currentPath = route.path;
    return {
        $_calcTerm: $_calcTerm,
        verticals: verticalsNavbar,
        userText: route.query.q,
        currentPath,
        getLuisBox: (name) => route.meta[$_calcTerm(name)],
        name: route.name,
        myClasses: route.meta.myClasses,
        luisType: currentPath.includes('food') ? 'foodTerm' : currentPath.includes('job') ? 'jobTerm' : 'term',
        currentSelection: route.path.slice(1)
    }
}
function headerResultPageFn(route) {
    return {
        userText: route.query.q
    }
}


const resultPage = {
    default: resultContent,
    personalize,
    header: pageHeader,
};
const resultProps = {
    default: dynamicPropsFn,
    personalize,
    header: verticalsLinkFun
};




const foodDetailsProps = {
    default: true,
};
const bookDetailsProps = {
    ...resultProps,
    default: dynamicDetailsPropsFn,
    //verticalListMobile: filterLinkFun,
    header: (route) => ({ ...verticalsLinkFun(route), name: "textbooks", id: route.params.id, currentSelection: "book", currentPath: "bookDetails" })
};
let routes2 = [
    {
        path: "/", components: {
            default: HomePage,
            header: homePageHeader
        }, name: "home", meta: {

        }
    },

    {
        path: "/result", name: "result", alias: [
            "/" + RouteTypes.questionRoute,
            "/" + RouteTypes.flashcardRoute,
            "/" + RouteTypes.notesRoute,
            "/" + RouteTypes.tutorRoute,
            "/" + RouteTypes.bookRoute,
            "/" + RouteTypes.jobRoute,
            "/" + RouteTypes.foodRoute
        ], components: resultPage, props: resultProps, meta: {
            isAcademic: true,
            analytics: {
                pageviewTemplate(route) {
                    return {
                        title: route.path.slice(1).charAt(0).toUpperCase() + route.path.slice(2),
                        path: route.path,
                        location: window.location.href
                    }
                }
            }
        }
    },
    {
        path: "/book/:id",
        name: RouteTypes.bookDetailsRoute,
        components: {
            default: bookDetails,
            header: boodDetailsHeader
        },
        props: bookDetailsProps,
        meta: {
            pageName: "book"
        }
    },
    {
        path: "/food/:id",
        name: RouteTypes.foodDetailsRoute,
        components: {
            default: foodDetails,
            header: foodHeader
        },
        props: foodDetailsProps
    },
    {
        path: "/item/:university/:courseId/:courseName/:id/:itemName", name: "item", component: showItem, props: true, meta: {
            pageName: RouteTypes.notesRoute
        }
    },
    {
        path: "/flashcard/:university/:courseId/:courseName/:id/:itemName", name: "flashcard", components: {default:showFlashcard,header:previewHeader}, props: true,
        meta: {
            pageName: RouteTypes.flashcardRoute
        }
    },

];
for (let v in staticRoutes) {
    let item = staticRoutes[v];
    routes2.push({
        path: item.path || "/" + item.name,
        name: item.name,
        components: {
            header: satelliteHeader,
            default: item.import
        }
    })
}

export const routes = routes2;