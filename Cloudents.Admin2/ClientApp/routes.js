import Home from './components/Home.vue';

import Question from './components/question/question.vue'
import QMark from './components/question/questionComponents/mark/markQuestion.vue'
import QDelete from './components/question/questionComponents/delete/deleteQuestion.vue'
import QAdd from './components/question/questionComponents/add/addQuestion.vue'
import QAddBulk from './components/question/questionComponents/addBulk/addBulkQuestions.vue'
import QAccept from './components/question/questionComponents/accept/acceptQuestion.vue'
import QPending from './components/question/questionComponents/pendingQuestions/pendingQuestions.vue'

import User from './components/user/user.vue'
import UToken from './components/user/token/tokenUser.vue'
import UCashout from './components/user/cashout/cashoutUser.vue'
import USuspend from './components/user/suspend/suspendUser.vue'

import Document from './components/document/document.vue'
import approveDelete from './components/document/documentComponents/approveDelete/approveDelete.vue'
import Dev from './components/dev/dev.vue'
import UChangeCountry from './components/dev/changeCountry/changeCountry.vue'
import UDelete from './components/dev/deleteUser/deleteUser.vue'

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
            path:'acceptQuestion',
            component: QAccept
          },
          {
            path:'pendingQuestions',
            component: QPending
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
        path: '/document',
        name: 'document',
        component: Document,
        children: [
            {
                path: '',
                redirect: 'approveDelete',
            },

            {
                path:'approveDelete',
                component: approveDelete
            },
            {
                path: '*',
                redirect: 'approveDelete',
            },
        ]
    },
     {
      path: '/dev',
      name: 'dev',
      component: Dev,
      children: [
        {
          path: '',
          redirect: 'change-country',
        },
        {
          path:'change-country',
          component: UChangeCountry
        },
        {
          path:'delete-user',
          component: UDelete
        },
        {
          path: '*',
          redirect: 'change-country',
        },
      ]
    },
    {
         path: '/*',
         redirect: '/home',
       },
]

  
