import { staticComponents } from './routesUtils.js';
import * as routeName from "./routeNames.js";
import store from '../store'

export const profileRoutes = [
    {
        path: "/profile/:id?/:name?",
        name: routeName.Profile,
        components: {
            default: () => import(`../components/new_profile/new_profile.vue`),
            ...staticComponents(['header'])
        },
        beforeEnter(to, from, next) {          
            if (to.params.id == null) {
                if (store.getters.getUserLoggedInStatus) {
                    let id = store.getters.getAccountId
                    let name = store.getters.getAccountName
                    next({name:routeName.Profile, params : {id, name}})
                } else {
                    next('/')
                }
                return
            }
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