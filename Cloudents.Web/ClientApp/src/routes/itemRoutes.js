import store from '../store';
import * as routeNames from "./routeNames.js";

export const itemRoutes = [
    {
        path: "/document/:courseName/:name/:id",
        name: "document",
        props: {
            default: (route) => ({
                id: route.params.id
            }),
        },
        beforeEnter: (to, from, next) => {
            let itemId = to.params?.id;
            store.dispatch('documentRequest',itemId)
                .then(()=>{
                    let item = store.getters.getDocumentDetails;
                    next({
                        name: routeNames.Profile,
                        params: {
                            id: item.userId,
                            name: item.userName
                        },
                        query:{
                            d: item.id
                        }
                    })
                })
                .catch(()=>{
                    next('/')
                })
        }
    },
]