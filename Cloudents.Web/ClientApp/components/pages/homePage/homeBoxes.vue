<template>
    <div class="homeBoxes d-block d-sm-flex">
        <div class="box mb-11">
            <div class="image"></div>
            <h3 class="desc mt-5 mb-2">Online teaching platform with videos and canvas</h3>
            <div class="d-flex justify-center align-center">
                <div class="link onlineTeach mr-3">See it for yourself</div>
                <img src="./images/line-3.png" alt="">
            </div>
        </div>
        <div class="box mb-11">
            <div class="image"></div>
            <h3 class="desc mt-5 mb-2">Pay and receive payments for content and lessong directly</h3>
            <div class="d-flex justify-center align-center">
                <div class="link payments mr-3">Learn more on payments</div>
                <img src="./images/line-3-copy.png" alt="">
            </div>
        </div>
        <div class="box mb-11">
            <div class="image"></div>
            <h3 class="desc mt-5 mb-2">find, or be found, from the most relevant students and teachers</h3>
            <div class="d-flex justify-center align-center">
                <div class="link relevant mr-3">Serach to find or get found</div>
                <img src="./images/line-3-copy-2.png" alt="">
            </div>
        </div>
    </div>
</template>

<script>
// import arrow from './arrow.svg'

export default {
    components: {
        // arrow
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
@import "../../../styles/colors.less";

.homeBoxes {
    .responsive-property(margin-bottom, 60px, null, 40px);
    justify-content: space-evenly;
    .box {
        text-align: center;
        .image {
            width: 160px;
            height: 104px;
            opacity: 0.43;
            margin: 0 auto;
            background-color: #d8d8d8;
        }
        .desc {
            font-size: 16px;
            line-height: 1.63;
            text-align: center;
            font-weight: normal;
            color: @global-purple;
            max-width: 260px;
            width: 100%;
            margin: 0 auto;
        }
        .link {
            font-size: 14px;
            font-weight: 600;
            &.onlineTeach {
                color: #a052f3;
            }
            &.payments {
                color: #3293f6;
            }
            &.relevant {
                color: #f5a623;
            }
        }
    }
}
</style>