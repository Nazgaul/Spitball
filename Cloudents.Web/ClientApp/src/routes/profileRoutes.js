import { staticComponents } from './routesUtils.js';
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
        props: {
            default: (route) => {
                return {
                    id: route.params.id
                }
            }
        }
    },
]