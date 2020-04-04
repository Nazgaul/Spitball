<template>
    <div class="homeBoxes d-block d-sm-flex">
        <div class="box mb-11">
            <div class="image"></div>
            <h3 class="desc mt-5 mb-2" v-t="'homePage_online_teach'"></h3>
            <div class="d-flex justify-center align-center">
                <router-link :to="{ name: routeNames.StudyRoom }" class="link onlineTeach mr-3" v-t="'homePage_online_teach_link'"></router-link>
                <img src="./images/line-3.png" alt="">
            </div>
        </div>
        <div class="box mb-11">
            <div class="image"></div>
            <h3 class="desc mt-5 mb-2" v-t="'homePage_pay_and_receive'"></h3>
            <div class="d-flex justify-center align-center">
                <router-link to="/" class="link payments mr-3" v-t="'homePage_pay_and_receive_link'"></router-link>
                <img src="./images/line-3-copy.png" alt="">
            </div>
        </div>
        <div class="box mb-11">
            <div class="image"></div>
            <h3 class="desc mt-5 mb-2" v-t="'homePage_find_relevant'"></h3>
            <div class="d-flex justify-center align-center">
                <router-link to="/" class="link relevant mr-3" v-t="'homePage_find_relevant_link'"></router-link>
                <img src="./images/line-3-copy-2.png" alt="">
            </div>
        </div>
    </div>
</template>

<script>
// import arrow from './arrow.svg'
import * as routeNames from '../../../routes/routeNames'

export default {
    components: {
        // arrow
    },
    data() {
        return {
            routeNames
        }
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