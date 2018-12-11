import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import userRank from "../../../helpers/UserRank/UserRank.vue";
export default {
  data() {
    return {
      actions: [{ title: "Report this item" }],
      maximumAnswersToDisplay: 3,
      isRtl: global.isRtl
    };
  },
  props: {
    cardData: {
      required: true
    }
  },
  components: {
    userAvatar,
    userRank
  },
  computed: {
    uploadDate() {
      if (this.cardData && this.cardData.dateTime) {
        return this.$options.filters.fullMonthDate(this.cardData.dateTime);
      } else {
        return "";
      }
    },
    isSold() {
      return this.cardData.hasCorrectAnswer;
    },
    randomRank() {
      return Math.floor(Math.random() * 3);
    },
    answersNumber() {
      let answersNum = this.cardData.answers;
      if (answersNum > this.maximumAnswersToDisplay) {
        return this.maximumAnswersToDisplay;
      }
      return answersNum;
    },
    answersDeltaNumber() {
      let answersNum = this.cardData.answers;
      let delta = 0;
      if (answersNum > this.maximumAnswersToDisplay) {
        delta = answersNum - this.maximumAnswersToDisplay;
      }
      return delta;
    },
    randomViews() {
      return Math.floor(Math.random() * 1001);
    },
    questionReputation() {
      return Math.floor(Math.random() * 100);
    }
  }
};