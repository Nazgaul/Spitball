import reviewService from '../services/reviewService';

const state = {
    review: {
        rate: 0,
        reviewText: '',
        roomId: '',
    },
    reviewDialog: false,
    allowReview: true
};
const getters = {
    getReviewDialogState: state => state.reviewDialog,
    getReview: state => state.review,
    getAllowReview: state => state.allowReview
};

const mutations = {
    changeReviewDialogState(state, val) {
        state.reviewDialog = val;
    },
    setReview(state, reviewData) {
        state.review = reviewData;
    },
    setReviewStars(state, val) {
        state.review.rate = val;
    },
    setAllowReview(state, val){
        state.allowReview = val;
    }

};

const actions = {
    updateAllowReview({commit}, val){
        commit('setAllowReview', val);
    },
    updateReviewDialog({commit}, val) {
        commit('changeReviewDialogState', val);
    },
    updateReviewStars({commit}, val) {
        commit('setReviewStars', val);
    },
    updateReview({commit}, reviewData) {
        commit('setReview', reviewData);
    },
    submitReview({commit}, reviewData) {
        commit('setReview', reviewData);
        return reviewService.sendReview(reviewData)
                            .then((resp) => {
                                commit('setAllowReview', false);
                                      return resp;
                                  },
                                  (error) => {
                                      console.log('errorsend review', error);
                                        return error;
                                  });
    }
};
export default {
    state,
    mutations,
    getters,
    actions
};