
import Vue from "vue";
import Vuex from "vuex";

import Search from "./search";
import User from "./User";
import Question from "./question";
import Account from "./account";
import Toaster from "./Toaster";
import MarketingCampaign from './marketingCampaigns'
import loginDialog from './loginDialog'
import newQuestionDialog from './newQuestionDialog'
import uploadFiles from  "./uploadFiles";
import University from "./university";
import marketingBox from './marketingBox'
import mobileFooter from './mobileFooter';
import buyTokens from './buyTokens';
import chatStore from './chatStore';
import becomeTutor from './becomeTutor';
import studyRoomsStore from './studyRoomsStore';
import tutorList from './tutorList';
import userOnlineStatus from './userOnlineStatus';
import leaveReview from './leaveReview';
import requestTutor from './requestTutor';
import signalRStore from './signalRStore';
import routeStore from './routeStore';
import calendarStore from './calendarStore'
import gapiStore from './gapiStore';
import paymetStore from './paymetStore.js'
import homePage_store from './homePage_store.js'


Vue.use(Vuex);
const store = new Vuex.Store({
    modules: {
        Search,
        User,
        // LuisData,
        Account,
        Question,
        Toaster,
        MarketingCampaign,
        loginDialog,
        newQuestionDialog,
        University,
        uploadFiles,
        // document,
        // homeLanding,
        // homeworkHelpStore,
        // studyDocumentsStore,
        marketingBox,
        //leaderBoard,
        mobileFooter,
        //onBoardGuide,
        buyTokens,
        chatStore,
        becomeTutor,
        studyRoomsStore,
        tutorList,
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
        homePage_store
    }
});

export default store;
