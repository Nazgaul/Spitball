import Vue from "vue";
import Vuex from "vuex";
//This is new vuex
//import Search from "./session";
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
//TODO: server side fix
import createPersistedState from "vuex-persistedstate"
import notification from "./notification";
import uploadFiles from  "./uploadFiles";
import University from "./university"

function canWriteStorage(storage) {
    try {
        storage.setItem('@@', 1);
        storage.removeItem('@@');
        return true;
    } catch (e) {
    }

    return false;
}

const storageFallback = [{
    provider: localStorage,
    enable: true,
},
    {
        provider: sessionStorage,
        enable: true,
    }]
for (let v in storageFallback) {
    let item = storageFallback[v];
    item.enable = canWriteStorage(item.provider);
}

let storage = storageFallback.find(f => f.enable);
const plugins = [];
if (storage) {
    plugins.push(createPersistedState(
        {
            paths: ["User"],
            storage: storage.provider
        }
    ));
}

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
        uploadFiles
    },
    //plugins: plugins
});
export default store;
