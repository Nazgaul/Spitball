<template>
    <div class="cash-out-card" :class="{'active':available}">
        <div class="data">
            <div class="points">
                <div class="num">{{cost}}</div>
                <div class="data">
                    <div class="text">SBL</div>
                    <div class="dolar-val">{{pointsForDollar}} SBL = $1</div>
                </div>
            </div>
            <button class="redeem-btn" @click="redeem(cost)">Redeem</button>
        </div>
        <img :src="require(`./../img/${imageSrc}`)"/>
    </div>
</template>

<script>
    import walletService from '../../../services/walletService';
    import { mapActions} from 'vuex';
    export default {

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
            }
        },
        methods: {
            ...mapActions({
                updateBalance: 'updateUserBalance',
                updateToasterParams: 'updateToasterParams'
            }),
            redeem(amount){
             walletService.redeem(amount)
                 .then(response => {
                        // show toaster text
                         this.updateToasterParams({
                             toasterText: 'Coupon will be sent via email',
                             showToaster: true,
                         });
                         //update user balance
                         this.updateBalance(-amount);
                         this.$parent.$emit('updateEarnedPoints', amount);
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
