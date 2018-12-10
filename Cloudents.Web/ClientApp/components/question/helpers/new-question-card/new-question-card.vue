<template>
  <div class="question-container">
    <router-link :to="{path:'/question/'+cardData.id}">
      <div class="question-header-container">
        <div class="question-header-large-sagment">
          <div class="avatar-area">
            <user-avatar :user-name="cardData.user.name" :user-id="cardData.user.id"/>
          </div>
          <div class="rank-area">
            <user-rank :rank="randomRank"></user-rank>
          </div>
          <div class="date-area">{{uploadDate}}</div>
          <div class="sold-area" v-if="isSold">
            <div class="sold-container">
              <span>SOLD</span>
              <v-icon>sbf-curved-arrow</v-icon>
            </div>
          </div>
        </div>
        <div class="question-header-small-sagment">
          <div class="price-area" :class="{'sold': isSold}">
            {{cardData.price.toFixed(2)}}
            <span>SBL</span>
          </div>
          <div class="menu-area">
            <v-menu bottom left>
              <v-btn :depressed="true" @click.prevent slot="activator" icon>
                <v-icon>sbf-3-dot</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile v-for="(action, i) in actions" :key="i" @click>
                  <v-list-tile-title>{{ action.title }}</v-list-tile-title>
                </v-list-tile>
              </v-list>
            </v-menu>
          </div>
        </div>
      </div>
      <div class="question-body-container">
        <div class="question-left-body-container">
          <span class="question-raputation upvote-arrow">
            <v-icon>sbf-arrow-right</v-icon>
          </span>
          <span class="question-raputation question-score">{{questionReputation}}</span>
          <span class="question-raputation downvote-arrow">
            <v-icon>sbf-arrow-right</v-icon>
          </span>
        </div>
        <div class="question-right-body-container">
          <div class="question-body-header-container">
            <span class="question-subject">{{cardData.subject}}</span>
          </div>
          <div
            class="question-body-content-container"
            :class="[`align-switch-${cardData.isRtl ? isRtl ? 'l' : 'r' : isRtl ? 'r' : 'l'}`]"
          >
            <span>{{cardData.text}}</span>
          </div>
        </div>
      </div>
      <div class="question-footer-container">
        <div class="answer-display-container">
          <div>
            <v-icon
              v-for="(answer, index) in answersNumber"
              :key="index"
              class="answer-icon"
            >sbf-comment-icon</v-icon>
          </div>
          <div v-if="answersDeltaNumber > 0" class="answers-delta-number-container">
            <span>{{answersDeltaNumber}}</span>
          </div>
          <div v-if="cardData.watchingNow > 0" class="answer-currently-watching-container" :class="{'space': answersNumber > 0}">
            <span>Someone is answering now…</span>
          </div>
        </div>
        <div class="answers-info-container">
          <div v-if="cardData.filesNum > 0" class="answers-attachments-container">
            <v-icon>sbf-attach</v-icon>
            <span>{{cardData.filesNum}}</span>
          </div>
          <div class="answers-viewers-container">
            <v-icon class>sbf-views</v-icon>
            <span>{{randomViews}}</span>
          </div>
        </div>
      </div>
      <div v-if="!isSold" class="question-bottom-section">
        <div class="question-input-container">
          <input class="question-input" placeholder="Answer..." type="text">
        </div>
      </div>
    </router-link>
  </div>
</template>

