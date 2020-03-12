<template>
    <v-snackbar
        absolute
        top
        :timeout="10000"
        :value="showToaster"
        @input="onCloseToaster"
    >
        <i18n path="dashboardPage_pending_payment" tag="div">
            <span>{{pendingPayment}}</span>
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
            this.$store.commit('setToaster', '')
            this.showToaster = false
        }
    }
}
</script>