//import Home from './components/Home.vue';

import question from './components/question/question.vue';
import qMark from './components/question/questionComponents/mark/markQuestion.vue';
import qDelete from './components/question/questionComponents/delete/deleteQuestion.vue';
import qAdd from './components/question/questionComponents/add/addQuestion.vue';
import qAddBulk from './components/question/questionComponents/addBulk/addBulkQuestions.vue';
import qAccept from './components/question/questionComponents/accept/acceptQuestion.vue';
import qPending from './components/question/questionComponents/pendingQuestions/pendingQuestions.vue';
import qFlagged from './components/question/questionComponents/flaggedQuestions/flaggedQuestions.vue';

import answer from './components/answer/answer.vue';
import aDelete from './components/answer/answerComponents/delete/deleteAnswer.vue';
import aAccept from './components/answer/answerComponents/accept/acceptAnswer.vue';
import aFlagged from './components/answer/answerComponents/flaggedAnswers/flaggedAnswers.vue';

import user from './components/user/user.vue';
import uToken from './components/user/token/tokenUser.vue';
import uCashout from './components/user/cashout/cashoutUser.vue';
import uSuspend from './components/user/suspend/suspendUser.vue';
import activeUsers from './components/user/activeUsers/activeUsers.vue';

import document from './components/document/document.vue';
import approveDelete from './components/document/documentComponents/approveDelete/approveDelete.vue';
import documentDelete from './components/document/documentComponents/documentDelete/documentDelete.vue';
import flaggedDocument from './components/document/documentComponents/flaggedDocument/flaggedDocument.vue';

import dev from './components/dev/dev.vue';
import uChangeCountry from './components/dev/changeCountry/changeCountry.vue';
import uDelete from './components/dev/deleteUser/deleteUser.vue';

import userMain from './components/userMainView/userMainView.vue';
import userQuestions from './components/userMainView/userQuestions/userQuestions.vue';
import userAnswers from './components/userMainView/userAnswers/userAnswers.vue';
import userDocuments from './components/userMainView/userDocuments/userDocuments.vue';
import userPurchasedDocuments from './components/userMainView/userPurchasedDocuments/userPurchasedDocuments.vue';
import userConversations from './components/userMainView/userConversations/userConversations.vue';
import userSessions from './components/userMainView/userSessions/userSessions.vue';

import management from './components/management/Management.vue';
import coursesPending from './components/management/coursesPending/coursesPending.vue';
import universityPending from './components/management/universityPending/universityPending.vue';

import conversation from './components/conversation/conversation.vue';
import conversations from './components/conversation/conversationComponent/conversationDetalis/conversationDetails.vue';
import startConversations from './components/conversation/startconversation.vue'
// import conversationMessages from './components/conversation/conversationComponent/conversationMessages/conversationMessages.vue';

import tutors from './components/tutor/tutor.vue';
import pendingTutors from './components/tutor/pendingTutors/pendingTutor.vue';
import studyRoom from './components/studyRoom/studyRoom.vue';
import studyRoomSession from './components/studyRoom/studyRoomComponents/sessions/studyRoomsSessions.vue';

export const routes = [
    {
       path: '/home/:userId?',
       name: 'userMainView',
       component: userMain,
       props: true,
        children: [
            {
                name: 'userQuestions',
                path: 'userQuestions',
                component: userQuestions
            },
            {
                name: 'userAnswers',
                path:'userAnswers',
                component: userAnswers
            },
            {
                name:'userDocuments',
                path:'userDocuments',
                component: userDocuments
            },
            {
                name:'userPurchasedDocuments',
                path:'userPurchasedDocuments',
                component: userPurchasedDocuments
            },
            {
              name:'userConversations',
              path:'userConversations',
              component: conversations
            },
            {
              name:'userSessions',
              path:'userSessions',
              component: userSessions
            }
            // {
            //     path:'userDownvotes',
            //     component: userDownvotes
            // },
            // {
            //     path:'userFlags',
            //     component: userFlags
            // },
        ]
    },
    {
        path: '/tutor',
        name: 'tutor',
        component: tutors,
        children: [
            {
                path: '',
                redirect: 'pendingTutors'
            },
            {
            path: 'pendingTutors',
            component: pendingTutors
            }
            ]

    },
    {
        path: '/question',
        name: 'question',
        // route level code-splitting
        // this generates a separate chunk (question.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        // component: () => import(/* webpackChunkName: "about" */ './components/question/question')
        component: question,
        children: [
          {
            path: '',
            redirect: 'mark'
          },
          {
            path: 'mark',
            component: qMark
          },
          {
            path:'delete',
            component: qDelete
          },
          {
            path:'add',
            component: qAdd
          },
          {
            path:'addBulk',
            component: qAddBulk
          },
          {
            path:'acceptQuestion',
            component: qAccept
          },
          {
            path:'pendingQuestions',
            component: qPending
          },
          {
            path:'flaggedQuestions',
            component: qFlagged
          },
          {
            path: '*',
            redirect: 'mark'
          }
        ]
    },

    {
        path: '/answer',
        name: 'answer',
        component: answer,
        children: [
            {
              path: '',
              redirect: 'flaggedAnswers'
            },
            {
                path: 'delete',
                component: aDelete
            },
          
            {
                path: 'acceptAnswer',
                component: aAccept
            },
            {
                path: 'flaggedAnswers',
                component: aFlagged
            },
            {
              path: '*',
              redirect: 'delete'
            }
        ]
    },

      {
        path: '/user',
        name: 'user',
        component: user,
        children: [
          {
            path: '',
            redirect: 'token'
          },
          {
            path: 'token',
            component: uToken
          },
          {
            path:'cashout',
            component: uCashout
          },

          {
            path:'suspend',
            component: uSuspend
          },
            {
                path:'active-users',
                component: activeUsers
            },
          {
            path: '*',
            redirect: 'token'
          }
        ]
     },
    {
        path: '/document',
        name: 'document',
        component: document,
        children: [
            {
                path: '',
                redirect: 'approveDelete'
            },
            {
                path:'approveDelete',
                component: approveDelete
            },
            {
                path:'documentDelete',
                component: documentDelete
            },
            {
                path:'flaggedDocument',
                component: flaggedDocument
            },
            {
                path: '*',
                redirect: 'approveDelete'
            }
        ]
    },

    {
        path: '/management',
        name: 'management',
        component: management,
        children: [
            {
                path: '',
                redirect: 'coursesPending'
            },
            {
                path: 'coursesPending',
                component: coursesPending
            },
            {
                path: 'universityPending',
                component: universityPending
            }
        ]
    },

    {
        path: '/conversation',
        name: 'conversation',
        component: conversation,
        children: [
            {
                path: '',
                redirect: 'conversationDetails'
            },
            {
                path: 'conversationDetails',
                component: conversations
            },
            {
              path:'send',
              component: startConversations
            }
            // {
                // path: 'conversationDetail/:id',
                // component: conversationMessages,
                // props: (route) => ({
                //         id: route.params.id
                // })
                
            // }
        ]
    },
    {
        path: '/reports',
        name: 'reports',
        component: studyRoom,
        children: [
            {
                path: '',
                component: studyRoomSession
            }
            ]
    },

     {
      path: '/dev',
      name: 'dev',
      component: dev,
      children: [
        {
          path: '',
          redirect: 'change-country'
        },
        {
          path:'change-country',
          component: uChangeCountry
        },
        {
          path:'delete-user',
          component: uDelete
        },
        {
          path: '*',
          redirect: 'change-country'
        }
      ]
    },
    {
         path: '/*',
         redirect: '/home'
       }
];

  
