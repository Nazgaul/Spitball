<template>
    <div class="cash-out-card" :class="{'active':available}">
        <div class="data">
            <div class="points">
                <div class="num">{{cost}}</div>
                <div class="data">
                    <div class="text">points</div>
                    <div class="dolar-val">{{pointsForDollar}} points = $1</div>
                </div>
            </div>
            <button class="redeem-btn" @click="redeem(cost)">Redeem</button>
        </div>
        <img :src="require(`./../img/${imageSrc}`)"/>
    </div>
</template>

<script>
    import walletService from '../../../services/walletService';

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
            redeem(amount){
             walletService.redeem(amount)
                 .then(response => {
                        console.log(response)
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
