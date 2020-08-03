
import Vue from "vue";
import Vuex from "vuex";
import axios from 'axios';

import User from "./User";
import Account from "./account";
import Toaster from "./Toaster";
import uploadFiles from  "./uploadFiles";
import mobileFooter from './mobileFooter';
import chatStore from './chatStore';
import userOnlineStatus from './userOnlineStatus';
import leaveReview from './leaveReview';
import requestTutor from './requestTutor';
import signalRStore from './signalRStore';
import routeStore from './routeStore';
import calendarStore from './calendarStore'
import gapiStore from './gapiStore';
import paymetStore from './paymetStore.js'
import homePage_store from './homePage_store.js'
import dialog_Store from './dialogStore/dialog_Store.js'
import utils_Store from './utils_Store'
import banner_Store from './banner_Store.js'
import dashboard_Store from './dashboard_Store.js'
import tutorDashboard from './tutorDashboard.js'
import profile from './profile.js';
import document from "./document.js";
import courseStore from './courseStore.js';

import studyRoomStore from './studyRoomStore/studyRoomStore.js'
import signalRPlugin from '../plugins/signalRPlugin';
import componentPlugin from '../plugins/componentPlugin';
import twilioPlugin from '../plugins/twilioPlugin';
import twilioStore from './studyRoomStore/twilioStore.js'
import codeEditor_store from './studyRoomStore/codeEditor_store.js'
import tutoringCanvas from './studyRoomStore/tutoringCanvas.js'
import roomRecording_store from './studyRoomStore/roomRecording_store.js'
const plugins = [
    signalRPlugin({hubPath:'/sbhub'}), 
    componentPlugin(),
    twilioPlugin(),

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
       // Question,
        Toaster,
        uploadFiles,
        document,
        mobileFooter,
        chatStore,
        leaveReview,
        userOnlineStatus,
        requestTutor,
        signalRStore,
        routeStore,
        calendarStore,
        gapiStore,
        paymetStore,
        homePage_store,
        dialog_Store,
        utils_Store,
        banner_Store,
        dashboard_Store,
        tutorDashboard,
        profile,
        studyRoomStore,
        twilioStore,
        codeEditor_store,
        tutoringCanvas,
        roomRecording_store,
        courseStore,
    },
    plugins,
});

store.$axios = axios.create({
        baseURL: '/api'
    })

store.$axios.interceptors.response.use(
    response => response,
    error => {
        if(error.response.status === 401){
            global.location = '/?authDialog=register';
        } else if(error.response.status === 404){
            let type = error.response.config.method;
            let url = error.response.config.url;
            global.location = `/error/notfound?client=true&type=${encodeURIComponent(type)}&url=${encodeURIComponent(url)}`;
        } else{
            return Promise.reject(error);
        }
    }
);

export default store;
