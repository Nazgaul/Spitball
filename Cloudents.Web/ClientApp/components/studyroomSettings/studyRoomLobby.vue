<template>
  <div>
      <p>to access to study room u have to:</p>
      <p>complete: </p>
      <p>{{missing.type}}  
         <v-btn v-if="isNeedPayment" @click="Payment(missing)" color="success">complete</v-btn>
         <v-icon v-else color="success">sbf-check-circle</v-icon>
      </p>
  </div>
</template>

<script>
export default {
   props:{
      missing:{
         
      }
   },
   methods: {
      Payment:function(missing){
         let params = {title: 'payme_title', name: missing.data}
         this.$store.dispatch('requestPaymentURL', params); 
      },
   },
   computed: {
      isNeedPayment(){
         return this.$store.getters.getStudyRoomData.needPayment
      }
   },
   // created() {
   //    this[this.missing.type](this.missing)
   // },
}
</script>
