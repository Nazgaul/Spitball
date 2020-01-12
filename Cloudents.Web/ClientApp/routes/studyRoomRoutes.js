import {lazyComponent} from './routesUtils.js';



export const studyRoomRoutes = [
    {
        path: "/studyroomSettings/:id?",
        name: 'roomSettings',
        components: {
            default: lazyComponent('studyroomSettings/studyroomSettings')
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
            default: lazyComponent('studyroom/tutor')
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
]