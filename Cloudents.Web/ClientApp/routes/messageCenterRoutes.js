import * as routeName from "./routeNames.js";
import { staticComponents } from './routesUtils.js';

export const messageCenterRoutes = [
   {
      path: `/messages/:id?`,
      name: routeName.MessageCenter,
      components: {
          default: () => import(`../components/pages/messageCenter/messageCenter.vue`),
          ...staticComponents(['header'])
      },
    //   props: {
    //       default: (route) => ({
    //           id: route.params.id
    //       })
    //   },
      meta: {
        requiresAuth: true
      },
    //   beforeEnter: (to, from, next) => {
    //   }
  }
];