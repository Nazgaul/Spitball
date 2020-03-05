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
        },
        alias: 'studyroomSettings'
    },
    {
        path: `/${routeName.StudyRoom.path}/:id?`,
        name: routeName.StudyRoom.name,
        components: {
            default: () => import(`../components/studyroom/tutor.vue`),
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        },
    },
    { 
        path: routeName.StudyRoom.alias, 
        redirect: routeName.StudyRoom.path 
    }
]