<template>
    <v-container class="new-baller-container">
        <v-layout align-center justify-center class="new-baller-header">
                            <span class="new-baller-title d-flex text-center"
                                  v-language:inner>newBaller_title
                            </span>

        </v-layout>

        <v-layout column class="mt-4 mb-4" :class="[$vuetify.breakpoint.smAndUp ? 'px-5' : 'px-4']">
            <v-flex class="mb-2">
                            <span class="new-baller-sub-title" v-language:inner>newBaller_subtitle

                            </span>
            </v-flex>
            <div class="nb-flex align-center">
                <span class="line-number mr-4">1</span>
                <span class="line-text" v-language:inner>newBaller_list_item_one</span>
            </div>
            <div class="nb-flex align-center">
                <span class="line-number mr-4">2</span>
                <span class="line-text" v-language:inner>newBaller_list_item_two</span>

            </div>
            <div class="nb-flex align-center">
                <span class="line-number mr-4">3</span>
                <span class="line-text" v-language:inner>newBaller_list_item_three</span>
            </div>

        </v-layout>
        <v-layout class="new-baller-btm-section" :class="[$vuetify.breakpoint.smAndUp ? 'px-5' : 'px-4']" column>
            <v-flex class="mb-4">
                            <span class="bottom-text" v-language:inner>newBaller_bottom_text_one
                            </span>
            </v-flex>
            <v-flex>
                            <span class="bottom-text" v-language:inner>newBaller_bottom_text_two
                            </span>
            </v-flex>
        </v-layout>

        <v-layout class="new-baller-action-section" :class="[$vuetify.breakpoint.smAndUp ? 'py-3' : 'pb-4 pt-4']" align-center justify-center>
            <v-btn class="new-baller-btn sb-background" v-show="showNewBallerAction"
                   @click="closeNewBallerDialog()">
                <span v-language:inner>newBaller_action_got</span>
            </v-btn>
        </v-layout>
    </v-container>
</template>

<script>
    import {mapActions} from 'vuex';
    export default {
        name: "newBaller",
        data() {
            return {
                showNewBallerAction: false,
            }
        },
        props: {
            newBallerDialog: {
                type: Boolean
            },
        },
        methods: {
            ...mapActions(['updateNewBallerDialogState']),
            closeNewBallerDialog() {
                global.localStorage.setItem("sb-newBaller-suppresed", true);
                this.updateNewBallerDialogState(false);
            },
        },
        created(){
            setTimeout(() => {
                return this.showNewBallerAction = true;
            }, 2000);
        }

    }
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    .new-baller {
        border-radius: 4px;
        box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.5);
        background: @color-white;
        .new-baller-container {
            padding: 0;
            @media(max-width: @screen-xs){
                margin: 0;
            }
        }
        .nb-flex{
            display: flex;
        }
        .new-baller-header {
            background-color: @global-purple;
        }
        .new-baller-btn{
            &.v-btn {
                border-radius: 16px;
                width: 40px;
                
                font-size: 16px;
                color: fade(@color-white, 87%);
                box-shadow: none;
                height: 32px;
                text-transform: capitalize;
                &.sb-background{
                    background-color: @global-purple;


                }
            }
        }
        .new-baller-title {
            //height: 54px;
            
            font-size: 22px;
            font-weight: 600;
            line-height: 2.77;
            letter-spacing: -0.2px;
            color: fade(@color-white, 87%);
        }
        .new-baller-sub-title {  
            font-size: 16px;
            font-weight: bold;
            line-height: 1.5;
            letter-spacing: -0.2px;
            color: fade(@textColor, 90%);
        }
        .line-number {
            height: 43px;          
            font-size: 32px;
            color: @global-purple;
            @media(max-width: @screen-xs){
                font-size: 20px;
                height: 30px;
            }
        }
        .line-text {  
            font-size: 14px;
            line-height: 1.79;
            letter-spacing: -0.1px;
            color: @textColor;
        }
        .new-baller-btm-section {
            padding: 20px 0 25px 0;
            background-color: #f6f6f6;
            .bottom-text {
                max-width: 474px;
                display: inline-block;
                font-size: 13px;
                font-weight: bold;
                line-height: 1.38;
                letter-spacing: -0.1px;
                color: @global-purple;
            }
        }
        .new-baller-action-section {

        }

    }

</style>