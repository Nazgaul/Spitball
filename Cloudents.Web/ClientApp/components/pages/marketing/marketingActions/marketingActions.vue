<template>
    <v-row class="marketingActions pa-4 mb-2 mb-sm-4 text-center" dense>
        <v-col class="pa-0 mb-6 d-flex justify-space-between" cols="12">
            <div class="text text-left">{{$t('marketing_title')}}</div>
            <slot name="closeIcon"></slot>
        </v-col>
        
        <template>
            <v-col class="box pa-0 " cols="12" sm="4">
                <img class="mb-4 mt-sm-0" src="../images/sharePost.png" alt="">
                <div class="text1 mb-1">{{$t('marketing_sharePost_title1')}}</div>
                <div class="text2 mb-3">{{$t('marketing_sharePost_title2')}}</div>
                <v-btn class="marketingbtn mb-4 mb-sm-0" color="#4c59ff" disabled outlined rounded>{{$t('marketing_lets_go')}}</v-btn>
            </v-col>
            <v-col class="box pa-0" cols="12" sm="4">
                <img class="mb-4 mt-4 mt-sm-0" src="../images/specialOffer.png" alt="">
                <div class="text1 mb-1">{{$t('marketing_createOffer_title1')}}</div>
                <div class="text2 mb-3">{{$t('marketing_createOffer_title2')}}</div>
                <v-btn v-openDialog="'createCoupon'" class="marketingbtn mb-4 mb-sm-0" color="#4c59ff" outlined rounded>{{$t('marketing_get_started')}}</v-btn>
            </v-col>
            <v-col class="box pa-0" cols="12" sm="4">
                <img class="mb-4 mt-4 mt-sm-0" src="../images/createVideo.png" alt="">
                <div class="text1 mb-1">{{$t('marketing_createVideo_title1')}}</div>
                <div class="text2 mb-3">{{$t('marketing_createVideo_title2')}}</div>
                <v-btn class="marketingbtn" color="#4c59ff" outlined rounded @click="openIntercom">{{$t('marketing_get_started')}}</v-btn>
            </v-col>
        </template>
    </v-row>
</template>

<script>
import intercomService from "../../../../services/intercomService";

export default {
    name: "marketingActions",
    props: {
        resource: {
            type: String,
        }
    },
    computed: {
        title() {
            return this.setResource({
                
            })
        }
    },
    methods: {
        openIntercom() {
            intercomService.showDialog();
        },
        setResource(nameObj) {
            return this.$t(nameObj[this.resource]);
        }
    },
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';
    .marketingActions {
        background: white;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 8px;

        @media (max-width: @screen-xs) {
            box-shadow: none;
        }

        .text {
            color: @global-purple;
            font-weight: 600;
            font-size: 20px;
            @media (max-width: @screen-xs) {
                font-size: 16px;
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
                margin-bottom: 2px;
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