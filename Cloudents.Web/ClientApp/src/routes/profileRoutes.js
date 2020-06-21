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
            let option = {
                id: to.params.id,
                type: 'documents',
                params:{
                    page: 0,
                    pageSize: vuetify.framework.breakpoint.xsOnly ? 3 : 8,
                }
            }
            store.dispatch('syncProfile', option).then(({user}) => {
                if(user.isTutor) return next()

                let previousLink = from.fullPath || '/';
                next(previousLink);
            }).catch(ex => {
                console.error(ex);
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