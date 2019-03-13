
import Vue from "vue";
import Vuex from "vuex";

import Search from "./search";
import User from "./User";
import Question from "./question";
import SpitballPreview from "./SpitballPreview";
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
import tutoringChat from './tutoringChat';
import tutoringCanvas from './tutoringCanvas';
import tutoringSharedDoc from './tutoringSharedDocument';
import tutoringMainStore from './tutoringMain';




Vue.use(Vuex);
const store = new Vuex.Store({
    modules: {
        Search,
        User,
        SpitballPreview,
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
        tutoringChat,
        tutoringCanvas,
        tutoringSharedDoc,
        tutoringMainStore
    }
});

export default store;
