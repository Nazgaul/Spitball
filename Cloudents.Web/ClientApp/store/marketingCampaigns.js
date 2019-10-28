import marketingService from '../services/marketing/marketingService'

const state = {
    campaignName: "noCampaign",
    campaignData: marketingService.getCampaignData("noCampaign")
};
const mutations = {
    setCampaign(state, data) {
        state.campaignName = data.campaignName;
        state.campaignData = data.campaignData;
    },
};
const getters = {
    getCampaignName: (state) => state.campaignName,
    getCampaignData: (state) => {
        return state.campaignData;
    },
    isCampaignOn: (state) => {
        return state.campaignName !== "noCampaign";
    }
};
const actions = {
    updateCampaign({commit}, name) {
        let markObj = {
            campaignName: name,
            campaignData: marketingService.getCampaignData(name)
        };
        commit('setCampaign', markObj);
    },
};
export default {
    actions, state, mutations, getters
};