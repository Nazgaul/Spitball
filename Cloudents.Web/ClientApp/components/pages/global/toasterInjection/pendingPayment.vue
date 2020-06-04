<template>
    <v-snackbar
        absolute
        top
        :timeout="0"
        :value="showToaster"
        @input="onCloseToaster"
    >
        <i18n path="dashboardPage_pending_payment" tag="div">
            <span>{{pendingPayment}}</span>
            <span>{{ $tc('dashboardPage_payment_pluralize', showPaymentText) }}</span>
            <router-link :to="{name: routeNames.MySales}">{{ $t('dashboardPage_btn_approve') }}</router-link>
        </i18n>
    </v-snackbar>
</template>


<script>
import * as routeNames from '../../../../routes/routeNames'

export default {
    name: '',
    computed: {
        pendingPayment() {
            return this.$store.getters.getPendingPayment
        },
        showPaymentText() {
            return this.pendingPayment > 1 ? 2 : 1;
        }
    },
    data() {
        return {
            showToaster :true,
            routeNames
        }
    },
    methods: {
        onCloseToaster() {
            this.$store.commit('clearComponent')
            this.showToaster = false
        }
    }
}
</script>