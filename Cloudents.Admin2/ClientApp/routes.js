import Home from './components/Home.vue';

import Question from './components/question/question.vue'
import QMark from './components/question/questionComponents/mark/markQuestion.vue'
import QDelete from './components/question/questionComponents/delete/deleteQuestion.vue'
import QAdd from './components/question/questionComponents/add/addQuestion.vue'
import QAddBulk from './components/question/questionComponents/addBulk/addBulkQuestions.vue'

import User from './components/user/user.vue'
import UToken from './components/user/token/tokenUser.vue'
import UCashout from './components/user/cashout/cashoutUser.vue'
import USuspend from './components/user/suspend/suspendUser.vue'
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
          {
            path:'addBulk',
            component: QAddBulk
          },
          {
            path: '*',
            redirect: 'mark',
          },
        ]
      },
      {
        path: '/user',
        name: 'user',
        component: User,
        children: [
          {
            path: '',
            redirect: 'token',
          },
          {
            path: 'token',
            component: UToken
          },
          {
            path:'cashout',
            component: UCashout
          },
          {
            path:'suspend',
            component: USuspend
          },
          {
            path: '*',
            redirect: 'token',
          },
        ]
     },
    {
         path: '/*',
         redirect: '/home',
       },
]

  
