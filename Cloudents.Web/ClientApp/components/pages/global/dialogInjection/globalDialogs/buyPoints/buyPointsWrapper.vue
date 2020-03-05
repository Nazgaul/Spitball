<template>
    <v-dialog :value="true" persistent max-width="840px" :content-class="selectedBuyPointsClass" :fullscreen="$vuetify.breakpoint.xsOnly">
       <component :is="selectedBuyPointsComponent"></component>
    </v-dialog>
</template>

<script>
import paymentService from '../../../../../../services/payment/paymentService.js';
const buyPointsFrymo = () => import('./components/buyTokenFrymo.vue');
const buyPointsIL = () => import('./components/buyPointsIL.vue');
const buyPointsUS = () => import('./components/buyPointsUS.vue');

export default {
   name:'buyPointsWrapper',
   components:{
      buyPointsFrymo,
      buyPointsIL,
      buyPointsUS
   },
   computed: {
      selectedBuyPointsComponent(){
         return paymentService.getBuyPointsComponent()
      },
      //TEMP
      selectedBuyPointsClass(){
         return !this.$store.getters.isFrymo ? 'buy-tokens-popup' : 'buy-tokens-frymo-popup';
      }
   },
   created() {
      this.$store.dispatch('updateIsBuyPoints',true);
   },
   beforeDestroy() {
      this.$store.dispatch('updateIsBuyPoints',false);
   },

}
</script>
<style lang="less">
.buy-tokens-frymo-popup{
   background-color: white;
}
</style>