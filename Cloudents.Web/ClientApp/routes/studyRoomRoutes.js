import * as routeName from "./routeNames.js";

export const studyRoomRoutes = [
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