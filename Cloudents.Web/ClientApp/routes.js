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
const previewHeader = () => import("./components/helpers/header.vue");
const documentPreviewHeader = () => import("./components/preview/headerDocument.vue");
import { staticRoutes } from "./components/satellite/satellite-routes";

//const mobileDetailsFirstLine = () => import("./components/header/headerFirstLineMobile.vue");
import { details } from './data'


function dynamicPropsFn(route) {
    let newName = route.path.slice(1);
    let filterOptions = [];
    let filtersList=['jobType','source','course'];

        if (route.query.filter) {
            filterOptions = filterOptions.concat(route.query.filter);
            if (route.query.jobType) {
                filterOptions = filterOptions.concat(route.query.jobType);
            }
        } else {
            Object.entries(route.query).forEach(([key, val])=>{
                if(val&&filtersList.includes(key)) {
                    filterOptions = filterOptions.concat(val);
                }
            });
        }

    return {
        name: newName,
        query: route.query,
        params: route.params,
        hasExtra: newName.includes('food'),
        isPromo:route.query.hasOwnProperty("promo"),
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




const foodDetailsProps = {
    default: true,
};
const bookDetailsProps = {
    ...resultProps,
    default: dynamicDetailsPropsFn,
    header: (route) => ({id: route.params.id })
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
        props: bookDetailsProps
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