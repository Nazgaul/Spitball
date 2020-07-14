<template>
  <v-layout column class="payme-popup pa-3">
    <div class="text-center payme-top-title mb-5">{{$t('paymentUs title', [$store.getters.getRoomTutor.tutorName])}}</div>
    <div class="text-center payme-top-title">{{$t('seesion require payment', [$price(tutorPrice, tutorCurrency)])}}</div>
    <div class="stripeWrapper mt-5">
      <div class="text-center">
        <v-btn @click="stripePay" :loading="isLoading" class="white--text" width="160" color="#4c59ff" rounded depressed>
          <span class="payBtn" v-t="'pay'"></span>
        </v-btn>
      </div>
    </div>
    <stripe ref="stripe"></stripe>
  </v-layout>
</template>

<script>
// import * as routeNames from "../../../../../../../routes/routeNames";
// import * as componentConsts from '../../../../toasterInjection/componentConsts.js';
import stripe from '../../../../stripe.vue'
export default {
  name: "paymentUS",
  components: {
    stripe
  },
  data() {
    return {
      isLoading:false,
      stripe: null,
      // elements: null,
      // cardElement: null,
      // stripeError: '',
    }
  },
  computed: {
    tutorName() {
      return this.$store.getters.getRoomTutor?.tutorName
    },
    tutorPrice() {
      return this.$store.getters.getRoomTutor?.tutorPrice?.amount
    },
    tutorCurrency() {
      return this.$store.getters.getRoomTutor?.tutorPrice?.currency
    }
  },
  methods: {
    // closeDialog() {
    //   let isStudyRoom = this.$store.getters.getRoomIdSession && this.$route.name === routeNames.StudyRoom;
    //   if (isStudyRoom) {
    //     let isExit = confirm(this.$t("payme_are_you_sure_exit"));
    //     if (isExit) {
    //       this.$router.push("/");
    //       this.$store.commit('removeComponent',componentConsts.PAYMENT_DIALOG)
    //     }
    //   } else {
    //     this.$store.commit('removeComponent',componentConsts.PAYMENT_DIALOG)
    //   }
    // },
    async stripePay() {
      let sessionObj = {
        userId: this.$store.getters.getAccountId,
        studyRoomId: this.$store.getters.getRoomIdSession
      }

      let x = await this.$store.dispatch('updateStudyroomLiveSessionsWithPrice', sessionObj);
      this.$refs.stripe.redirectToStripe(x);
    }
  }
};
</script>

<style lang="less">
@import "../../../../../../../styles/mixin.less";
@import "../../../../../../../styles/colors.less";

.payme-popup {
  position: relative;
  border-radius: 4px;
  background-color: #ffffff;
  -webkit-overflow-scrolling: touch;

  .payme-top-title {
    font-size: 18px;
    font-weight: 600;
    color: @global-purple;
    @media (max-width: @screen-xs) {
      font-size: 18px;
    }
  }
  .payBtn {
    font-weight: 600;
  }
  // .exit-btn {
  //   position: absolute;
  //   top: 16px;
  //   right: 16px;
  //   font-size: 12px !important;
  //   color: rgba(0, 0, 0, 0.541);
  // }
  // .payme-popup-top {
  //   text-align: center;
  //   .v-skeleton-loader__button {
  //     width: inherit;
  //     height: inherit;
  //   }

  //   .payme-top-title {
  //     font-size: 20px;
  //     font-weight: bold;
  //     color: @global-purple;
  //     @media (max-width: @screen-xs) {
  //       font-size: 18px;
  //     }
  //   }
  //   .payme-top-desc {
  //     font-size: 14px;
  //     font-weight: 600;
  //     line-height: 1.5;
  //     letter-spacing: -0.17px;
  //     color: @global-purple;
  //     @media (max-width: @screen-xs) {
  //       padding: 0 24px;
  //     }
  //   }
  //   .payme-content {
  //     @media (max-width: @screen-xs) {
  //       padding-left: 12px;
  //     }
  //     display: flex;
  //     justify-content: center;
  //     .payme-content-div {
  //       @media (max-width: @screen-xs) {
  //         flex-direction: row;
  //         padding-bottom: 12px;
  //       }
  //       .flexSameSize();
  //       display: flex;
  //       flex-direction: column;
  //       align-items: center;
  //       .payme-content-img {
  //         width: 32px;
  //         height: 32px;
  //         @media (max-width: @screen-xs) {
  //           width: 26px;
  //           height: 26px;
  //           margin-right: 12px;
  //         }
  //         object-fit: contain;
  //       }
  //       .payme-content-txt {
  //         font-size: 14px;
  //         font-weight: 600;
  //         line-height: 1.5;
  //         letter-spacing: -0.17px;
  //         color: @global-purple;
  //         white-space: pre-line;
  //       }
  //     }
  //   }
  // }

  // .payme-popup-bottom {
  //   @media (max-width: @screen-xs) {
  //     .flexSameSize();
  //     flex-direction: column-reverse;
  //     align-items: start;
  //   }
  //   display: flex;
  //   align-items: center;
  //   justify-content: space-between;
  //   background-color: #f0f0f7;
  //   padding: 16px 22px;
  //   p {
  //     line-height: 1.8;
  //     color: @global-purple;
  //     max-width: 83%;
  //     margin: 0;
  //     font-size: 14px;
  //     letter-spacing: 0.1px;
  //     @media (max-width: @screen-xs) {
  //       font-size: 12px;
  //       max-width: inherit;
  //     }
  //   }
  //   img {
  //     @media (max-width: @screen-xs) {
  //       height: 32px;
  //       margin-bottom: 12px;
  //       align-self: center;
  //     }
  //     height: 54px;
  //   }
  // }
  // #paypal-button-container2 {
  //     margin:0 auto;
  // }
  // .stripeWrapper {
    // #card-stripe {
    //   border: 1px solid #e5e5e5;
    //   border-radius: 6px;
    //   padding: 10px;
    // }
  // }
}
</style>