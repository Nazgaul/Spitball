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
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
]