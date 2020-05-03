
import Vue from "vue";
import Vuex from "vuex";

import User from "./User";
import Question from "./question";
import Account from "./account";
import Toaster from "./Toaster";
import newQuestionDialog from './newQuestionDialog'
import uploadFiles from  "./uploadFiles";
import University from "./university";
import mobileFooter from './mobileFooter';
import chatStore from './chatStore';
import becomeTutor from './becomeTutor';
import userOnlineStatus from './userOnlineStatus';
import leaveReview from './leaveReview';
import requestTutor from './requestTutor';
import signalRStore from './signalRStore';
import routeStore from './routeStore';
import calendarStore from './calendarStore'
import gapiStore from './gapiStore';
import paymetStore from './paymetStore.js'
import homePage_store from './homePage_store.js'
import sideMenu_Store from './sideMenu_Store'
import dialog_Store from './dialogStore/dialog_Store.js'
import utils_Store from './utils_Store'
import banner_Store from './banner_Store.js'
import dashboard_Store from './dashboard_Store.js'
import feed from './feedStore.js'
import profile from './profile.js';

import studyRoomStore from './studyRoomStore/studyRoomStore.js'
import signalRPlugin from '../plugins/signalRPlugin';
import componentPlugin from '../plugins/componentPlugin';
import twilioPlugin from '../plugins/twilioPlugin';
import twilioStore from './studyRoomStore/twilioStore.js'
// import logger from 'vuex/dist/logger.js'
const plugins = [
    signalRPlugin({hubPath:'/sbhub'}), 
    componentPlugin(),
    twilioPlugin()
]

// if(process.env.NODE_ENV !== "production"){
//     plugins.push(logger())
// }
// const onModuleAValueChange= (store) => {
//     store.watch(
//         state => state.route,
//         (val, oldVal) => {
//             // Don't do anything on init state
//             if (!oldVal) return;
// console.log(val,oldVal);
//             // // This will trigger all refresh actions on all store. But you can add anything here
//             // // Essentially does: 
//             // // store.dispatch(`moduleA/refresh`); 
//             // // store.dispatch(`moduleB/refresh`); 
//             // // store.dispatch(`moduleC/refresh`);
//             // for (let state in store.state) {
//             //     const action = `${state}/refresh`;
//             //     // If the store does not have an refresh action ignore
//             //     if (store._actions[action]) store.dispatch(`${state}/refresh`);
//             // }

//             // // Additional action 
//             // store.dispatch(`moduleC/moduleC_Action`);
//         }
//     );
// };


Vue.use(Vuex);
const store = new Vuex.Store({
    modules: {
        User,
        Account,
        Question,
        Toaster,
        newQuestionDialog,
        University,
        uploadFiles,
        feed,
        // document,
        // homeLanding,
        // homeworkHelpStore,
        // studyDocumentsStore,
        //leaderBoard,
        mobileFooter,
        //onBoardGuide,
        chatStore,
        becomeTutor,
        leaveReview,
        userOnlineStatus,
        requestTutor,
        signalRStore,
        // ...studyRoomStore,
        // loginRegister,
        routeStore,
        calendarStore,
        gapiStore,
        paymetStore,
        homePage_store,
        sideMenu_Store,
        dialog_Store,
        utils_Store,
        banner_Store,
        dashboard_Store,
        profile,

        studyRoomStore,
        twilioStore,
    },
    plugins,
   // plugins: [onModuleAValueChange]
});
export default store;
