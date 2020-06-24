import { staticComponents } from './routesUtils.js';
import * as routeName from "./routeNames.js";
import store from '../store'
import vuetify from '../plugins/vuetify';

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
        beforeEnter(to, from, next) {
            let options = {
                id: to.params.id,
                pageSize: vuetify.framework.breakpoint.xsOnly ? 3 : 8
            }            
            store.dispatch('syncProfile', options).then(() => {
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