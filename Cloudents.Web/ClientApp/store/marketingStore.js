import dashboardService from '../services/dashboardService';
// const state = {
// };

// const getters = {
// };

// const mutations = {
// };

const actions = {
    getPromoteData() {
        return dashboardService.getContentItems()
    },
};

export default {
    // state,
    // getters,
    // mutations,
    actions
}