
import Vue from "vue";
import Vuex from "vuex";

import Search from "./search";
import User from "./User";
import Question from "./question";
import LuisData from "./luisData";
import Account from "./account";
import Toaster from "./Toaster";
import MarketingCampaign from './marketingCampaigns'
import loginDialog from './loginDialog'
import newQuestionDialog from './newQuestionDialog'
import document from './document'
import notification from "./notification";
import uploadFiles from  "./uploadFiles";
import University from "./university";
import homeLanding from "./homeLanding"
import homeworkHelpStore from './homeworkHelp_store'
import studyDocumentsStore from './studyDocuments_store';
import tutorsStore from './tutors_store';
import marketingBox from './marketingBox'
//import leaderBoard from './leaderBoard'
import mobileFooter from './mobileFooter';
//import onBoardGuide from './onBoardGuide';
import buyTokens from './buyTokens';
import chatStore from './chatStore';
import becomeTutor from './becomeTutor';
import studyRoomsStore from './studyRoomsStore';
import tutorList from './tutorList';
import userOnlineStatus from './userOnlineStatus';
import leaveReview from './leaveReview';
import requestTutor from './requestTutor';
import signalRStore from './signalRStore';
import studyRoomStore from './studyRoomStore/index';
import loginRegister from './loginRegister';
import routeStore from './routeStore';
import codeEditor from './codeEditor_store'


Vue.use(Vuex);
const store = new Vuex.Store({
    modules: {
        Search,
        User,
        LuisData,
        Account,
        Question,
        Toaster,
        MarketingCampaign,
        loginDialog,
        newQuestionDialog,
        notification,
        University,
        uploadFiles,
        document,
        homeLanding,
        homeworkHelpStore,
        studyDocumentsStore,
        tutorsStore,
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
        ...studyRoomStore,
        loginRegister,
        routeStore,
        codeEditor
    }
});

export default store;
