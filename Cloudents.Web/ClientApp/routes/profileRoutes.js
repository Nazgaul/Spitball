import {staticComponents} from './routesUtils.js';

export const profileRoutes = [
    {
        path : "/profile",
        redirect: { name: 'feed' }
    },
    {
        path: "/profile/:id/:name",
        components: {
            default: () => import(`../components/new_profile/new_profile.vue`),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        name: "profile",
        // meta:{
        //     showMobileFooter: true,
        // },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
]