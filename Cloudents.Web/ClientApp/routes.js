import * as RouteTypes from "./routeTypes";

import resultContent from './components/results/Result.vue';
import HomeworkHelpComponent from './components/results/HomeworkHelp/HomeworkHelp.vue';
import StudyDocumentsComponent from './components/results/StudyDocuments/StudyDocuments.vue';
import pageHeader from './components/header/header.vue';
// import landingHeader from './components/landingPage/helpers/landingHeader.vue'
import landingPage from './components/landingPage/landingPage.vue';
import schoolBlock from './components/schoolBlock/schoolBlock.vue';
import verticalsTabs from './components/header/verticalsTabs.vue';
import { staticRoutes } from "./components/satellite/satellite-routes";

const showItem = () => import("./components/preview/Item.vue");
const showFlashcard = () => import("./components/preview/Flashcard.vue");
const bookDetailsHeader = () => import("./components/book/header.vue");
const bookDetails = () => import("./components/book/ResultBookDetails.vue");
const satelliteHeader = () => import("./components/satellite/header.vue");
const previewHeader = () => import("./components/helpers/header.vue");
const viewQuestion = () => import("./components/question/question-details/questionDetails.vue");
const wallet = () => import("./components/wallet/wallet.vue");
// const viewProfile = () => import("./components/profile/profile.vue");
const newProfile = () => import("./components/new_profile/new_profile.vue");
const profilePageHeader = () => import("./components/profile/header/header.vue");
const login = () => import("./components/new_registration/login.vue");
// const university = () => import("./components/helpers/uni-select/uniSelect.vue");

// course section
const setCourse = () => import("./components/courses/courses.vue");
const addCourse = () => import("./components/courses/addCourses/addCourses.vue");
const editCourse = () => import("./components/courses/editCourses/editCourses.vue");
/*
 new uni section
*/
const setUniversity = () => import("./components/university/university.vue");
const addUniversity = () => import("./components/university/addUniversity/addUniversity.vue");

const tutorComponent = () => import("./components/tutor/tutor.vue");

function dynamicPropsFn(route) {
    let newName = route.path.slice(1);

    return {
        name: newName,
        query: route.query,
        params: route.params,
    };
}

function dynamicDetailsPropsFn(route) {
    return {
        name: route.name,
        query: route.query,
        id: route.params.id,
        params: route.params,
    };
}

function headerResultPageFn(route) {
    return {
        userText: route.query.term,
        submitRoute: route.path,
        currentSelection: route.path.slice(1)
    };
}

function verticalResultPageFn(route) {
    return {
        currentSelection: route.path.slice(1)
    };
}

const resultPage = {
    default: resultContent,
    header: pageHeader,
    verticals: verticalsTabs,
    schoolBlock: schoolBlock,
};
const resultProps = {
    default: dynamicPropsFn,
    header: headerResultPageFn,
    verticals: verticalResultPageFn
};
const homeworkHelpPage = {
    default: HomeworkHelpComponent,
    header: pageHeader,
    schoolBlock: schoolBlock,
    verticals: verticalsTabs
};
const studyDocumentsPage = {
    default: StudyDocumentsComponent,
    header: pageHeader,
    schoolBlock: schoolBlock,
    verticals: verticalsTabs
};

