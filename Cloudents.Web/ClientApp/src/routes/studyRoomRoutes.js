import * as routeName from "./routeNames.js";
import store from '../store'
import {staticComponents} from './routesUtils.js';

export const studyRoomRoutes = [
    {
        path: `/studyroomSettings/:id?`,
        name: 'studyroomSettings',
        redirect: { name: routeName.StudyRoom }
    },
    {
        path: `/studyroom/:id?`,
        name: routeName.StudyRoom,
        // components: {
        //     default: () => import(`../components/studyroom/tutor.vue`),
        // },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        },
        meta: {
            layout : 'studyRoomLayout'
        },
        beforeEnter: (to, from, next) => {
            if(!to.params?.id){
                next('/');
                return
            }
            store.commit('clearComponent')
            next();
        }
    },
    {
        path: '/live/:id?',
        name: routeName.StudyRoomLanding,
        components: {
            default: () => import(`../components/pages/studyroomLandingPage/studyroomLandingPage.vue`),
            ...staticComponents([ 'footer']),
        },
        // beforeEnter: (to, from, next) => {
        //     if(!to.params?.id){
        //         next('/');
        //         return
        //     }
        //     next();
        // }
    }
]