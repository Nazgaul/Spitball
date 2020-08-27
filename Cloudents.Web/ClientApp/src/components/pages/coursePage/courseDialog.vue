<template>
  <v-dialog :value="true" content-class="coursePayment" :fullscreen="$vuetify.breakpoint.xsOnly" persistent max-width="500px" transition="dialog-transition" scrollable>
    <div class="text-center">
        <div class="text-end ma-2">
            <v-icon size="14" @click="closeDialog">{{$vuetify.icons.values.close}}</v-icon>
        </div>
        <bdi>
        <div class="coursePaymeTitle mt-6" v-t="'course_payment_title'"></div>
        <div class="coursePaymeSubTitle mt-6 px-12" v-t="'course_payment_subTitle'"></div>
        </bdi>
        <div class="coursePaymentActions mb-4">
            <v-btn class="coursePaymentBtnConnect white--text" tag="a" href="https://spitball.payme.co.il/onboarding/5d4c0b6d0aee19001ceec9bb" width="150" height="40" depressed rounded color="#4452fc">
                <span v-t="'course_payment_btn_connect'" /> 
            </v-btn>
            <v-btn @click="closeDialog" class="coursePaymentBtnBack text" color="white" width="150" height="40" depressed rounded>
                <span v-t="'courseBtn_cancel'"/>
            </v-btn>
          <div class="coursePaymentHelp mt-8" v-t="'course_payment_detail'"></div>
        </div>
    </div>
  </v-dialog>
</template>

<script>
import {COURSE_PAYMENT_DIALOG} from '../../pages/global/toasterInjection/componentConsts'

export default {
  computed: {
    isMobile(){
      return this.$vuetify.breakpoint.xsOnly;
    }
  },
  methods: {
    closeDialog() {
        this.$store.commit('removeComponent', COURSE_PAYMENT_DIALOG)
    }
  }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';

.coursePayment {
    background: #fff;
    color: @global-purple;
    .coursePaymeTitle {
      font-size: 20px;
      font-weight: bold;
    }
    .coursePaymeSubTitle {
      font-size: 16px;
      line-height: 1.75;
    }
    .coursePaymentActions {
      margin-top: 150px;

      .coursePaymentBtnConnect {
        font-weight: 600;
      }
      .coursePaymentBtnBack {
          margin: 6px 8px;
          color: @global-blue;
          font-weight: 600;
          border: 1px solid @global-blue !important;
      }
    }
    .coursePaymentHelp {
      font-size: 12px;
    }
}
</style>