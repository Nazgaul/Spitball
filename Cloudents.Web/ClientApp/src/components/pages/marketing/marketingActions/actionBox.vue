<template functional>
 <!-- :sm="props.isDashboard ? '6' : '4'" -->
    <v-col class="box pa-0 px-sm-3" :cols="props.isMobile? 12 : 6">
        <img class="boxImage mb-4 mt-4 mt-sm-0" :src="props.data.image" alt="">
        <div class="text1 mb-1">{{props.data.title1}}</div>
        <div class="text2 mb-3">{{props.data.title2}}</div>
        <div class="d-flex align-center flex-column flex-sm-row">
            <slot name="courseSelect"></slot>
            <v-btn 
                @click="props.data.action ? props.data.action() : ''"
                :to="props.data.route ? props.data.route : ''"
                class="marketingbtn mb-4 mb-sm-0"
                color="#4c59ff"
                outlined
                rounded
                :disabled="props.index === 1 && Object.keys(props.currentCourseItem).length === 0"
            >
                {{props.data.buttonText}}
            </v-btn>
        </div>
    </v-col>
</template>

<style lang="less" scoped >
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
                margin-bottom: 16px;
                border-right: none;
            }
        }
        &:nth-child(2) {
            padding-left: 0 !important;
            padding-right: 24px !important;
            @media (max-width: @screen-xs) {
                padding-right: 0 !important;
            }
            .boxImage {
                margin-top: 0 !important;
            }
        }
        &:nth-child(4) {
            padding-right: 0 !important;
            padding-left: 25px !important;
            @media (max-width: @screen-xs) {
                padding-left: 0 !important;
            }
        }
    }
</style>