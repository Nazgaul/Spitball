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
         ...staticComponents(['header', 'sideMenu']),
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
            path: 'edit/:id',
            name: CourseUpdate,
            // beforeEnter: (to, from, next) => {
            //    store.dispatch('getCourseInfo', to.params.id).then(()=>{
            //       next()
            //    })
            // }
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