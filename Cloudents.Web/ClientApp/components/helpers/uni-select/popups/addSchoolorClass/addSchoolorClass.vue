<template>
    <div class="add-school-class-container" :class="{'active': popupData.show}">
        <div class="header-container px-2">
            <div class="title-add">
                <span>{{title}}</span>
            </div>
            <div class="close-botton"><v-icon @click="close()">sbf-close</v-icon></div>
        </div>
        <div class="bottom-container">
            <div class="school-prop">
                <div class="outer-div">
                    <input type="text" v-model="propName" :placeholder="placeholder"/>
                </div>
                
            </div>
            <div class="buttons-container py-2 align-center" >
                <span v-show="!!errorString">{{errorString}}</span>
                <!-- <button class="transparent" @click="continueAction()" v-language:inner>uniSelect_yes_change</button> -->
                <button @click="continueAction()">{{buttonText}}</button>
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
        },
        placeholder: ''
    },
    data(){
        return {
            schoolDesc: LanguageService.getValueByKey("uniSelect_school_desc"),
            propName: "",
            errorString: "",
        }
    },
    computed:{
        schoolName(){
           return this.getSchoolName()
        },
        title(){
            return this.popupData.isSchool ? LanguageService.getValueByKey('uniSelect_add_new_university') : LanguageService.getValueByKey('uniSelect_add_new_class')
        },
        buttonText(){
            return this.popupData.isSchool ? LanguageService.getValueByKey('uniSelect_add_university') : LanguageService.getValueByKey('uniSelect_add_class');
        }
    },
    methods:{
        ...mapGetters(["getSchoolName"]),
        reset(){
            this.propName = "";
            this.errorString = "";
        },
        continueAction(){
            if(this.popupData.isSchool){
                if(this.propName.length > 10){
                    this.letGo(true);
                }else{
                    this.errorString = LanguageService.getValueByKey('uniSelect_university_length_error');
                }
            }else{
                if(this.propName.length > 3){
                    this.letGo(false);
                }else{
                    this.errorString = LanguageService.getValueByKey('uniSelect_class_length_error');
                }
            }
        },
        close(){
            this.reset();
            this.popupData.closeFunction();
        },
        letGo(isSchool){
            if(isSchool){
                this.popupData.continueActionFunction(false, this.propName);
            }else{
                //if class
                this.popupData.continueActionFunction({text:this.propName});
            }
            this.close();
        }
    }
}
</script>

<style lang="less">
    @import "../../../../../styles/mixin.less";
    .add-school-class-container{
        display: none;
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
            height: 40px;
            background-color: #f7f7f7;
            display: flex;
            flex-direction: row;
            justify-content: space-between;
            border-top-left-radius: 4px;
            border-top-right-radius: 4px;
            align-items: center;
            .close-botton{
                i{
                    font-size: 14px;
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
                            color: #4452FC;
                        }
                        &.small{
                            font-size:15px;
                            margin-top: 10px;
                        }
                    }
                }
            }
            .title-add{
                letter-spacing: -.1px;
                font-family: @fontOpenSans;
                font-size: 14px;
                font-weight: 600;
                line-height: 1.23;
                text-align: center;
                color: rgba(0,0,0,.87);
            }
        }
        .bottom-container{
            margin: 0px auto;
            text-align: center;
            .school-prop{
                background-color: #5158af;
                box-shadow: 0 2px 4px 0 rgba(0,0,0,.07);
                /*background-image: linear-gradient(140deg, #4b8ffe, rgba(68, 82, 252, 0.91));*/
                display: flex;
                .outer-div{
                    background-color: #fff;
                    margin: 30px auto;
                    padding: 10px;
                    border-radius: 4px;
                    input{
                        background-color: white;
                        outline: none;
                        border-radius: 4px;
                        line-height: 25px;
                        width: 250px;
                        padding-left: 5px;
                        /*border: 2px solid #c5c5c5;*/
                    }
                }
                
            }
            .buttons-container{
                display: flex;
                flex-direction: column;
                justify-content: space-around;
                align-items: center;
                span{
                    margin-top: 4px;
                    color: #ff5252;
                }
                button{
                    font-family:@fontFiraSans;
                    display: flex;
                    max-width: 150px;
                    align-items: center;
                    padding: 10px 12px;
                    border-radius: 24px;
                    background-color: #4452fc!important;
                    text-transform: capitalize;
                    box-shadow: 0 2px 9px 0 rgba(0,0,0,.35);
                    font-size: 16px;
                    font-weight: 400;
                    color: #fff;
                    outline: none;
                    line-height: 16px;

                }
            }
        }
    }
</style>
