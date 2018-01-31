﻿const HomePage = () => import("./components/home/home.vue");
const homePageHeader = () => import("./components/home/header.vue");
import * as RouteTypes from "./routeTypes";
const resultContent = () => import("./components/results/Result.vue");
const foodDetails = () => import("./components/food/foodDetails.vue");
const foodResultPage = () => import("./components/food/Result.vue");
//const foodHeader = () => import("./components/food/foodHeader.vue");
const dialogToolbar = () =>  import("./components/dialog-toolbar/DialogToolbar.vue");
const showItem = () => import("./components/preview/Item.vue");
const showFlashcard = () => import("./components/preview/Flashcard.vue");
const personalize = () => import("./components/settings/ResultPersonalize.vue");
const pageHeader = () => import("./components/header/header.vue");
const boodDetailsHeader = () => import("./components/book/header.vue");
const bookDetails = () => import("./components/book/ResultBookDetails.vue");
const satelliteHeader = () => import("./components/satellite/header.vue");
const previewHeader = () => import("./components/helpers/header.vue");
const documentPreviewHeader = () => import("./components/preview/headerDocument.vue");
import { staticRoutes } from "./components/satellite/satellite-routes";

function dynamicPropsFn(route) {
    let newName = route.path.slice(1);

    return {
        name: newName,
        query: route.query,
        params: route.params,
        isPromo:route.query.hasOwnProperty("promo"),
    }
}
function dynamicDetailsPropsFn(route) {
    return {
        name: route.name,
        query: route.query,
        id: route.params.id,
        params: route.params,
    }
}

function headerResultPageFn(route) {
    return {
        userText: route.query.q,
        submitRoute:route.path,
        currentSelection: route.path.slice(1)
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
    header: headerResultPageFn
};

const foodPage = {
    default: foodResultPage,
    personalize,
    header: pageHeader,
};


const foodDetailsProps = {
    default: true,
    header: () => ({
            toolbarTitle: "Food & Deals",
            height: "48",
        })
};
const bookDetailsProps = {
    default: dynamicDetailsPropsFn,
    header: (route) => (
        {
            name: "textbooks",
            id: route.params.id,
            currentSelection: "book",
            currentPath: "bookDetails"
        })
};
let routes2 = [
    {
        path: "/", components: {
            default: HomePage,
            header: homePageHeader
        }, name: "home"
    },
    {
        path: `/${ RouteTypes.foodRoute}`, name: "food", components: foodPage, props: resultProps
    },

    {
        path: "/result", name: "result", alias: [
            "/" + RouteTypes.questionRoute,
            "/" + RouteTypes.flashcardRoute,
            "/" + RouteTypes.notesRoute,
            "/" + RouteTypes.tutorRoute,
            "/" + RouteTypes.bookRoute,
            "/" + RouteTypes.jobRoute
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
            personalize,
            header: boodDetailsHeader
        },
        props: bookDetailsProps
    },
    {
        path: "/food/:id",
        name: RouteTypes.foodDetailsRoute,
        components: {
            default: foodDetails,
            header: dialogToolbar
        },
        props: foodDetailsProps
    },
    {
        path: "/item/:university/:courseId/:courseName/:id/:itemName", name: "item",
        components: { default: showItem, header: documentPreviewHeader,personalize},
        props: {default:(route)=>({id:route.params.id})}
    },
    {
        path: "/flashcard/:university/:courseId/:courseName/:id/:itemName",
        name: "flashcard",
        components: { default: showFlashcard, header: previewHeader,personalize },
        props: {default:(route)=>({id:route.params.id})}
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