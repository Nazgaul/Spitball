import Home from './components/Home.vue';

export const routes = [
    {
       path: '/home',
       name: 'home',
       component: Home
    },
    {
         path: '/*',
         redirect: '/home',
       },
]
// export const routes = [
//     {
//       path: '/home',
//       name: 'home',
//       component: Home
//     },
//     {
//       path: '/question',
//       name: 'question',
//       // route level code-splitting
//       // this generates a separate chunk (question.[hash].js) for this route
//       // which is lazy-loaded when the route is visited.
//       // component: () => import(/* webpackChunkName: "about" */ './components/question/question')
//       component: Question,
//       children: [
//         {
//           path: '',
//           redirect: 'mark',
//         },
//         {
//           path: 'mark',
//           component: QMark
//         },
//         {
//           path:'delete',
//           component: QDelete
//         },
//         {
//           path:'add',
//           component: QAdd
//         },
//       ]
//     },


//     {
//       path: '/*',
//       redirect: '/home',
//     },
//   ]

  
