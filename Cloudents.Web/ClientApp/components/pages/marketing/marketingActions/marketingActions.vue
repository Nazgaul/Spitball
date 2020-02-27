<template>
    <v-row class="marketingActions pa-0 text-center" :class="{'marketingPage pa-4 mb-2 mb-sm-4': $route.name === routeNames.Marketing}">
        <v-col class="pa-0 mb-6 d-flex justify-space-between" cols="12">
            <div class="text text-left" v-if="$route.name === routeNames.Marketing">{{$t('marketing_title')}}</div>
            <div class="text text-left d-block d-sm-none" v-else>{{$t('promote_title')}}</div>
        </v-col>
        
        <template v-for="(data, index) in resource">
            <actionBox :key="index" :data="data"></actionBox>
        </template>
    </v-row>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames'
import actionBox from './actionBox.vue';

export default {
    name: "marketingActions",
    components: {
        actionBox
    },
    props: {
        resource: {
            type: Object,
            required: true
        }
    },
    data() {
        return {
            routeNames
        }
    },
    computed: {
        mdAndDown() {
            return this.$vuetify.breakpoint.mdAndDown
        },
        shareImage() {
          return this.mdAndDown ? require('../images/sharePostSmall.png') : require('../images/sharePost.png')
        },
        offersImage() {
          return this.mdAndDown ? require('../images/specialOfferSmall.png') : require('../images/specialOffer.png')
        },
        createVideo() {
          return this.mdAndDown ? require('../images/createVideoSmall.png') : require('../images/createVideo.png')
        },
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';
    .marketingActions {
        width: 100%;
        margin: 0 auto;
        &.marketingPage {
            background: white;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
            border-radius: 8px;
            @media (max-width: @screen-xs) {
                box-shadow: none;
                border-radius: 0;
            }
        }

        .text {
            color: @global-purple;
            font-weight: 600;
            font-size: 20px;
            @media (max-width: @screen-xs) {
                font-size: 18px;
            }
        }

        .box {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: space-between;
            color: @global-purple;
            .text1 {
                font-size: 16px;
                font-weight: 600;
                // margin-bottom: 2px;
            }
            .text2 {
                font-size: 12px;
            }
            .marketingbtn {
                text-transform: initial;
                font-weight: 600;
                min-width: 120px;
                letter-spacing: normal;
                span {
                    margin-bottom: 2px;
                }
            }
            &:not(:last-child) {
                border-right: 1px solid #dddddd;

                @media (max-width: @screen-xs) {
                    border-bottom: 1px solid #dddddd;
                    border-right: none;
                }
            }
        }
    }
</style>