<template>
    <div class="add-school-class-container" :class="{'active': popupData.show}">
        <div class="header-container">
            <div class="close-botton"><v-icon @click="close()">sbf-close</v-icon></div>
            <div class="title">
                <span>{{title}}</span>
            </div>
        </div>
        <div class="bottom-container">
            <div class="school-prop">
                <div class="outer-div">
                    <input type="text" maxlength="100" spellcheck="true" autocomplete="off" v-model="propName"/>
                </div>
                
            </div>
            <div class="buttons-container">
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
        }
    },
    data(){
        return{
            schoolDesc: LanguageService.getValueByKey("uniSelect_school_desc"),
            propName: "",
            errorString: ""
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
            position: relative;
            height: 40px;
            background-color: #f7f7f7;
            display:flex;
            flex-direction: column;
            justify-content: center;
            border-top-left-radius: 4px;
            border-top-right-radius: 4px;
            .close-botton{
                position: absolute;
                right:10px;
                top:10px;
                i{
                    font-size: 16px;
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
            .title{
                margin-left: 15px;
                font-family: @fontFiraSans;
                font-size: 18px !important;
                font-weight: normal;
                font-style: normal;
                font-stretch: normal;
                line-height: 1.5;
                letter-spacing: normal;
                color: rgba(0, 0, 0, 0.87);
            }
        }
        .bottom-container{
            margin: 0px auto;
            text-align: center;
            .school-prop{
                background-image: linear-gradient(140deg, #4b8ffe, rgba(68, 82, 252, 0.91));
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
                        border: 2px solid #c5c5c5;
                    }
                }
                
            }
            .buttons-container{
                display: flex;
                flex-direction: column;
                justify-content: space-around;
                span{
                    margin-top: 4px;
                    color: #ff5252;
                }
                button{
                    padding: 10px;
                    outline: none;
                    border-radius: 24px;
                    background-color: #4452fc;
                    color:#FFF;
                    margin: 7px auto;
                }
            }
        }
    }
</style>
