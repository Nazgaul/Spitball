<template>
  <v-layout column class="payme-popup">
    <v-icon class="exit-btn cursor-pointer" @click="closeDialog">{{$vuetify.icons.values.close}}</v-icon>
    <div class="payme-popup-top pt-4">
      <div class="payme-top-title">{{$t('payme_top_title')}}</div>
      <v-layout justify-space-between wrap class="payme-content pt-4 pt-sm-8 pb-2 pb-sm-8 mx-sm-8">
        <v-flex class="payme-content-div">
          <img class="payme-content-img" src="./images/timer.png" />
          <span class="payme-content-txt pt-0 pt-sm-2">{{$t('payme_content_txt_time')}}</span>
        </v-flex>
        <v-flex  class="payme-content-div mx-0 ">
          <img class="payme-content-img" src="./images/sheild.png" />
          <span
            class="payme-content-txt pt-0 pt-sm-2">{{$t('payme_content_txt_sheild')}}</span>
        </v-flex>
        <v-flex  class="payme-content-div">
          <img class="payme-content-img" src="./images/hands.png" />
          <span
            class="payme-content-txt pt-0 pt-sm-2">{{$t('payme_content_txt_hands')}}</span>
        </v-flex>
      </v-layout>
    </div>
    <div class="stripeWrapper mx-auto my-4">
      <div id="card-stripe"></div>
      <div class="text-center mt-8">
        <v-btn @click="stripePay" :loading="isLoading" class="white--text" width="120" color="#4c59ff" rounded depressed>pay</v-btn>
      </div>
    </div>
  </v-layout>
</template>

<script>
import * as routeNames from "../../../../../../../routes/routeNames";
export default {
  name: "paymentUS",
  data() {
    return {
      isLoading:false,
      stripe: null,
      elements: null,
      cardElement: null,
      stripeError: '',
    }
  },
  computed: {
    getStripeToken() {
      return this.$store.getters.getStripeToken
    }
  },
  methods: {
    closeDialog() {
      let isStudyRoom = this.$store.getters.getRoomIdSession && this.$route.name === routeNames.StudyRoom;
      if (isStudyRoom) {
        let isExit = confirm(this.$t("payme_are_you_sure_exit"));
        if (isExit) {
          this.$router.push("/");
        }
      } else {
        this.$closeDialog();
      }
    },
    stripePay() {
      let self = this
      this.isLoading = true
      this.$store.dispatch('getStripeSecret').then(({data}) => {
        self.stripe.confirmCardSetup(data.secret, {
          payment_method: {
            card: self.cardElement
          }
        }).then((result) => {
            if (result.error) {
              console.log(result.error.message);
              return
            }
            if (result.setupIntent.status === 'succeeded') {
              self.$store.dispatch('updateRoomIsNeedPayment',false)
            }
        }).finally(() => {
          self.isLoading = false
        })
      })
    }
  },
  mounted() {
    let self = this;
    this.$loadScript("https://js.stripe.com/v3/?advancedFraudSignals=false").then(() => {
      self.stripe = window.Stripe(this.getStripeToken);  
      self.elements = self.stripe.elements();
      self.cardElement = self.elements.create('card');
      self.cardElement.mount('#card-stripe');
    })
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
  .exit-btn {
    position: absolute;
    top: 16px;
    right: 16px;
    font-size: 12px !important;
    color: rgba(0, 0, 0, 0.541);
  }
  .payme-popup-top {
    text-align: center;
    .v-skeleton-loader__button {
      width: inherit;
      height: inherit;
    }

    .payme-top-title {
      font-size: 20px;
      font-weight: bold;
      color: @global-purple;
      @media (max-width: @screen-xs) {
        font-size: 18px;
      }
    }
    .payme-top-desc {
      font-size: 14px;
      font-weight: 600;
      line-height: 1.5;
      letter-spacing: -0.17px;
      color: @global-purple;
      @media (max-width: @screen-xs) {
        padding: 0 24px;
      }
    }
    .payme-content {
      @media (max-width: @screen-xs) {
        padding-left: 12px;
      }
      display: flex;
      justify-content: center;
      .payme-content-div {
        @media (max-width: @screen-xs) {
          flex-direction: row;
          padding-bottom: 12px;
        }
        .flexSameSize();
        display: flex;
        flex-direction: column;
        align-items: center;
        .payme-content-img {
          width: 32px;
          height: 32px;
          @media (max-width: @screen-xs) {
            width: 26px;
            height: 26px;
            margin-right: 12px;
          }
          object-fit: contain;
        }
        .payme-content-txt {
          font-size: 14px;
          font-weight: 600;
          line-height: 1.5;
          letter-spacing: -0.17px;
          color: @global-purple;
          white-space: pre-line;
        }
      }
    }
  }

  .payme-popup-bottom {
    @media (max-width: @screen-xs) {
      .flexSameSize();
      flex-direction: column-reverse;
      align-items: start;
    }
    display: flex;
    align-items: center;
    justify-content: space-between;
    background-color: #f0f0f7;
    padding: 16px 22px;
    p {
      line-height: 1.8;
      color: @global-purple;
      max-width: 83%;
      margin: 0;
      font-size: 14px;
      letter-spacing: 0.1px;
      @media (max-width: @screen-xs) {
        font-size: 12px;
        max-width: inherit;
      }
    }
    img {
      @media (max-width: @screen-xs) {
        height: 32px;
        margin-bottom: 12px;
        align-self: center;
      }
      height: 54px;
    }
  }
  #paypal-button-container2 {
      margin:0 auto;
  }
  .stripeWrapper {
    position: relative;
    width: 400px;
    #card-stripe {
      border: 1px solid #e5e5e5;
      border-radius: 6px;
      padding: 10px;
    }
  }
}
</style>