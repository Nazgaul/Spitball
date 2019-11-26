<template>
  <section>
      <!-- LOGIN -->
    <sb-dialog :showDialog="getLoginDialog"
               :popUpType="'loginPop'"
               :content-class="'login-popup'"
               :max-width="'550px'">
        <loginToAnswer/>
    </sb-dialog>
    <!--ADD QUESTION -->
    <sb-dialog :isPersistent="true"
                :showDialog="newQuestionDialogSate"
                :popUpType="'newQuestion'"
                :max-width="'510px'"
                :content-class="'question-request-dialog'">
        <addQuestion/>
    </sb-dialog>
    <!-- TUTOR REQUEST-->
    <sb-dialog :isPersistent="true"
                :showDialog="getRequestTutorDialog"
                :popUpType="'tutorRequestDialog'"
                :max-width="'510px'"
                :content-class="'tutor-request-dialog'">
        <tutorRequest/>
    </sb-dialog>
    <!-- UPLOAD -->
    <sb-dialog :showDialog="getDialogState"
                :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
                :popUpType="'uploadDialog'"
                :maxWidth="'716'"
                :onclosefn="setUploadDialogState"
                :activateOverlay="false"
                :isPersistent="$vuetify.breakpoint.smAndUp"
                :content-class="'upload-dialog'">
        <uploadMultipleFiles v-if="getDialogState"/>
    </sb-dialog>
    <!-- BECOME TUTOR -->
    <sb-dialog :showDialog="becomeTutorDialog"
                :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
                :popUpType="'becomeTutorDialog'"
                :maxWidth="'840'"
                :maxHeight="'588'"
                :onclosefn="setUploadDialogState"
                :activateOverlay="false"
                :isPersistent="$vuetify.breakpoint.smAndUp"
                :content-class="'become-tutor'">
        <becomeTutor v-if="becomeTutorDialog"/>
    </sb-dialog>
    <!-- PAYMENT -->
    <sb-dialog :isPersistent="true"
               :showDialog="getShowPaymeDialog"
               :popUpType="'payme'"
               :content-class="'payme-popup'"
               maxWidth='840px'>
        <paymentDialog/>
    </sb-dialog>
    <!-- BUY TOKENS -->
    <sb-dialog :showDialog="getBuyTokensDialog"
                :popUpType="'buyTokens'"
                :content-class="!isFrymo ? 'buy-tokens-popup' : 'buy-tokens-frymo-popup'"
                :onclosefn="closeSblToken"
                maxWidth='840px'>
        <buyTokens v-if="!isFrymo" popUpType="buyTokens"/>
        <buyTokenFrymo v-else popUpType="buyTokensFrymo"/>
    </sb-dialog>
    <!-- REFERRAL -->
    <sb-dialog v-if="!!accountUser"
               :showDialog="getReferralDialog"
               :popUpType="'referralPop'"
               :onclosefn="closeReferralDialog"
               :content-class="'login-popup'">
        <referralDialog
            :isTransparent="true"
            :onclosefn="closeReferralDialog"
            :showDialog="getReferralDialog"
            :popUpType="'referralPop'"/>
    </sb-dialog>
  </section>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";

import loginToAnswer from "../../../question/helpers/loginToAnswer/login-answer.vue";
import addQuestion from "../../../question/askQuestion/askQuestion.vue";
import tutorRequest from '../../../tutorRequestNEW/tutorRequest.vue';
import uploadMultipleFiles from '../../../uploadFilesDialog/uploadMultipleFiles.vue';
import becomeTutor from "../../../becomeTutor/becomeTutor.vue";
import paymentDialog from '../../../tutor/tutorHelpers/paymentDIalog/paymentDIalog.vue';
import buyTokens from "../../../dialogs/buyTokens/buyTokens.vue";
import buyTokenFrymo from "../../../dialogs/buyTokenFrymo/buyTokenFrymo.vue";
import referralDialog from "../../../question/helpers/referralDialog/referral-dialog.vue";

export default {
    components:{
        sbDialog,
        loginToAnswer,
        addQuestion,
        tutorRequest,
        uploadMultipleFiles,
        becomeTutor,
        paymentDialog,
        buyTokens,
        buyTokenFrymo,
        referralDialog
    },
    computed: {
       ...mapGetters([
           'getLoginDialog',
           "newQuestionDialogSate",
           'getRequestTutorDialog',
           'getDialogState',
           'becomeTutorDialog',
           'getShowPaymeDialog',
           'getBuyTokensDialog',
           'isFrymo',
           'accountUser',
           'getReferralDialog']), 
    },
    methods: {
        ...mapActions(['updateDialogState','updateBuyTokensDialog','updateReferralDialog','changeSelectPopUpUniState']),
        setUploadDialogState() {
            this.updateDialogState(false);
        },
        closeSblToken() {
            this.updateBuyTokensDialog(false);
        },
        closeReferralDialog() {
            this.updateReferralDialog(false)
        },
        closeUniPopDialog() {
            this.changeSelectPopUpUniState(false);
        },
    },

}
</script>

<style lang="less">

</style>