<template>
   <v-dialog :value="true" persistent :maxWidth="getMaxWith" :fullscreen="$vuetify.breakpoint.xsOnly">
      <component :is="selectedPaymentComponent"></component>
   </v-dialog>
</template>

<script>
import paymentService from '../../../../../../services/payment/paymentService.js';
const paymentIL = () => import('./components/paymentIL.vue');
const paymentUS = () => import('./components/paymentUS.vue');

export default {
   name: 'paymentWrapper',
   components:{
      paymentIL,
      paymentUS
   },
   computed: {
      selectedPaymentComponent(){
         return paymentService.getPaymentComponent()
      },
      getMaxWith() {
         if (this.selectedPaymentComponent == 'paymentUS') {
            return 670;

         }
         return 840;
      }
   },
}
</script>