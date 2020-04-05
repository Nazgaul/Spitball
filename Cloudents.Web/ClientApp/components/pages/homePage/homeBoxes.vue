<template>
    <div class="homeBoxes d-block d-sm-flex">
        <div class="box mb-11 mb-sm-0">
            <div class="image">
                <bubblePurpleMobileIcon v-if="$vuetify.breakpoint.smAndDown" />
                <bubblePurpleIcon v-else />
            </div>
            <h3 class="desc mt-6 mb-3" v-t="'homePage_online_teach'"></h3>
            <div class="d-flex justify-center align-center">
                <router-link :to="{ name: routeNames.StudyRoom }" target="_blank" class="link onlineTeach mr-3" v-t="'homePage_online_teach_link'"></router-link>
                <arrow class="arrow purpleArrow" />
            </div>
        </div>
        <div class="box mb-11 mb-sm-0 mx-2">
            <div class="image">
                <bubbleBlueMobileIcon v-if="$vuetify.breakpoint.smAndDown" />
                <bubbleBlueIcon v-else />
            </div>
            <h3 class="desc mt-6 mb-3" v-t="'homePage_pay_and_receive'"></h3>
            <div class="d-flex justify-center align-center">
                <a :href="payAndReceiveLink" class="link payments mr-3" v-t="'homePage_pay_and_receive_link'"></a>
                <arrow class="arrow blueArrow" />
            </div>
        </div>
        <div class="box mb-11 mb-sm-0">
            <div class="image">
                <bubbleYellowMobileIcon v-if="$vuetify.breakpoint.smAndDown" />
                <bubbleYellowIcon v-else />
            </div>
            <h3 class="desc mt-6 mb-3" v-t="'homePage_find_relevant'"></h3>
            <div class="d-flex justify-center align-center">
                <a class="link relevant mr-3" href="https://www.spitball.co/tutor-list" target="_blank" v-t="'homePage_find_relevant_link'"></a>
                <arrow class="arrow yellowArrow" />
            </div>
        </div>
    </div>
</template>

<script>
import bubblePurpleIcon from './images/bubble-purple.svg'
import bubbleBlueIcon from './images/bubble-blue.svg'
import bubbleYellowIcon from './images/bubble-yellow.svg'
import bubblePurpleMobileIcon from './images/bubble-purple-small.svg'
import bubbleBlueMobileIcon from './images/bubble-blue-small.svg'
import bubbleYellowMobileIcon from './images/bubble-yellow-small.svg'

import arrow from './images/arrow.svg'
import * as routeNames from '../../../routes/routeNames'

export default {
    components: {
        bubblePurpleIcon,
        bubbleBlueIcon,
        bubbleYellowIcon,
        bubblePurpleMobileIcon,
        bubbleBlueMobileIcon,
        bubbleYellowMobileIcon,
        arrow
    },
    data() {
        return {
            routeNames
        }
    },
    computed: {
        payAndReceiveLink() {
            let links = {
                IL: 'https://help.spitball.co/en/article/%D7%9E%D7%A2%D7%A8%D7%9B%D7%AA-%D7%94%D7%AA%D7%A9%D7%9C%D7%95%D7%9E%D7%99%D7%9D-%D7%A9%D7%9C-%D7%A1%D7%A4%D7%99%D7%98%D7%91%D7%95%D7%9C',
                US: 'https://help.spitball.co/en/article/spitballs-payment-system '
            }
            return links[global.country]
        }
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
@import "../../../styles/colors.less";

.homeBoxes {
    .responsive-property(margin-bottom, 90px, null, 60px);
    justify-content: space-evenly;
    margin-right: 10px;
    margin-left: 10px;
    .box {
        text-align: center;
        width: 100%;
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
        .arrow {
            cursor: pointer;
            &.purpleArrow {
                path {
                    fill: #a052f3;
                }
            }
            &.blueArrow {
                path {
                    fill: #3293f6;
                }
            }
            &.yellowArrow {
                path {
                    fill: #f5a623;
                }
            }
        }
    }
}
</style>