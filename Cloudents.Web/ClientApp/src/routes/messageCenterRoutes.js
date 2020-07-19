import * as routeName from "./routeNames.js";
import store from '../store'
import { staticComponents } from './routesUtils.js';

export const messageCenterRoutes = [
  {
    path: `/messages/:id?`,
    name: routeName.MessageCenter,
    components: {
        default: () => import(`../components/pages/messageCenter/messageCenter.vue`),
        ...staticComponents(['header'])
    },
    meta: {
      requiresAuth: true,
      showMobileFooter: true,
    },
    beforeEnter: (to, from, next) => {
      let conversationsList = store.getters.getConversations
      if(conversationsList.length){
        next()
        return
      }else{
        store.dispatch("getAllConversations").then((data)=>{
          if(data) {
            next()
            return
          }
          next('/')
        })
      }
    }
  }
];