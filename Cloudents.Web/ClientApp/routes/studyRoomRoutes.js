import store from '../store'
import * as routeName from "./routeNames.js";


export const studyRoomRoutes = [
    // {
    //     path: "/studyroomLobby/:id?",
    //     name: "studyRoomLobby",
    //     beforeEnter: (to, from, next) => {
    //         // ...
    //         debugger
    //         next()
    //     },
    //     props: {
    //         default: (route) => ({
    //             id: route.params.id
    //         })
    //     }
    // },
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
                            // next({name:routeName.StudyRoomSettings,params:{id:to.params.id,step:'fasd'}});

                            // check if is StudentSide && is Student needPayment
                            let isStudentNeedPayment = (!roomProps.isTutor && roomProps.needPayment);
                            if(isStudentNeedPayment){
                                next({name:routeName.StudyRoomSettings,params:{id:to.params.id}});
                                return
                            }
                            next()
                            return
                        })
                    }
                }else{
                    // check if is StudentSide && is Student needPayment
                    let isStudentNeedPayment = (!store.getters.getStudyRoomData.isTutor && store.getters.getStudyRoomData.needPayment);
                    if(isStudentNeedPayment){
                        next({name:routeName.StudyRoomSettings,params:{id:to.params.id}});
                        return
                    }
                    next()    
                    return            
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