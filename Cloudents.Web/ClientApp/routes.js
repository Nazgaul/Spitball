import * as RouteTypes from "./routeTypes";

const resultContent = () => import("./components/results/Result.vue");
const dialogToolbar = () => import("./components/dialog-toolbar/DialogToolbar.vue");
const showItem = () => import("./components/preview/Item.vue");
const showFlashcard = () => import("./components/preview/Flashcard.vue");
const pageHeader = () => import("./components/header/header.vue");
//const pageHeaderBasic = () => import("./components/helpers/slimHeader.vue");
const bookDetailsHeader = () => import("./components/book/header.vue");
const slimHeader = () => import("./components/helpers/slimHeader/header.vue");
const bookDetails = () => import("./components/book/ResultBookDetails.vue");
const satelliteHeader = () => import("./components/satellite/header.vue");
const previewHeader = () => import("./components/helpers/header.vue");
// const newQuestion = () => import("./components/question/newQuestion/newQuestion.vue");
const viewQuestion = () => import("./components/question/question-details/questionDetails.vue");
const viewProfile = () => import("./components/profile/profile.vue");
const wallet = () => import("./components/wallet/wallet.vue");
const profilePageHeader = () => import("./components/profile/header/header.vue");
// const viewChat = () => import("./components/chat/view/chat.vue");
const userSettings = () => import("./components/settings/view/settings.vue");
//const userSettings = () => import("./components/settings/userSettings.vue");
import {staticRoutes} from "./components/satellite/satellite-routes";
import * as consts from "./store/constants";
const login = () => import("./components/new_registration/login.vue");
function dynamicPropsFn(route) {
    let newName = route.path.slice(1);

    return {
        name: newName,
        query: route.query,
        params: route.params,
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
        userText: route.query.term,
        submitRoute: route.path,
        currentSelection: route.path.slice(1)
    }
}


const resultPage = {
    default: resultContent,
    header: pageHeader,
};
const resultProps = {
    default: dynamicPropsFn,
    header: headerResultPageFn
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
        path: "/",
        name: "home",
        redirect: "/ask"
    },

    {
        path: "/result", name: "result", alias: [
            "/" + RouteTypes.marketRoute,
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
            header: bookDetailsHeader
        },
        props: bookDetailsProps
    },
    {
        //document/{universityName}/{courseName}/{id:long}/{name}
        path: "/note/:universityName/:courseName/:id/:name",
        alias: ['/document/:universityName/:courseName/:id/:name'],
        name: "item",
        components: {
            default: showItem,
            header: pageHeader
        },
        props: {
            default: (route) => ({id: route.params.id}),
            header: ()=>({currentSelection: "note"})
        }
    },
    {
        path: "/flashcard/:university/:courseId/:courseName/:id/:itemName",
        name: "flashcard",
        components: {default: showFlashcard, header: previewHeader},
        props: {default: (route) => ({id: route.params.id})}
    },
     {
        path: "/question/:id",
        components: {
            default: viewQuestion,
            header: pageHeader,
        },
        name: "question",
        props: {
            header: {submitRoute: '/ask'},
            default: (route) => ({id: route.params.id}),
        },
    },

    {
        path: "/profile/:id",
        components: {
            default: viewProfile,
            header: profilePageHeader,
        },
        name: "profile",
        props: {
            default: (route) => ({id: route.params.id})
        },
    },
    {
        path: "/wallet",
        components: {
            default: wallet,
            header: pageHeader
        },
        name: "wallet",
        meta: {
            requiresAuth: true
        },
        props: {
            // default: (route) => ({id: route.params.id}),
            header: ()=>({currentSelection: "ask"})
        },
    },

    {
        path: "/register", alias: ['/signin', '/resetpassword'],
        components: {
            default: login,
        },
        name: "registration",
        beforeEnter: (to, from, next) => {
            //prevent entering if loged in
            if(global.isAuth){
                next(false)
            }else{
                next()
            }
        }
    },

    {
        path: "/conversations",
        name: "conversations",
        components: {
            default: () => import("./components/conversations/conversations.vue"),
            header: pageHeader
        },
        meta: {
            requiresAuth: true
        },
        props: {

            header: ()=>({currentSelection: "ask"})
        },
    }

];

for (let v in staticRoutes) {
    let item = staticRoutes[v];
    routes2.push({
        path: item.path || "/" + item.name,
        name: item.name,
        components: {
            header: satelliteHeader,
            default: item.import
        },
        meta: {static: true},
        props: {default: (route) => item.params ? item.params(route) : {}}
    })
}

export const routes = routes2;

//
// function checkUserStatus(to, next) {
//     store.dispatch('userStatus').then(response => {
//         if (store.getters.loginStatus) {
//
//             to.path === "/register" ? next("/") : next();
//         }
//         else {
//             to.path === "/register" ? next() : next("signin");
//         }
//        // next()
//     }).catch(error => {
//         next("signin");
//     });
// }