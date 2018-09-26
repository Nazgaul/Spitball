import Home from './components/Home.vue';

import Question from './components/question/question.vue'
import QMark from './components/question/questionComponents/mark/markQuestion.vue'
import QDelete from './components/question/questionComponents/delete/deleteQuestion.vue'
import QAdd from './components/question/questionComponents/add/addQuestion.vue'

import User from './components/user/user.vue'
export const routes = [
    {
       path: '/home',
       name: 'home',
       component: Home
    },
    {
        path: '/question',
        name: 'question',
        // route level code-splitting
        // this generates a separate chunk (question.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        // component: () => import(/* webpackChunkName: "about" */ './components/question/question')
        component: Question,
        children: [
          {
            path: '',
            redirect: 'mark',
          },
          {
            path: 'mark',
            component: QMark
          },
          {
            path:'delete',
            component: QDelete
          },
          {
            path:'add',
            component: QAdd
          },
        ]
      },
      {
        path: '/user',
        name: 'user',
        component: User
     },
    {
         path: '/*',
         redirect: '/home',
       },
]

  
