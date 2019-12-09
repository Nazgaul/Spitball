import * as RouteTypes from "./routeTypes";
const globalBanner = () => import('./components/pages/layouts/banner/banner.vue');

const feeds = () => import('./components/results/feeds/Feeds.vue');
const pageHeader = () => import('./components/pages/layouts/header/header.vue');
const sideMenu = () => import('./components/pages/layouts/sideMenu/sideMenu.vue');
// const document = () => import("./components/document/document.vue");
const itemPage = () => import("./components/pages/itemPage/item.vue");

const viewQuestion = () => import("./components/question/question-details/questionDetails.vue");
const wallet = () => import("./components/wallet/wallet.vue");
const newProfile = () => import("./components/new_profile/new_profile.vue");

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

const landingPage = () => import('./components/pages/landingPage/landingPage.vue');
const tutorLandingPage=()=> import("./components/tutorLandingPage/tutorLandingPage.vue");
const landingPageFooter = () => import('./components/pages/layouts/footer/footer.vue')

const homePage = () => import('./components/landingPage/pages/homePage.vue');
const registerPage = () => import('./components/loginPageNEW/pages/registerPage.vue');

const myPurchases = () => import('./components/pages/dashboard/myPurchases/myPurchases.vue');


function dynamicPropsFn(route) {
    let newName = route.path.slice(1);

    return {
        name: newName,
        query: route.query,
        params: route.params
    };
}

function headerResultPageFn(route) {
    return {
    };
}

function verticalResultPageFn(route) {
    return {
        
    };
}
const resultProps = {
    default: dynamicPropsFn,
    header: headerResultPageFn,
    verticals: verticalResultPageFn
};
const feedPage = {
    default: feeds,
    banner: globalBanner,
    header: pageHeader,
    sideMenu: sideMenu
};

const studyRoomsPage = {
    default: studyRoomsComponent,
    banner: globalBanner,
    header: pageHeader,
    sideMenu: sideMenu
};

let routes2 = [
    {
        path: "/",
        name: "landingPage",
        components: {
            default: landingPage,
            banner: globalBanner,
            header: pageHeader,
            footer: landingPageFooter
        },
        children:[
            {
                path: '',
                component: homePage
            },
            {
                path: "/tutor-list/:course?",
                name: "tutorLandingPage",
                components: {
                    default: tutorLandingPage
                },
                meta: {
                    showMobileFooter: true, 
                }
            }
            
        ]
    },
    
    {
        path: "/" + RouteTypes.feedRoute,
        name: "feed",
        components: feedPage,
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
                }
            },
            {
                path: 'add',
                name: 'addCourse',
                component: addCourse,
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: 'edit',
                name: 'editCourse',
                component: editCourse,
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: '*',
                redirect: 'edit',
                meta: {
                    requiresAuth: true
                }
            }
        ],
        components: {
            default: setCourse,
            banner: globalBanner,
            header: pageHeader,
            sideMenu: sideMenu
        },
        meta: {
            requiresAuth: true
        }
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
                }
            },
            {
                path: 'add',
                name: 'addUniversity',
                component: addUniversity,
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: '*',
                redirect: 'add',
                meta: {
                    requiresAuth: true
                }
            }
        ],
        components: {
            default: setUniversity,
            banner: globalBanner,
            header: pageHeader,
            sideMenu: sideMenu
        },
        meta: {
            requiresAuth: true
        }
    },
    {
        path: "/document/:courseName/:name/:id",
        // alias: ['/document/:courseName/:name/:id'],
        name: "document",
        components: {
            header: pageHeader,
            banner: globalBanner,
            default: itemPage,
            sideMenu: sideMenu,
        },
        props: {
            default: (route) => ({
                id: route.params.id
            }),
        }
    },
    {
        path: "/studyroom/:id?",
        name: 'tutoring',
        components: {
            default: tutorComponent
        },
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
            banner: globalBanner,
            sideMenu: sideMenu,
            header: pageHeader
        },
        name: "question",
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },

    {
        path: "/profile/:id/:name",
        components: {
            default: newProfile,
            banner: globalBanner,
            header: pageHeader,
            sideMenu: sideMenu,
        },
        name: "profile",
        meta:{
            showMobileFooter: true,
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
    {
        path: "/student-or-tutor",
        components: {
            // default: viewProfile,
            default: studentOrTutor,
            sideMenu: sideMenu,
            banner: globalBanner,
            header: pageHeader
        },
        name: "studentTutor",
        meta: {
            requiresAuth: true
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },

    {
        path: "/wallet",
        components: {
            default: wallet,
            header: pageHeader,
            banner: globalBanner,
            sideMenu: sideMenu,
        },
        name: "wallet",
        meta: {
            requiresAuth: true
        },
    },

    {
        path: "/register",
        alias: ['/signin', '/resetpassword'],
        components: {
            default: registerPage
        },
        name: "registration",
        beforeEnter: (to, from, next) => {
            if(global.isAuth) {
                next(false);
            } else {
                next();
            }
        }
    },
    {
        path: "/myPurchases",
        components: {
            default: myPurchases,
            header: pageHeader,
            banner: globalBanner,
            sideMenu: sideMenu,
        },
        name: "myPurchases",
        meta: {
            requiresAuth: true
        },
    },
];


export const routes = routes2;