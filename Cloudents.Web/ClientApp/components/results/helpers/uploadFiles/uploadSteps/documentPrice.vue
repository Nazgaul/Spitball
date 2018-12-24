<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <h3 class="upload-cloud-text sb-title" v-language:inner>upload_files_price_title</h3>
            <h4 class="sb-subtitle mt-2"><span v-language:inner>upload_files_price_subtitle</span>
                <a href="" class="sbl-faq">SBL’s.</a></h4>
        </div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder">
                <label :for="'school'" class="steps-form-label school mb-2">
                    <v-icon >sbf-tag-icon</v-icon>
                    <span v-language:inner>upload_files_price_label</span></label>
                <div class="wrap-row-price">
                    <div class="price-wrap">
                        <!--updating document obj inside -->
                        <sbl-currency v-model="uploadPrice"
                                      class="sb-input-upload-price">
                        </sbl-currency>
                        <div class="sbl-suffix">SBL</div>
                        <span class="balance-label" v-show="uploadPrice">
                            <span v-language:inner>upload_files_price_equals</span>
                            {{uploadPrice | dollarVal}}$
                        </span>
                        <span class="balance-label" v-show="!uploadPrice"
                              v-language:inner>upload_files_price_zero_means_free

                        </span>

                    </div>
                    <div class="sb-current-balance">
                        <v-icon class="sb-wallet-icon">sbf-wallet-new</v-icon>
                        <span name="account-balance" class="sb-account-amount">{{accountUser.balance | currencyLocalyFilter}}</span>
                        <span class="balance-label account-label"v-language:inner>upload_files_price_your_balance</span>
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
    import { currencyValidator } from "../consts";

    export default {
        name: "uploadStep_6",
        components: {sblCurrency},
        data() {
            return {
                uploadPrice: null,
            }
        },
        computed: {
            ...mapGetters({
                accountUser: 'accountUser'
            }),
        },

        methods: {
            ...mapActions(['updateFile']),

        },

    }
</script>

<style>

</style>