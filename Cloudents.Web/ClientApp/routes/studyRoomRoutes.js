import store from '../store'
import * as routeName from "./routeNames.js";

export const studyRoomRoutes = [
    {
        path: `/${routeName.StudyRoomSettings.path}/:id?`, 
        name: routeName.StudyRoomSettings.name,
        components: {
            default: () => import(`../components/studyroomSettings/studyroomSettings.vue`),
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
        path: `/${routeName.StudyRoom.path}/:id?`,
        name: routeName.StudyRoom.name,
        components: {
            default: () => import(`../components/studyroom/tutor.vue`),
        },
        beforeEnter: (to, from, next) => {
            if(!to.params.id){
                next()
                return;
            }else{
                store.dispatch('maor_updateStudyRoomInformation',to.params.id).then(()=>{
                    next()
                }).catch((nextStepRoute)=>{
                    if(to.query.dialog){
                        next();
                        return
                    }
                    next({...nextStepRoute })
                })
            }
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
]