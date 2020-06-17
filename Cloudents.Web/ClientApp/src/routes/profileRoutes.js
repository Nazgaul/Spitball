import { staticComponents } from './routesUtils.js';
import store from '../store'
import * as routeName from "./routeNames.js";

export const profileRoutes = [
    {
        path: "/profile",
        redirect: { name: routeName.Feed }
    },
    {
        path: "/profile/:id/:name",
        name: routeName.Profile,
        components: {
            default: () => import(`../components/new_profile/new_profile.vue`),
            ...staticComponents(['banner', 'header'])
        },
        beforeEnter: (to, from, next) => {
            store.dispatch('syncProfile', to.params).then(({user}) => {
                if(user.isTutor) {
                    next()
                    return
                }
                if(from.fullPath) {
                    next(from.fullPath)
                    return
                }
                next('/')
            }).catch(() => {
                next({name: routeName.notFound})
            })
        },
        props: {
            default: (route) => {
                return {
                    id: route.params.id
                }
            }
        }
    },
]