import * as routeName from "./routeNames.js";

export const studyRoomRoutes = [
    {
        path: `/studyroomSettings/:id?`, 
        name: routeName.StudyRoomSettings,
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
        path: `/studyroom/:id?`,
        name: routeName.StudyRoom,
        components: {
            default: () => import(`../components/studyroom/tutor.vue`),
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
]