<template>
    <v-card>
        <v-toolbar class="headline" height="40" dark color="#43425D">
          <v-icon class="subheading">sbf-credit-card-row</v-icon>
          <v-toolbar-title class="subheading" v-language:inner="'put some title here'"/>
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-card-text>
          <iframe class="payment-iframe" width="100%" height="520" :src="paymentUrl"></iframe>
        </v-card-text>
        <v-card-actions>
          <v-btn color="success" @click="closePaymentDialog" v-language:inner="'close btn'"/>
        </v-card-actions>
    </v-card>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
export default {
    name: 'paymentDIalog',
    computed: {
        ...mapGetters(['getPaymentUrl']),
        paymentUrl(){
            return this.getPaymentUrl
        }
    },
    methods: {
        ...mapActions(['updatePaymentDialog']),
        closePaymentDialog(){
            this.updatePaymentDialog(false)
        }
    },
}
</script>

<style lang="less">
    .payment-iframe{
        border: none
    }
</style>