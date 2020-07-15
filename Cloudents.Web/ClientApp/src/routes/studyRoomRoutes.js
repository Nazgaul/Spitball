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
            if(!store.getters.getUserLoggedInStatus){
                next({
                    name: routeName.StudyRoomLanding,
                    params:{...to.params}
                });
                return
            }
            store.commit('clearComponent');
            store.dispatch('updateStudyRoomInformation',to.params.id)
            .then(()=>{
                if(store.getters.getRoomIsNeedPayment){
                    next({
                        name: routeName.StudyRoomLanding,
                        params:{...to.params }
                    });
                    return;
                }else{
                    next();
                    return
                }
            })
            .catch((err)=>{
                if(err?.response){
                    next('/');
                    return
                }
            })
        }
    },
    {
        path: '/live/:id?',
        name: routeName.StudyRoomLanding,
        components: {
            default: () => import(`../components/pages/studyroomLandingPage/studyroomLandingPage.vue`),
            ...staticComponents([ 'footer']),
        },
        beforeEnter: (to, from, next) => {
            if(!to.params?.id){
                next('/');
                return
            }else{
                store.dispatch('updateStudyRoomInformation',to.params.id)
                    .then(()=>{
                        next();
                        return;
                    })
                    .catch((err)=>{
                        if(err?.response){
                            next('/');
                            return
                        }
                    })
            }
        }
    }
]