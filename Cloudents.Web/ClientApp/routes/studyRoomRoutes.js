import * as routeName from "./routeNames.js";

export const studyRoomRoutes = [
    {
        path: `/studyroomSettings/:id?`,
        name: 'studyroomSettings',
        redirect: { name: routeName.StudyRoom }
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
    }
]