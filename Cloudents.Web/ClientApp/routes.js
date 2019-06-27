import * as RouteTypes from "./routeTypes";

//const resultContent = () => import('./components/results/Result.vue');
const HomeworkHelpComponent = () => import('./components/results/HomeworkHelp/HomeworkHelp.vue');
const StudyDocumentsComponent = () => import('./components/results/StudyDocuments/StudyDocuments.vue');
const TutorsComponent = () => import('./components/results/Tutors/Tutors.vue');

const pageHeader = () => import('./components/header/header.vue');

const schoolBlock = () => import('./components/schoolBlock/schoolBlock.vue');
const verticalsTabs = () => import('./components/header/verticalsTabs.vue');
import { staticRoutes } from "./components/satellite/satellite-routes";

const showItem = () => import("./components/preview/Item.vue");
const satelliteHeader = () => import("./components/satellite/header.vue");
//const previewHeader = () => import("./components/helpers/header.vue");
const viewQuestion = () => import("./components/question/question-details/questionDetails.vue");
const wallet = () => import("./components/wallet/wallet.vue");
const newProfile = () => import("./components/new_profile/new_profile.vue");
const profilePageHeader = () => import("./components/new_profile/header/header.vue");
const login = () => import("./components/new_registration/login.vue");

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
const studyRoomsComponent = () => import("./components/studyRooms/studyRooms.vue");
const studentOrTutor= () => import("./components/studentOrTutor/studentOrTutor.vue");

const tutorLandingPage=()=> import("./components/tutorLandingPage/tutorLandingPage.vue")

const landingPage = () => import('./components/landingPage/landingPage.vue');
const FindTutor = () => import('./components/landingPage/pages/FindTutor.vue');
// import HowItWorks from "./components/landingPage/pages/HowItWorks.vue";
// import BecomeTutor from "./components/landingPage/pages/BecomeTutor.vue";
const registerPage = () => import('./components/loginPageNEW/pages/registerPage.vue')


function dynamicPropsFn(route) {
    let newName = route.path.slice(1);

    return {
        name: newName,
        query: route.query,
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

// const resultPage = {
//     default: resultContent,
//     header: pageHeader,
//     verticals: verticalsTabs,
//     schoolBlock: schoolBlock,
// };
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
const tutorPage = {
    default: TutorsComponent,
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

const studyRoomsPage = {
    default: studyRoomsComponent,
    header: pageHeader,
    schoolBlock: schoolBlock
};

let routes2 = [
    {
        path: "/",
        name: "landingPage",
        components: {
            default: landingPage,
        },
        children:[
            {
                path: '',
                component: FindTutor
            },
            {
                path: "/tutor-list",
                name: "tutorLandingPage",
                components: {
                    default: tutorLandingPage,
                },
            },
            
        ]
    },
    
    // {
    //     path: "/result",
    //     name: "result",
    //     alias: [
    //         // "/" + RouteTypes.questionRoute,
    //         // "/" + RouteTypes.notesRoute,
    //         // "/" + RouteTypes.tutorRoute,
    //     ],
    //     components: resultPage,
    //     props: resultProps,
    //     meta: {
    //         isAcademic: true,
    //         showMobileFooter: true,
    //         analytics: {
    //             pageviewTemplate(route) {
    //                 return {
    //                     title: route.path.slice(1).charAt(0).toUpperCase() + route.path.slice(2),
    //                     path: route.path,
    //                     location: window.location.href
    //                 };
    //             }
    //         }
    //     }
    // },
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
        path: "/" + RouteTypes.tutorRoute,
        name: "tutors",
        components: tutorPage,
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
    {
        path: "/courses/",
        name: "setCourse",
        children: [
            {
                path: '',
                redirect: 'edit',
                meta: {
                    requiresAuth: true
                },
            },
            {
                path: 'add',
                name: 'addCourse',
                component: addCourse,
                meta: {
                    requiresAuth: true
                },
            },
            {
                path: 'edit',
                name: 'editCourse',
                component: editCourse,
                meta: {
                    requiresAuth: true
                },
            },
            {
                path: '*',
                redirect: 'edit',
                meta: {
                    requiresAuth: true
                },
            },
        ],
        components: {
            default: setCourse,
            header: pageHeader,
            schoolBlock: schoolBlock,
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
                meta: {
                    requiresAuth: true
                },
            },
            {
                path: 'add',
                name: 'addUniversity',
                component: addUniversity,
                meta: {
                    requiresAuth: true
                },
            },
            {
                path: '*',
                redirect: 'add',
                meta: {
                    requiresAuth: true
                },
            },
        ],
        components: {
            default: setUniversity,
            header: pageHeader,
            schoolBlock: schoolBlock,
        },
        meta: {
            requiresAuth: true
        },
    },

    {
        path: "/note/:courseName/:name/:id",
        alias: ['/document/:courseName/:name/:id'],
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
        path: "/studyroom/:id?",
        name: 'tutoring',
        components: {
            default: tutorComponent,
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
        path: "/study-rooms",
        name: 'studyRooms',
        components: studyRoomsPage,
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
        path: "/profile/:id/:name",
        components: {
            default: newProfile,
            header: profilePageHeader,
        },
        name: "profile",
        // meta:{
        //     showMobileFooter: true,
        // },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        },
    },
    {
        path: "/student-or-tutor",
        components: {
            // default: viewProfile,
            default: studentOrTutor,
            schoolBlock: schoolBlock,
            header: pageHeader,
        },
        name: "studentTutor",
        meta: {
            requiresAuth: true
        },
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
            requiresAuth: true
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
        path: "/registerPageNEW",
        components: {
            default: registerPage,
        },
        name: "registerPageNEW",
        beforeEnter: (to, from, next) => {
            //prevent entering if loged in
            if(global.isAuth) {
                next(false);
            } else {
                next();
            }
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