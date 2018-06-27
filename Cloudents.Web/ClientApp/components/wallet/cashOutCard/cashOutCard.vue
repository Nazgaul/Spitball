<template>
    <div class="cash-out-card available" :class="{'active': available}">
        <div class="data">
            <div class="points">
                <div class="num">{{cost}}</div>
                <div class="data">
                    <div class="text">points</div>
                    <div class="dolar-val">{{pointsForDollar}} points = $1</div>
                </div>
            </div>
            <button class="redeem-btn">Redeem</button>
        </div>
        <img :src="require(`./../img/${imageSrc}`)"/>
    </div>
</template>

<script>
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
        computed: {
            imageSrc() {
                return this.available ? this.image + '-active.png' : this.image + '-disactivate.png'
            }
        },
    }
</script>
<style src="./cashOutCard.less" lang="less"></style>
