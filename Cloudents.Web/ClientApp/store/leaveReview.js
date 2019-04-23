import reviewService from '../services/reviewService';

const state = {
    review: {
        rate: 0,
        reviewText: '',
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
    setReview(state, reviewData) {
        state.review = reviewData;
    },
    setReviewStars(state, val) {
        state.review.rate = val;
    }
};

const actions = {
    updateReviewDialog({commit, state}, val) {
        commit('changeReviewDialogState', val);
    },
    updateReviewStars({commit, state}, val) {
        commit('setReviewStars', val);
    },
    updateReview({commit, state}, reviewData) {
        commit('setReview', reviewData);
    },
    submitReview({commit, state}, reviewData) {
        commit('setReview', reviewData);
        return reviewService.sendReview(reviewData)
                            .then((resp) => {
                                      return resp;
                                  },
                                  (error) => {
                                      console.log('errorsend review', error);
                                        return error
                                  });
    }
};
export default {
    state,
    mutations,
    getters,
    actions
};