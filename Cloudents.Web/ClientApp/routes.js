function lazyComponent(path) {
    return () => import(`./components/${path}.vue`);
}

const routesDefaults = {
    banner: lazyComponent('pages/layouts/banner/banner'),
    header: lazyComponent('pages/layouts/header/header'),
    sideMenu: lazyComponent('pages/layouts/sideMenu/sideMenu'),
    footer: lazyComponent('pages/layouts/footer/footer')
}

function staticComponents(components) {
    let defaultRoutes = {};

    components.forEach(comp => defaultRoutes[comp] = routesDefaults[comp]);
        
    return defaultRoutes;
}

function dynamicPropsFn(route) {
    let newName = route.path.slice(1);

    return {
        name: newName,
        query: route.query,
        params: route.params
    };
}

const resultProps = {
    default: dynamicPropsFn,
};

const feedPage = {
    default: lazyComponent('results/feeds/Feeds'),
    ...staticComponents(['banner', 'header', 'sideMenu'])
};

const dashboardPages = {
    default: lazyComponent('pages/dashboardPage/dashboardPage'),
    ...staticComponents(['banner', 'header', 'sideMenu'])
};

let routes2 = [
    {
        path: "/",
        name: "landingPage",
        components: {
            default: lazyComponent('pages/landingPage/landingPage'),
            ...staticComponents(['banner', 'header', 'footer']),
        },
        children:[
            {
                path: '',
                component: lazyComponent('landingPage/pages/homePage')
            },
            {
                path: "/tutor-list/:course?",
                name: "tutorLandingPage",
                components: {
                    default: lazyComponent('tutorLandingPage/tutorLandingPage')
                },
                meta: {
                    showMobileFooter: true, 
                }
            }
            
        ]
    },
    
    {
        path: "/" + 'feed',
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
                component: lazyComponent('courses/addCourses/addCourses'),
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: 'edit',
                name: 'editCourse',
                component: lazyComponent('courses/editCourses/editCourses'),
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
            default: lazyComponent('courses/courses'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
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
                component: lazyComponent('university/addUniversity/addUniversity'),
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
            default: lazyComponent('university/university'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        meta: {
            requiresAuth: true
        }
    },
    {
        path: "/document/:courseName/:name/:id",
        name: "document",
        components: {
            default: lazyComponent('pages/itemPage/item'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        props: {
            default: (route) => ({
                id: route.params.id
            }),
        }
    },
    {
        path: "/studyroomSettings/:id?",
        name: 'roomSettings',
        components: {
            default: lazyComponent('studyroomSettings/studyroomSettings')
            // default: roomSettings
        },
        header: () => ({
            submitRoute: '/tutoring'
        }),
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
    {
        path: "/studyroom/:id?",
        name: 'tutoring',
        components: {
            default: lazyComponent('studyroom/tutor')
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
            default: lazyComponent('question/question-details/questionDetails'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        name: "question",
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
{
path : "/profile",
redirect: { name: 'feed' }
},
    {
        path: "/profile/:id/:name",
        components: {
            default: lazyComponent('new_profile/new_profile'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        name: "profile",
        // meta:{
        //     showMobileFooter: true,
        // },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
    {
        path: "/student-or-tutor",
        components: {
            default: lazyComponent('studentOrTutor/studentOrTutor'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
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
            default: lazyComponent('wallet/wallet'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
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
            default: lazyComponent('loginPageNEW/pages/registerPage')
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
        path: "/my-followers",
        components: dashboardPages,
        name: "myFollowers",
        props: {
            default: (route) => ({
                component: route.name,
            })
        },
        meta: {
            requiresAuth: true,
            showMobileFooter: true,
        },
    },
    {
        path: "/my-sales",
        components: dashboardPages,
        name: "mySales",
        props: {
            default: (route) => ({
                component: route.name,
            })
        },
        meta: {
            requiresAuth: true,
            showMobileFooter: true,
        },
    },
    {
        path: "/my-content",
        components: dashboardPages,
        name: "myContent",
        props: {
            default: (route) => ({
                component: route.name,
            })
        },
        meta: {
            requiresAuth: true,
            showMobileFooter: true,
        },
    },
    {
        path: "/my-purchases",
        components: dashboardPages,
        name: "myPurchases",
        props: {
            default: (route) => ({
                component: route.name,
            })
        },
        meta: {
            requiresAuth: true,
            showMobileFooter: true,
        },
    },
    {
        path: "/study-rooms",
        components: dashboardPages,
        name: "myStudyRooms",
        props: {
            default: (route) => ({
                component: route.name,
            })
        },
        meta: {
            requiresAuth: true,
            showMobileFooter: true,
        },
    },
    {
        path:'*',
        redirect : () => {
            window.location = "/error/notfound?client=true";
        }

    }
];
export const routes = routes2;