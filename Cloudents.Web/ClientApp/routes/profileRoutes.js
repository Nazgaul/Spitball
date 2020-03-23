import { staticComponents } from './routesUtils.js';
import store from '../store'
import * as routeName from "./routeNames.js";

export const profileRoutes = [
    {
        path: "/profile",
        redirect: { name: routeName.Feed }
    },
    {
        name: routeName.Profile,
        path: "/profile/:id/:name",
        components: {
            default: () => import(`../components/new_profile/new_profile.vue`),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        beforeEnter: (to, from, next) => {
            if (to.params?.id) {
                next()
            } else {
                if (store.getters.accountUser?.id) {
                    let { id, name } = store.getters.accountUser;
                    next({ name: routeName.Profile, params: { id, name } })
                } else {
                    next({ name: routeName.Feed })
                }
            }
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
    {
        path: "/profilet/",
        name: routeName.ProfileT,
        components: {
            default: () => import(`../components/pages/profile/profileTutor/profileTutor.vue`),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        }
    },
]