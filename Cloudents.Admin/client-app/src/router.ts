import Vue from 'vue'
import Router from 'vue-router'


import Home from './views/Home.vue'
import Question from '@/components/question/question'
import QMark from '@/components/question/questionComponents/mark/mark'
import QDelete from '@/components/question/questionComponents/delete/delete'
import QAdd from '@/components/question/questionComponents/add/add'

Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
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
      path: '/*',
      redirect: '/home',
    },
  ]
})
