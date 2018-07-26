
//const HomePage = () => import("./components/home/home.vue");
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
const documentPreviewHeader = () => import("./components/preview/headerDocument.vue");
//const landingTemplate = () => import("./components/landing-pages/pageTemplate.vue");
const registration = () => import("./components/registration/registration.vue");
const signin = () => import("./components/registration/signin.vue");
const registerAccount = () => import("./components/registration/steps/registerAccount/registerAccount.vue");
const newQuestion = () => import("./components/question/newQuestion/newQuestion.vue");
const viewQuestion = () => import("./components/question/question-details/questionDetails.vue");
const viewProfile = () => import("./components/profile/profile.vue");
const wallet = () => import("./components/wallet/wallet.vue");
const profilePageHeader = () => import("./components/profile/header/header.vue");
// const viewChat = () => import("./components/chat/view/chat.vue");
const userSettings = () => import("./components/settings/view/settings.vue");
//const userSettings = () => import("./components/settings/userSettings.vue");
import {staticRoutes} from "./components/satellite/satellite-routes";
import * as consts from "./store/constants";
const verifyPhone = () => import("./components/registration/verifyPhone/verify.vue");

// import store from "./store";

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
        userText: route.query.q,
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
        path: "/item/:university/:courseId/:courseName/:id/:itemName", name: "item",
        components: {default: showItem, header: documentPreviewHeader},
        props: {default: (route) => ({id: route.params.id})}
    },
    {
        path: "/flashcard/:university/:courseId/:courseName/:id/:itemName",
        name: "flashcard",
        components: {default: showFlashcard, header: previewHeader},
        props: {default: (route) => ({id: route.params.id})}
    },
    {
        path: "/newquestion", components: {
            default: newQuestion,
            header: pageHeader,
        }, name: "newQuestion",
        meta: {
            requiresAuth: true
        }
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
            header: previewHeader
        },
        name: "wallet",
        meta: {
            requiresAuth: true
        }
    },
    {
        path: "/register", components: {
            default: registration,
        }, name: "registration",
    },

    {
        path: "/verify-phone",
        components: {
            default: verifyPhone,
        },
        // props: { newsletterPopup: false },

        name: "phoneVerify",

    },
    {
        path: "/congrats",
        components: {
            default: registerAccount,
        },
        name: "congrats",

    },

    {
        path: "/signin", components: {
            default: signin
        }, name: "signin"
    },

    {
        path: "/conversations",
        name: "conversations",
        components: {
            default: () => import("./components/conversations/conversations.vue"),
            header: slimHeader
        },
        meta: {
            requiresAuth: true
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