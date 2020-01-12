import {dashboardRoutes} from './routes/dashboardRoutes.js';
import {profileRoutes} from './routes/profileRoutes.js';
import {studyRoomRoutes} from './routes/studyRoomRoutes.js';
import {registrationRoutes} from './routes/registrationRoutes.js';
import {landingRoutes} from './routes/landingRoutes.js';
import {questionRoutes} from './routes/questionRoutes.js';
import {itemRoutes} from './routes/itemRoutes.js';



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


let routes2 = [
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
    ...landingRoutes,
    ...registrationRoutes,
    ...studyRoomRoutes,
    ...profileRoutes,
    ...dashboardRoutes,
    ...questionRoutes,
    ...itemRoutes,
    {
        path:'*',
        redirect : () => {
            window.location = "/error/notfound?client=true";
        }

    }
];
export const routes = routes2;
