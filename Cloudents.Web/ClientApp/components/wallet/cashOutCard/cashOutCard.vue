<template>
    <div class="cash-out-card" :class="{'active':available}">
        <div class="data points-wrap">
            <bdi>
                <div class="points">
                    <div class="num">{{cost | commasFilter}}</div>
                    <div class="data">
                        <div class="text dolar-val" v-language:inner="'cashoutcard_SBL'"></div>
                    </div>
                </div>
            </bdi>
            <v-btn class="redeem-btn"
                   flat
                   value="Redeem"
                   :loading="loading"
                   :disabled="!available"
                   @click="redeem(cost)"><span v-language:inner="'cashoutcard_Redeem'"></span></v-btn>
        </div>
        <img :src="require(`./../img/${imageSrc}`)" />
    </div>
</template>

<script>
    import walletService from '../../../services/walletService';
    import { mapActions } from 'vuex';
    import { LanguageService } from '../../../services/language/languageService';
    export default {
        data() {
            return {
                loading: false
            }
        },
        props: {
            pointsForDollar: {
                type: Number
            },
            cost: {
                type: Number
            },
            image: {
                type: String
            },
            available: {
                type: Boolean
            },
            updatePoint: {
                type: Function
            }
        },
        methods: {
            ...mapActions({
                updateBalance: 'updateUserBalance',
                updateToasterParams: 'updateToasterParams'
            }),
            redeem(amount) {
                this.loading = true;
                walletService.redeem(amount)
                    .then(() => {
                        // show toaster text
                        this.updateToasterParams({
                            toasterText: LanguageService.getValueByKey('cashoutcard_Cashed'),
                            showToaster: true,
                        });
                        //update user balance
                        this.updateBalance(-amount);
                        this.updatePoint();
                        this.loading = false;
                    },
                        error => {
                            console.error('error getting transactions:', error)
                        });

            }
        },
        computed: {
            imageSrc() {
                return this.available ? this.image + '-active.png' : this.image + '-disactivate.png'
            }
        },
    }
</script>
<style src="./cashOutCard.less" lang="less"></style>
