<template>
    <div class="request-box-wrap">
        <v-layout>
            <v-flex xs12 sm12 md12 class="text-xs-center pt-4">
                <span class="subheading font-weight-bold request-box-title" v-language:inner>requestActions_title
                </span>
            </v-flex>
        </v-layout>
        <v-layout align-center class="pt-3 pb-3">
            <v-flex sm4 md4 class="btn-wrap text-xs-center">
                <v-btn round color="#e3e5ff" class="light-btn px-4">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-edit-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="text-capitalize body-2 font-weight-bold" v-language:inner>requestActions_btn_ask</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="text-capitalize body-2 font-weight-bold" v-language:inner>requestActions_btn_ask_mob</span>
                </v-btn>
            </v-flex>
            <v-flex sm4 md4 class="btn-wrap text-xs-center">
                <v-btn round color="#e3e5ff" class="light-btn px-4">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-edit-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp"  class="text-capitalize body-2 font-weight-bold" v-language:inner>requestActions_btn_upload</span>
                    <span v-show="$vuetify.breakpoint.xsOnly"  class="text-capitalize body-2 font-weight-bold" v-language:inner>requestActions_btn_upload_mob</span>
                </v-btn>
            </v-flex>
            <v-flex sm4 md4 class="btn-wrap text-xs-center">
                <v-btn round color="#e3e5ff" class="light-btn px-4">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-edit-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp"  class="text-capitalize body-2 font-weight-bold" v-language:inner>requestActions_btn_tutor</span>
                    <span v-show="$vuetify.breakpoint.xsOnly"  class="text-capitalize body-2 font-weight-bold" v-language:inner>requestActions_btn_tutor_mob</span>
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    export default {
        name: "requestActions"
    };
</script>

<style lang="less">
    @import '../../styles/mixin.less';

    .request-box-wrap {
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
        background-color: @color-white;
        .request-box-title {
            color: @textColor;
            letter-spacing: -0.4px;
        }
        .btn-wrap{
            /*margin-right: 12px;*/
        }
        .light-btn{
            border: solid 1px @lightBtnBorderColor;
            border-radius: 16px;
            height: 32px;
            color: @textColor;
            width: 189px;
            @media(max-width: @screen-xs){
                width: 110px;
            }
            .light-btn-icon{
                color: @lightBtnIconColor;
            }
        }

    }

</style>