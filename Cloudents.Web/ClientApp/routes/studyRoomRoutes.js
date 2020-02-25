import store from '../store'
import * as routeName from "./routeNames.js";

export const studyRoomRoutes = [
    {
        path: "/studyroomSettings/:id?",
        name: 'roomSettings',
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
        path: "/studyroom/:id?",
        name: 'tutoring',
        components: {
            default: () => import(`../components/studyroom/tutor.vue`),
        },
        beforeEnter: (to, from, next) => {
            if(to.params.id){
                if(!store.getters.getStudyRoomData){
                    if(to.params.id){
                        store.dispatch('maor_updateStudyRoomInformation',to.params.id).then((roomProps)=>{
                            roomProps.isTutor = store.getters.accountUser.id == roomProps.tutorId;
                            store.dispatch('updateStudyRoomProps',roomProps);
                            store.dispatch('maor_studyRoomMiddleWare',{to, from, next})
                        })
                    }
                }else{
                    store.dispatch('maor_studyRoomMiddleWare',{to, from, next})   
                }
            }else{
                next()
            }
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
]