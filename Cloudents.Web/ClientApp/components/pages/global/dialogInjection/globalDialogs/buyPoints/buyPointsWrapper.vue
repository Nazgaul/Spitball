<template>
    <v-dialog :value="true" persistent max-width="840px" :content-class="selectedBuyPointsClass">
       <component :is="selectedBuyPointsComponent"></component>
    </v-dialog>
</template>

<script>
import paymentService from '../../../../../../services/payment/paymentService.js';
import buyPointsFrymo from '../../../../../dialogs/buyTokenFrymo/buyTokenFrymo.vue';
import buyPoints from '../../../../../dialogs/buyTokens/buyTokens.vue';

export default {
   name:'buyPointsWrapper',
   components:{
      buyPointsFrymo,
      buyPoints
   },
   computed: {
      selectedBuyPointsComponent(){
         return paymentService.getBuyPointsComponent()
      },
      selectedBuyPointsClass(){
         return !this.$store.getters.isFrymo ? 'buy-tokens-popup' : 'buy-tokens-frymo-popup';
      }
   },

}
</script>