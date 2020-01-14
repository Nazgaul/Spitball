const state = {
    showSchoolBlock: global.innerWidth > 1260 ? true : false,
    showSchoolBlockMobile: false,
};

const getters = {
    getShowSchoolBlock: (state) => {
        let isMobile = global.innerWidth < 600;
        if(isMobile){
            return state.showSchoolBlock && state.showSchoolBlockMobile;
        }else{
            return state.showSchoolBlock;
        }
       
    },
};

const mutations = {
    updtaeShowSchoolBlock(state, val) {
        state.showSchoolBlock = val;
    },
    setShowSchoolBlockMobile(state, val) {
        state.showSchoolBlockMobile = val;
    },
};

const actions = {
    toggleShowSchoolBlock({commit, state}, val) {
        let isMobile = global.innerWidth < 600;
        if(typeof val !== "undefined") {
            commit('updtaeShowSchoolBlock', val);
        } else {
            if(isMobile){
                commit('setShowSchoolBlockMobile', !state.showSchoolBlock);
            }
            commit('updtaeShowSchoolBlock', !state.showSchoolBlock);
        }
    },
    setShowSchoolBlockMobile({commit}, val){
        commit('setShowSchoolBlockMobile', val);
    }
};

export default {
    state,
    mutations,
    getters,
    actions
};