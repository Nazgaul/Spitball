import { staticComponents } from './routesUtils.js';
import * as routeName from "./routeNames.js";
import store from '../store'
import vuetify from '../plugins/vuetify';

export const profileRoutes = [
    {
        path: "/profile/:id?/:name?",
        name: routeName.Profile,
        components: {
            default: () => import(`../components/new_profile/new_profile.vue`),
            ...staticComponents(['banner', 'header'])
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
            let options = {
                id: to.params.id,
                params:{
                    page: 0,
                    pageSize: vuetify.framework.breakpoint.xsOnly ? 3 : 8,
                }
            }            
            store.dispatch('syncProfile', options).then(() => {
                next()
            }).catch(ex => {
                console.error(ex);
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