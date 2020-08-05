import store from '../store'
import { staticComponents } from './routesUtils'
import { CourseCreate, CourseUpdate } from './routeNames'
export const courseRoutes = [
   {
      path: "/courses",
      redirect: "/",
      children: [
         {
            path: '*',
            redirect: '/',
         }
      ]
   },
   {
      path: '/course',
      components: {
         default: () => import(`../components/pages/coursePage/coursePage.vue`),
         ...staticComponents(['banner', 'header', 'sideMenu']),
      },
      children: [
         {
            path: '/',
            redirect: 'create'
         },
         {
            path: 'create',
            name: CourseCreate,
         },
         {
            path: ':id/edit',
            name: CourseUpdate,
         },
         {
            path: '*',
            redirect: 'create',
         }
      ],
      meta: {
         requiresAuth: true,
      },
      beforeEnter: (to, from, next) => {
         if(store.getters.getUserLoggedInStatus && store.getters.accountUser.isTutor){
             next()
             return
         }
         next('/')
     }
   }
]