import * as routeName from "./routeNames.js";
import store from '../store'
import {staticComponents} from './routesUtils.js';
import { router } from '../main.js';

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
            store.commit('clearComponent');
            store.dispatch('updateStudyRoomInformation',to.params.id)
                .then(()=>{
                    if(store.getters.getRoomIsBroadcast && (!store.getters.getStudyroomEnrolled && !store.getters.getRoomIsTutor)){
                        let routeData = router.resolve({
                            name: routeName.StudyRoomLanding,
                            params:{...to.params }
                        });
                        global.open(routeData.href, "_self"); 
                    }else{
                        next();

                    }
                })
                .catch((err)=>{
                    if(err?.response){
                        next('/');

                    }
                })
        }
    },
    {
        path: `/live/:id?`,
        name: 'studyroomSettings',
        redirect: { name: routeName.StudyRoomLanding }
    },
    {
        path: '/course/:id?',
        name: routeName.StudyRoomLanding,
        components: {
            default: () => import(`../components/pages/studyroomLandingPage/studyroomLandingPage.vue`),
            ...staticComponents([ 'footer']),
        },
        beforeEnter: (to, from, next) => {
            if(!to.params?.id){
                next('/');

            }else{
                store.dispatch('updateCourseDetails',to.params.id)
                     .then(()=>{
                        next();
                    })
                    .catch(()=>{
                        next('/');
                    })
            }
        }
    }
]