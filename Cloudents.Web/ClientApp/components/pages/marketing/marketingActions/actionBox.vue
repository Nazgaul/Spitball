<template functional>
    <v-col class="box pa-0" cols="12" sm="4">
        <img class="boxImage mb-4 mt-4 mt-sm-0" :src="props.data.image" alt="">
        <div class="text1 mb-1">{{props.data.title1}}</div>
        <div class="text2 mb-3">{{props.data.title2}}</div>
    <v-btn class="marketingbtn mb-4 mb-sm-0" color="#4c59ff" outlined rounded @click="props.data.action">{{props.data.buttonText}}</v-btn>
    </v-col>
</template>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';
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
            min-width: 120px !important; //vuetify
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
        &:nth-child(2) {
            .boxImage {
                margin-top: 0 !important;
            }
        }
    }
</style>