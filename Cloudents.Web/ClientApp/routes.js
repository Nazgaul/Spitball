const HomePage = () => import("./components/home/home.vue");
const homePageHeader = () => import("./components/home/header.vue");
import * as RouteTypes from "./routeTypes";

const resultContent = () => import("./components/results/Result.vue");
const foodDetails = () => import("./components/food/foodDetails.vue");
const foodResultPage = () => import("./components/food/Result.vue");
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
const landingTemplate = () => import("./components/landing-pages/pageTemplate.vue");
const registration = () => import("./components/registration/registration.vue");
const signin = () => import("./components/registration/signin.vue");
const askQuestion = () => import("./components/question/ask/askQuestion.vue");
const viewQuestion = () => import("./components/question/question-details/questionDetails.vue");
const viewProfile = () => import("./components/profile/profile.vue");
const profilePageHeader = () => import("./components/profile/header/header.vue");
// const viewChat = () => import("./components/chat/view/chat.vue");
const userSettings = () => import("./components/settings/view/settings.vue");
//const userSettings = () => import("./components/settings/userSettings.vue");
import {staticRoutes} from "./components/satellite/satellite-routes";
// import store from "./store";

function dynamicPropsFn(route) {
    let newName = route.path.slice(1);

    return {
        name: newName,
        query: route.query,
        params: route.params,
        isPromo: route.query.hasOwnProperty("promo"),
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

const foodPage = {
    default: foodResultPage,
    header: pageHeader,
};


const foodDetailsProps = {
    default: true,
    header: () => ({
        toolbarTitle: "Food & Deals",
        height: "48", // TODO: why this is string.
        app: true
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
        path: "/landing/get_a_tutor_now", components: {
            default: landingTemplate,
        }, name: "tutorV1"
    },
    {
        path: "/landing/tutor", components: {
            default: landingTemplate,
        }, name: "tutorV2"
    },
    {
        path: "/landing/get_the_notes_you_need", components: {
            default: landingTemplate,
        }, name: "notesV1"
    },
    {
        path: "/landing/notes", components: {
            default: landingTemplate,
        }, name: "notesV2"
    },
    {
        path: "/landing/You_Dont_Have_to_Be_Broke",
        components: {
            default: landingTemplate,
        }, name: "jobsV1"
    },
    {
        path: "/landing/jobs", components: {
            default: landingTemplate,
        }, name: "jobsV2"
    },
    {
        path: "/register", components: {
            default: registration,
        }, name: "registration",
    },
    {
        path: "/askquestion", components: {
            default: askQuestion,
            header: slimHeader,
        }, name: "askQuestion",
        meta: {
            requiresAuth: true
        },
    },
    {
        path: "/question/:id",
        components: {
            default: viewQuestion,
            header: slimHeader,
        },
        name: "question",
        props: {
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
    // {
    //     path: "/settings", components: {
    //         default: userSettings,
    //         slimHeader: pageHeader,
    //     }, name: "chat"
    // },
    //
    // {//TODO: remove chat component designed by Artem
    //     path: "/chat", components: {
    //         default: viewChat,
    //         slimHeader: pageHeader,
    //     }, name: "chat"
    // },
    {
        path: "/verify-phone",
        components: {
            default: registration,
        },
        name: "registration",
        meta: {step: 1}
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
        meta:{static:true},
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