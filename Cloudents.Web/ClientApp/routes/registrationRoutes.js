import {lazyComponent} from './routesUtils.js';

export const registrationRoutes = [
   {
      path: "/register",
      alias: ['/signin', '/resetpassword'],
      components: {
          default: lazyComponent('loginPageNEW/pages/registerPage')
      },
      name: "registration",
      beforeEnter: (to, from, next) => {
          if(global.isAuth) {
              next(false);
          } else {
              next();
          }
      }
  },
]