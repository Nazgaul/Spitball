import reviewService from '../services/reviewService';

const state = {
    review: {
        rate: 0,
        reviewTExt: '',
        roomId: '',
    },
    reviewDialog: false
};
const getters = {
    getReviewDialogState: state => state.reviewDialog,
    getReview: state => state.review
};

const mutations = {
    changeReviewDialogState(state, val) {
        state.reviewDialog = val;
    },
    setReview(state, val) {
        state.review = val;
    }
};

const actions = {
    updateReviewDialog({commit, state}, val) {
        commit('changeReviewDialogState', val);
    },
    updateReview({commit, state}, val) {
        commit('setReview', val);
    },
    submitReview({commit, state}) {
        return  reviewService.sendReview(state.review)
                              .then((resp) => {
                                        return resp;
                                    },
                                    (error) => {
                                        console.log('errorsend review', error);
                                    });
    }
};
export default {
    state,
    mutations,
    getters,
    actions
};