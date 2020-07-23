//import Home from './components/Home.vue';
import user from './components/user/user.vue';
import uToken from './components/user/token/tokenUser.vue';
import uCashout from './components/user/cashout/cashoutUser.vue';
import uSuspend from './components/user/suspend/suspendUser.vue';
import activeUsers from './components/user/activeUsers/activeUsers.vue';
import payments from './components/user/payments/PaymentUser.vue';
import changeCountry from './components/user/changeCountry/changeCountry.vue';
import deleteUser from './components/user/delete/delete.vue';

import document from './components/document/document.vue';
import approveDelete from './components/document/documentComponents/approveDelete/approveDelete.vue';
import documentDelete from './components/document/documentComponents/documentDelete/documentDelete.vue';
import flaggedDocument from './components/document/documentComponents/flaggedDocument/flaggedDocument.vue';



import userMain from './components/userMainView/userMainView.vue';
import userDocuments from './components/userMainView/userDocuments/userDocuments.vue';
import userPurchasedDocuments from './components/userMainView/userPurchasedDocuments/userPurchasedDocuments.vue';
import userSessions from './components/userMainView/userSessions/userSessions.vue';
import userSoldItems from './components/userMainView/userSoldItems/userSoldItems.vue';
import userNotes from './components/userMainView/userNotes/userNotes.vue';

import management from './components/management/Management.vue';
//import coursesPending from './components/management/coursesPending/coursesPending.vue';
import shortUrl from './components/management/shortUrl/shortUrl.vue';

import conversation from './components/conversation/conversation.vue';
import conversations from './components/conversation/conversationComponent/conversationDetalis/conversationDetails.vue';
import startConversations from './components/conversation/startConversation.vue';

import tutors from './components/tutor/tutor.vue';
import pendingTutors from './components/tutor/pendingTutors/pendingTutor.vue';
import deleteTutors from './components/tutor/tutorDelete/tutorDelete.vue';
import studyRoom from './components/studyRoom/studyRoom.vue';
import studyRoomSession from './components/studyRoom/studyRoomComponents/sessions/studyRoomsSessions.vue';

import leads from './components/leads/leads.vue';
import coupon from './components/coupon/coupon.vue';
//import subjects from './components/subjects/subjects.vue';
import upload from './components/upload/upload.vue';

import tutorList from './components/tutor/tutorList/tutorList.vue';

export const routes = [
    {
       path: '/home/:userId?',
       name: 'userMainView',
       component: userMain,
       props: true,
        children: [
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
                name:'userSoldItems',
                path:'userSoldItems',
                component: userSoldItems
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
            },
            {
              name: 'userNotes',
              path: 'userNotes',
              component: userNotes
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
            },
            {
              path: 'deleteTutors',
              component: deleteTutors
            },
            {
              path: 'tutorList',
              component: tutorList
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
            path: 'payments',
            component: payments
          },
          {
            path: 'change-country',
            component: changeCountry
          },
          {
            path: 'delete',
            component: deleteUser
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
                redirect: 'shortUrl'
            },
            // {
            //     path: 'coursesPending',
            //     component: coursesPending
            // },
            {
              path: 'shortUrl',
              component: shortUrl
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
      path: '/leads',
      name: 'leads',
      component: leads
    },
    {
      path: '/coupon',
      name: 'coupon',
      component: coupon
    },
    // {
    //   path: '/subjects',
    //   name: 'subjects',
    //   component: subjects
    // },
    {
      path: '/upload',
      name: 'upload',
      component: upload
    },
    
    {
         path: '/*',
         redirect: '/home'
    }
];

  
