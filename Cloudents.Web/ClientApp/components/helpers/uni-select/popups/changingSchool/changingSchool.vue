<template>
    <div class="changing-school-container" :class="{'active': popupData.show}">
        <div class="header-container">
            <div class="warning-triangle"><span>!</span></div>
            <div class="icons">
                <span><v-icon class="blue">sbf-school</v-icon></span>
                <span><v-icon class="blue small">sbf-curved-arrow</v-icon></span>
                <span><v-icon>sbf-school</v-icon></span>
            </div>
            <div class="title">
                <span v-language:inner>uniSelect_changing_school_question</span>
            </div>
        </div>
        <div class="bottom-container">
            <div class="description">
                <span class="blueText">{{schoolName}}</span>
                <span>{{schoolDesc}}</span>
            </div>
            <div class="buttons-container">
                <button class="transparent" @click="continueAction()" v-language:inner>uniSelect_yes_change</button>
                <button @click="revertAction()" v-language:inner>uniSelect_keep_school</button>
            </div>
        </div>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';
import { LanguageService } from "../../../../../services/language/languageService";

export default {
    props:{
        popupData:{
            type: Object,
            required: true
        }
    },
    data(){
        return{
            schoolDesc: LanguageService.getValueByKey("uniSelect_school_desc")
        }
    },
    computed:{
        schoolName(){
           return this.getSchoolName()
        }
    },
    methods:{
        ...mapGetters(["getSchoolName"]),
        revertAction(){
            this.popupData.continueActionFunction(true);
            this.popupData.closeFunction();
        },
        continueAction(){
            this.popupData.continueActionFunction();
            this.popupData.closeFunction();
        },
    }
}
</script>

<style lang="less">
    @import "../../../../../styles/mixin.less";
    .changing-school-container{
        display: none;
        height: 338px;
        border-radius: 4px;
        box-shadow: 0 1px 17px 0 rgba(0, 0, 0, 0.39);
        background-color: #ffffff;
        z-index: 15;
        position: absolute;
        .responsive-property(width, 469px, null, 96%);
        .responsive-property(top, 38px, null, 0);
        &.active{
            display: block;
        }
        .header-container{
            position: relative;
            height: 125px;
            background-color: #f7f7f7;
            display:flex;
            flex-direction: column;
            justify-content: center;
            border-top-left-radius: 4px;
            border-top-right-radius: 4px;
            .warning-triangle{
                position: absolute;
                right:0;
                top:0;
                width: 0;
                height: 0;
                border-style: solid;
                border-width: 0 55px 55px 0;
                border-color: transparent #ffeb3b transparent transparent !important;
                span{
                    position: absolute;
                    right: -40px;
                    top: 4px;
                    font-weight: 600;
                    font-family: @fontFiraSans;
                    font-size: 18px;
                    font-weight: normal;
                    font-style: normal;
                    font-stretch: normal;
                    line-height: normal;
                    letter-spacing: normal;
                    text-align: center;
                    color: rgba(0, 0, 0, 0.87);
                }
            }
            .icons{
                display:flex;
                margin: 14px auto;
                text-align: center;
                vertical-align: middle;
                span{
                    margin:3px;
                    i{
                        opacity: 0.35;
                        font-size: 33px;
                        &.blue{
                            color: @color-blue-new;
                        }
                        &.small{
                            font-size:15px;
                            margin-top: 10px;
                        }
                    }
                }
            }
            .title{
                margin: 0px auto;
                font-family: @fontFiraSans;
                font-size: 18px;
                font-weight: normal;
                font-style: normal;
                font-stretch: normal;
                line-height: 1.5;
                letter-spacing: normal;
                text-align: center;
                color: rgba(0, 0, 0, 0.87);
            }
        }
        .bottom-container{
            margin: 0px auto;
            text-align: center;
            .description{
                height:66px;
                white-space: pre-wrap;
                margin-top: 36px;
                line-height: 1.57;
                .blueText{
                    color: #43425d;
                    font-weight: 600;
                }
            }
            .buttons-container{
                display: flex;
                flex-direction: row;
                /*justify-content: space-evenly;*/
                justify-content: space-around;
                margin: 27px auto 37px;
                .responsive-property(width, 80%, null, 100%);
                button{
                    width: 162px;
                    height: 48px;
                    outline: none;
                    border-radius: 24px;
                    background-color: #43425d;
                    color:#FFF;
                    &.transparent{
                        background-color: transparent !important;
                        border: solid 1px #d8d6d6 !important;
                        color: rgba(0, 0, 0, 0.54);
                    }
                }
            }
        }
    }
</style>
