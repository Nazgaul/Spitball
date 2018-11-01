<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <h3 class="upload-cloud-text sb-title">Set your price</h3>
            <h4 class="sb-subtitle mt-2">In spitball platform You can buy and sell with SBL’s.
                <a href="" class="sbl-faq">What are SBL’s? </a></h4>
        </div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder">
                <label :for="'school'" class="steps-form-label school mb-2">
                    <v-icon class="mr-1">sbf-tag-icon</v-icon>
                    <span>Price</span></label>
                <div class="wrap-row-price">
                    <div class="price-wrap">
                        <sbl-currency v-model="uploadPrice"
                                      class="sb-input-upload-price">
                        </sbl-currency>

                        <div class="sbl-suffix">SBL</div>

                        <span class="balance-label" v-show="uploadPrice">Equals to {{uploadPrice | dollarVal}}$</span>

                    </div>
                    <div class="sb-current-balance">
                        <v-icon class="sb-wallet-icon">sbf-wallet-new</v-icon>
                        <span name="account-balance" class="sb-account-amount">{{accountUser.balance | currencyLocalyFilter}}</span>
                        <span class="balance-label">Your balance</span>
                    </div>
                </div>
            </div>
            <div class="btn-holder">

            </div>
        </div>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import sblCurrency from "../sbl-currency.vue"
    import {  currencyValidator } from "../consts";

    export default {
        name: "uploadStep_6",
        components: {sblCurrency},
        data() {
            return {
           uploadPrice: null,

            }
        },
        props: {
            callBackmethods: {
                type: Object,
                default: {},
                required: false
            }
        },
        computed: {
            ...mapGetters({
                getLegal: 'getLegal',
                getFileData: 'getFileData',
                getSchoolName: 'getSchoolName'
            }),
        },
        methods: {
            ...mapActions(['updateLegalAgreement']),

            changeStep(step) {
                this.callBackmethods.nextStep(step)
            },
            updateLegal() {
                this.updateLegalAgreement(this.legalCheck);
            },
        },

    }
</script>

<style >

</style>