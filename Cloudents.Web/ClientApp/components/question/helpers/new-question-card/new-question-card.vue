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
          <div class="sold-area" v-if="isSold">sold</div>
        </div>
        <div class="question-header-small-sagment">
          <div class="price-area">{{cardData.price.toFixed(2)}} <span>SBL</span></div>
        </div>
      </div>
      <div class="question-body-container">
        <div class="question-left-body-container"></div>
        <div class="question-right-body-container"></div>
      </div>
      <div class="question-footer-container"></div>
      <h1>{{cardData}}</h1>
    </router-link>
  </div>
</template>

<script>
import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import userRank from "../../../helpers/UserRank/UserRank.vue";
export default {
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
    }
    .question-header-small-sagment{
        .price-area{
            color: @color-blue-new;
            font-family: @fontOpenSans;
            font-size: 16px;
            font-weight: 600;
            span{
                font-size: 10px;
            }
        }
    }
  }
  .question-body-container {
    display: flex;
    .question-left-body-container {
      display: flex;
    }
    .question-right-body-container {
      display: flex;
    }
  }
  .question-footer-container {
    display: flex;
  }
}
</style>