const bookDetailsProps = {
    default: dynamicDetailsPropsFn,
    header: (route) => ({
        name: "textbooks",
        id: route.params.id,
        currentSelection: "book",
        currentPath: "bookDetails"
    })
};
let routes2 = [
    {
        path: "/",
        name: "main",
        components: {
            default: landingPage,

        },

    },
    {
        path: "/result",
        name: "result",
        alias: [
            "/" + RouteTypes.marketRoute,
            // "/" + RouteTypes.questionRoute,
            "/" + RouteTypes.flashcardRoute,
            // "/" + RouteTypes.notesRoute,
            "/" + RouteTypes.tutorRoute,
            "/" + RouteTypes.bookRoute,
            "/" + RouteTypes.jobRoute
        ],
        components: resultPage,
        props: resultProps,
        meta: {
            isAcademic: true,
            showMobileFooter: true,
            analytics: {
                pageviewTemplate(route) {
                    return {
                        title: route.path.slice(1).charAt(0).toUpperCase() + route.path.slice(2),
                        path: route.path,
                        location: window.location.href
                    };
                }
            }
        }
    },
    {
        path: "/" + RouteTypes.questionRoute,
        name: "ask",
        components: homeworkHelpPage,
        props: resultProps,
        meta: {
            isAcademic: true,
            showMobileFooter: true,
            analytics: {
                pageviewTemplate(route) {
                    return {
                        title: route.path.slice(1).charAt(0).toUpperCase() + route.path.slice(2),
                        path: route.path,
                        location: window.location.href
                    };
                }
            }
        }
    },
    {
        path: "/" + RouteTypes.notesRoute,
        name: "note",
        components: studyDocumentsPage,
        props: resultProps,
        meta: {
            isAcademic: true,
            showMobileFooter: true,
            analytics: {
                pageviewTemplate(route) {
                    return {
                        title: route.path.slice(1).charAt(0).toUpperCase() + route.path.slice(2),
                        path: route.path,
                        location: window.location.href
                    };
                }
            }
        }
    },
    // {
    //     path: "/uniselect/:step?",
    //     name: "uniselect",
    //     components: {
    //         default: university,
    //         header: pageHeader,
    //         schoolBlock: schoolBlock,
    //     },
    //     props: {
    //         default: (route) => ({
    //             step: route.params.step
    //         })
    //     },
    //     meta: {
    //         requiresAuth: true
    //     },
    // },
    {
        path: "/courses/",
        name: "setCourse",
        children: [
            {
                path: '',
                redirect: 'edit',
            },
            {
                path: 'add',
                name: 'addCourse',
                component: addCourse
            },
            {
                path: 'edit',
                name: 'editCourse',
                component: editCourse
            },
            {
                path: '*',
                redirect: 'edit',
            },
        ],
        components: {
            default: setCourse,
            header: pageHeader,
            schoolBlock: schoolBlock,
        },
        props: {
            default: (route) => ({
                step: route.params.step
            })
        },
        meta: {
            requiresAuth: true
        },
    },
    {
        path: "/university/",
        name: "setUniversity",
        children: [
            {
                path: '',
                redirect: 'add',
            },
            {
                path: 'add',
                name: 'addUniversity',
                component: addUniversity
            },
            {
                path: '*',
                redirect: 'add',
            },
        ],
        components: {
            default: setUniversity,
            header: pageHeader,
            schoolBlock: schoolBlock,
        },
        props: {
            default: (route) => ({
                step: route.params.step
            })
        },
        meta: {
            requiresAuth: true
        },
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
        path: "/note/:universityName/:courseName/:id/:name",
        alias: ['/document/:universityName/:courseName/:id/:name'],
        name: "document",
        components: {
            default: showItem,
            header: pageHeader
        },
        props: {
            default: (route) => ({
                id: route.params.id
            }),
            header: () => ({
                submitRoute: '/note',
                currentSelection: "note"
            })
        }
    },

    {
        path: "/tutoring/:id?",
        name: 'tutoring',
        components: {
            default: tutorComponent,
            // header: bookDetailsHeader
        },
        header: () => ({
            submitRoute: '/tutoring',
        }),
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
    {
        path: "/flashcard/:university/:courseId/:courseName/:id/:itemName",
        name: "flashcard",
        components: {
            default: showFlashcard,
            header: previewHeader
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
    {
        path: "/question/:id",
        components: {
            default: viewQuestion,
            header: pageHeader
        },
        name: "question",
        props: {
            header: {
                submitRoute: '/ask',
                currentSelection: "ask"
            },
            default: (route) => ({
                id: route.params.id
            }),
        },
    },

    {
        path: "/profile/:id",
        components: {
            // default: viewProfile,
            default: newProfile,
            header: profilePageHeader,
        },
        name: "profile",
        props: {
            default: (route) => ({
                id: route.params.id
            })
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
            requiresAuth: false
        },
        props: {
            header: () => ({
                currentSelection: "ask"
            })
        },
    },

    {
        path: "/register",
        alias: ['/signin', '/resetpassword'],
        components: {
            default: login,
        },
        name: "registration",
        beforeEnter: (to, from, next) => {
            //prevent entering if loged in
            if(global.isAuth) {
                next(false);
            } else {
                next();
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

            header: () => ({
                currentSelection: "ask"
            })
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
                     meta: {
                         static: true
                     },
                     props: {
                         default: (route) => item.params ? item.params(route) : {}
                     }
                 });
}

export const routes = routes2;