<script>
import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import userRank from "../../../helpers/UserRank/UserRank.vue";
export default {
  data() {
    return {
      actions: [{ title: "Flag" }],
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
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.question-container {
  width: 500px;
  padding: 16px;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
  .question-header-container {
    display: flex;
    flex-direction: row;
    line-height: 35px;
    white-space: nowrap;
    .question-header-large-sagment {
      display: flex;
      width: 90%;
      .rank-area {
        margin: 0 12px;
      }
      .date-area {
        font-family: @fontOpenSans;
        font-size: 13px;
        width: 96px;
        color: rgba(0, 0, 0, 0.38);
        vertical-align: middle;
      }
      .sold-area {
        .sold-container {
          width: 65px;
          display: flex;
          margin-left: -10px;
          background-color: #2bcea9;
          border-radius: 15px;
          height: 15px;
          margin-top: 10px;
          line-height: 15px;
          justify-content: space-evenly;
          span {
            font-family: @fontFiraSans;
            font-size: 11px;
            font-weight: 500;
            font-style: italic;
            color: #ffffff;
          }
          i {
            color: #fff;
            font-size: 11px;
          }
        }
      }
    }
    .question-header-small-sagment {
      display: flex;
      .price-area {
        color: @color-blue-new;
        font-family: @fontOpenSans;
        font-size: 16px;
        font-weight: 600;
        &.sold {
          color: rgba(0, 0, 0, 0.38);
        }
        span {
          font-size: 10px;
        }
      }
      .menu-area {
        margin-top: -10px;
        width: 30px;
        i {
          font-size: 20px;
          color: rgba(0, 0, 0, 0.25);
        }
      }
    }
  }
  .question-body-container {
    display: flex;
    padding: 8px;
    .question-left-body-container {
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      height: 90px;
      padding-top: 3px;
      .question-raputation {
        font-size: 15px;
        font-weight: 600;
        font-family: @fontOpenSans;
        font-weight: bold;
        text-align: center;
      }
      .upvote-arrow {
        transform: rotate(270deg);
        margin-right: 4px;
        i {
          color: @color-blue-new;
          font-size: 15px;
        }
      }
      .question-score {
        color: @color-blue-new;
      }
      .downvote-arrow {
        transform: rotate(90deg);
        margin-right: -3px;
        i {
          color: @color-blue-new;
          font-size: 15px;
        }
      }
    }
    .question-right-body-container {
      display: flex;
      flex-direction: column;
      margin-left: 21px;
      padding-right: 10px;
      word-break: break-word;
      padding-top: 3px;
      width: 100%;
      .question-body-header-container {
        .question-subject {
          color: rgba(0, 0, 0, 0.87);
          font-weight: bold;
          font-family: @fontOpenSans;
        }
      }
      .question-body-content-container {
        margin-top: 12px;
        opacity: 0.9;
        font-family: @fontOpenSans;
        line-height: 1.6;
        letter-spacing: -0.3px;
        color: rgba(0, 0, 0, 0.87);
        &.align-switch-l {
          direction: rtl;
        }
        &.align-switch-r {
          direction: ltr;
        }
      }
    }
  }
  .question-footer-container {
    display: flex;
    margin-top: 5px;
    justify-content: space-between;
    line-height: 30px;
    padding-left: 2px;
    .answer-display-container {
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      .answers-delta-number-container {
        margin-left: 25px;
        font-family: @fontFiraSans;
        color: rgba(0, 0, 0, 0.54);
      }
      .answer-icon {
        margin-right: -15px;
        background-color: #e8e8e8;
        width: 30px;
        height: 30px;
        border-radius: 30px;
        opacity: 1;
        border: 2px solid #fff;
        font-size: 14px;
        float: right;
      }
      .answer-currently-watching-container{
        opacity: 0.9;
        font-family: @fontOpenSans;
        font-size: 11px;
        letter-spacing: -0.2px;
        color: @color-blue-new;
        &.space{
          margin-left:25px;
        }
      }
    }
    .answers-info-container {
      display: flex;
      flex-direction: row;
      .answers-attachments-container {
        color: rgba(0, 0, 0, 0.54);
        margin-right: 20px;
        i {
          font-size: 18px;
        }
      }
      .answers-viewers-container {
        i {
          color: rgba(0, 0, 0, 0.25);
          font-size: 18px;
        }
        span {
          opacity: 0.9;
          font-family: @fontFiraSans;
          color: rgba(0, 0, 0, 0.54);
        }
      }
    }
  }
  .question-bottom-section {
    ::-webkit-input-placeholder {
      /* Chrome/Opera/Safari */
      font-family: @fontOpenSans;
      font-size: 14px;
      font-weight: 600;
      line-height: 1;
      color: @color-blue-new;
    }
    ::-moz-placeholder {
      /* Firefox 19+ */
      font-family: @fontOpenSans;
      font-size: 14px;
      font-weight: 600;
      line-height: 1;
      color: @color-blue-new;
    }
    :-ms-input-placeholder {
      /* IE 10+ */
      font-family: @fontOpenSans;
      font-size: 14px;
      font-weight: 600;
      line-height: 1;
      color: @color-blue-new;
    }
    :-moz-placeholder {
      /* Firefox 18- */
      font-family: @fontOpenSans;
      font-size: 14px;
      font-weight: 600;
      line-height: 1;
      color: @color-blue-new;
    }
    .question-input-container {
      padding-top: 14px;
      border-top: 1px solid #e8e8e8;
      margin-top: 8px;
      .question-input {
        background-color: #f5f5f5;
        width: 100%;
        border-radius: 32px;
        height: 32px;
        border: 1px solid #ededed;
        outline: none;
        padding-left: 20px;
      }
    }
  }
}
</style>
