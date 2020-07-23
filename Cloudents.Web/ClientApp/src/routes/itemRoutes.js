import store from '../store';

export const itemRoutes = [
    {
        path: "/document/:courseName/:name/:id",
        name: "document",
        props: {
            default: (route) => ({
                id: route.params.id
            }),
        },
        beforeEnter: (to) => {
            let itemId = to.params?.id;
            store.dispatch('updateCurrentItem',itemId);
        }
    },
]