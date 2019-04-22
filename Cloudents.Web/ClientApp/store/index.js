
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
import marketingBox from './marketingBox'
import leaderBoard from './leaderBoard'
import mobileFooter from './mobileFooter';
import onBoardGuide from './onBoardGuide';
import buyTokens from './buyTokens';
import tutoringCanvas from './tutoringCanvas';
import tutoringMainStore from './tutoringMain';
import chatStore from './chatStore';
import becomeTutor from './becomeTutor';
import studyRoomsStore from './studyRoomsStore';
import tutorList from './tutorList';




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
        marketingBox,
        leaderBoard,
        mobileFooter,
        onBoardGuide,
        buyTokens,
        tutoringCanvas,
        tutoringMainStore,
        chatStore,
        becomeTutor,
        studyRoomsStore,

        tutorList
    }
});

export default store;
