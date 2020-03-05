<template>
   <v-dialog :value="true" persistent :maxWidth="'840'" :fullscreen="$vuetify.breakpoint.xsOnly">
      <component :is="selectedPaymentComponent"></component>
   </v-dialog>
</template>

<script>
import paymentService from '../../../../../../services/payment/paymentService.js';
const paymentIL = () => import('./components/paymentIL.vue');
export default {
   name: 'paymentWrapper',
   components:{
      paymentIL,
   },
   computed: {
      selectedPaymentComponent(){
         return paymentService.getPaymentComponent()
      },
   },
}
</script>