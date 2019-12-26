<template>
    <div class="uf-sEdit">
        <v-layout wrap justify-space-between class="uf-sEdit-top pb-1 px-3" v-if="fileItems.length > 1 && !isError">
            <v-flex xs12 sm6 :class="[{'pl-4':!isMobile}]">
                <v-combobox 
                    browser-autocomplete="abcd"
                    :placeholder="coursePlaceHolder"
                    class="text-truncate"
                    flat hide-no-data
                    :append-icon="''"
                    v-model="courseForAll"
                    :items="getSelectedClasses"
                    :rules="[rules.matchCourse]"/>
            </v-flex>
            <v-flex xs6 sm3 :class="[{'pl-4':!isMobile}]">
                <v-text-field v-model="priceForAll" 
                              :placeholder="emptyPricePlaceholder"
                              :rules="[rules.integer,rules.maximum,rules.minimum]"
                              type="number"
                              :suffix="priceForAll? pricePts :''"/>
            </v-flex>
            <v-flex xs5 sm3 :class="[{'pl-4':!isMobile}]">
                <v-btn @click="applyAll" class="uf-sEdit-top-btn" color="white" depressed rounded>
                    <span v-language:inner="'upload_uf_sEdit_top_btn'"/>
                </v-btn>
            </v-flex>
        </v-layout>
        <div :class="['uf-sEdit-items',isMobile?'py-3':'pt-4',isMobile? 'px-2': 'px-3']" class="">
            <!-- <div v-if="isError" class="uf-sEdit-items-error mb-2" v-language:inner="'upload_uf_sEdit_items_error'"/> -->
            <v-form ref="filesDetailsForm">
                <transition-group name="slide-x-transition">
                    <div v-for="(fileItem, index) in fileItems" :key="fileItem.id">
                        <file-card v-if="!fileItem.error" :fileItem="fileItem" :singleFileIndex="index"/>
                        <file-card-error v-else :fileItem="fileItem" :singleFileIndex="index"/>                    
                    </div>
                </transition-group>
            </v-form>
        </div>
    </div>
</template>
<script>
import { mapGetters, mapActions } from 'vuex';

import {LanguageService} from '../../../services/language/languageService.js'
import { validationRules } from '../../../services/utilities/formValidationRules';


import fileCard from './fileCard.vue';
import fileCardError from './fileCardError.vue';

export default {
    name: "filesDetails",
    components: {fileCard,fileCardError},
    data() {
        return {
            coursePlaceHolder: LanguageService.getValueByKey("upload_uf_course_name"),
            emptyPricePlaceholder: LanguageService.getValueByKey("upload_uf_price"),
            pricePts: LanguageService.getValueByKey("upload_uf_price_pts"),
            someVal: '',
            priceForAll: '',
            fileItems: this.getFileData(),
            courseForAll:'',
            rules: {
                matchCourse:() => (
                    (   this.getSelectedClasses.length && 
                        this.getSelectedClasses.some(course=>course.text === this.courseForAll.text)
                        )) 
                    || LanguageService.getValueByKey("tutorRequest_invalid"),
                integer: (value) => validationRules.integer(value),
                maximum: (value) => validationRules.maxVal(value, 1000),
                minimum: (value) => validationRules.minVal(value,0),
            },
            // suggestsCourses:[]
        }
    },
    props: {
        propName: {
            type:String,
            default: ''
        },
        showError: {
            type: Boolean,
            default: false
        },
        errorText: {
            type:String,
            default: 'testin error'
        },
        callBackmethods: {
            type: Object,
            default(){
                return{}
            },
            required: false
        },
        chackValidation:{
            type:Boolean
        }
    },
    watch: {
        chackValidation(){
            if(this.$refs.filesDetailsForm.validate()){
                this.callBackmethods.send();
            }
        }
    },
    computed: {
        ...mapGetters(['getSelectedClasses']),
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isError(){
            return this.fileItems.some(item=>item.error)
        }
    },
    methods: {
        ...mapGetters(['getFileData']),
        ...mapActions(['setAllPrice','setAllCourse']),
        applyAll(){
            if(this.priceForAll){
                this.setAllPrice(this.priceForAll)
            }
            if(!!this.courseForAll){
                this.setAllCourse(this.courseForAll)
            }
        },
    },
}
</script>

<style lang="less">
    @import "../../../styles/mixin.less";
.uf-sEdit{
    @media (max-width: @screen-xs) {
        height: 100%;
        display: flex;
        flex-direction: column;
    }
   .uf-sEdit-top{
       @media (max-width: @screen-xs) {
        .flexSameSize()
       }
        .v-text-field__suffix{
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
        }
        input[type='number'] {
            -moz-appearance:textfield;
        }
        input[type=number]::-webkit-inner-spin-button, 
        input[type=number]::-webkit-outer-spin-button { 
            -webkit-appearance: none; 
        }
        .v-messages__message {
            line-height: 1.2;
        }
        .v-input__slot{
            margin-bottom:6px;
        }
        input{
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
            
            .v-input__slot {
                ::placeholder{
                    font-size: 14px;
                    color: #a1a3b0;
                    // font-weight:100;
                }
            }
        }
        .v-btn{
            @media (max-width: @screen-xs) {
                min-width: auto;
            }
            min-width: 150px;
            height: 40px !important;
            text-transform: capitalize !important;
            margin-left: 0;
            margin-right: 0;
        }
        .uf-sEdit-top-btn{
            color: @global-blue;
            border: 1px solid @global-blue !important;
            font-size: 14px;
            font-weight: 600;
            letter-spacing: -0.26px;
        }

   }
    .uf-sEdit-items{
        @media (max-width: @screen-xs) {
            height: 100%;
            max-height: unset;
        }
        background:#f0f0f2;
        border-top: 1px solid #e2e2e4;
        border-bottom: 1px solid #e2e2e4;
        max-height: 480px;
        overflow: auto;
        .uf-sEdit-items-error{
            font-size: 16px;
            font-weight: 600;
            font-style: normal;
            font-stretch: normal;
            line-height: 1.75;
            text-align: center;
            color: #d16061;
            margin-top: -8px;
        }
    }
}
</style>