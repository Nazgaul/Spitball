import { staticComponents } from './routesUtils.js';
import * as routeName from "./routeNames.js";
import store from '../store'

export const profileRoutes = [
    {
        path: "/profile",
        redirect: "/"
    },
    {
        path: "/profile/:id/:name",
        name: routeName.Profile,
        components: {
            default: () => import(`../components/new_profile/new_profile.vue`),
            ...staticComponents(['banner', 'header'])
        },
        beforeEnter(to, from, next) {          
            store.dispatch('syncProfile', to.params.id).then(() => {
                next()
            }).catch(() => {
                let previousLink = from.fullPath || '/';
                next(previousLink);
            })
        },
        props: {
            default: (route) => {
                return {
                    id: route.params.id
                }
            }
        },
        meta: {
            tutorHeaderSlot: true,
        }
    },
]