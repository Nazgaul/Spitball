import { staticComponents } from './routesUtils'
import { CourseCreate } from './routeNames'
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
            path: '*',
            redirect: 'create',
         }
      ],
      meta: {
         requiresAuth: true,
      },
   }
]