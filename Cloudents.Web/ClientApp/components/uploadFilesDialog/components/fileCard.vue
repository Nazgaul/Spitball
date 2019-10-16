<template>
    <v-card class="uf-sEdit-item mb-3">
        <v-icon v-if="!isLastItem" class="uf-sEdit-close" v-html="'sbf-close'" @click="deleteFile()"/>
        <v-layout row wrap pb-1 px-3>
            <v-layout row wrap justify-space-between>   
                <v-flex xs11 md8 pr-4>
                    <v-text-field style="direction: ltr;" :prepend-inner-icon="'sbf-attachment'" 
                                  v-model="item.name" 
                                  dir="ltr"
                                  :rules="[rules.required]"/>
                </v-flex>
                <v-flex xs7 md4>
                    <v-combobox 
                    browser-autocomplete="abcd"
                        :placeholder="itemCoursePlaceholder"
                        @keyup="searchCourses"
                        flat hide-no-data
                        :append-icon="''"
                        v-model="course"
                        :items="suggestsCourses"
                        :rules="[rules.required,rules.matchCourse]"/>
                </v-flex>
                <v-flex hidden-md-and-up xs4> 
                    <v-text-field class="uf_price"
                        :rules="[rules.integer,rules.maximum,rules.minimum]"  
                         type="number" 
                         v-model="item.price" 
                         :placeholder="emptyPricePlaceholder"
                         :suffix="item.price? pricePts :''"
                         />
                </v-flex>
            </v-layout>
            <v-layout row wrap justify-space-between>
                <v-flex xs12 md9 pr-4 class="uf_desc">
                    <!-- <v-textarea rows="2" :resize="false" style="margin: 0;padding: 0;"
                        v-model="item.description" 
                        :placeholder="itemDescPlaceholder">>

                    </v-textarea> -->
                    <v-text-field 
                        class="pt-1"
                        v-model="item.description" 
                        :placeholder="itemDescPlaceholder">
                    </v-text-field>
                </v-flex>
                <v-flex hidden-sm-and-down md2> 
                    <v-text-field class="uf_price pt-1"
                        :rules="[rules.integer,rules.maximum,rules.minimum]" 
                         type="number" 
                         v-model="item.price" 
                         :placeholder="emptyPricePlaceholder"
                         :suffix="item.price? pricePts :''"
                         />
                </v-flex>
            </v-layout>
        </v-layout>
    </v-card>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import debounce from "lodash/debounce";

import { LanguageService } from "../../../services/language/languageService";
import universityService from '../../../services/universityService.js';
import { validationRules } from '../../../services/utilities/formValidationRules';

export default {
    name: "fileCard",
    data() {
        return {
            suggestsCourses: [],
            fileNamePlaceholder: LanguageService.getValueByKey("upload_multiple_fileName_placeholder"),
            emptyPricePlaceholder: LanguageService.getValueByKey("upload_uf_price"),
            itemDescPlaceholder: LanguageService.getValueByKey("upload_uf_desc"),
            itemCoursePlaceholder: LanguageService.getValueByKey("upload_uf_course_name"),
            pricePts: LanguageService.getValueByKey("upload_uf_price_pts"),
            selectedCourse:'',
            rules: {
                required: (value) => validationRules.required(value),
                integer: (value) => validationRules.integer(value),
                matchCourse:() => ((this.suggestsCourses.length && this.suggestsCourses.some(course=>course.text === this.selectedCourse)
                        ) || this.isFromQuery) || LanguageService.getValueByKey("tutorRequest_invalid"),
                maximum: (value) => validationRules.maxVal(value, 1000),
                minimum: (value) => validationRules.minVal(value,0),
            },
            isFromQuery: false,
        }
    },
    props: {
        fileItem: {
            type: Object,
            default: {}
        },
        singleFileIndex: {
            type: Number,
            required: true
        },
        quantity: {
            type: Number,
            required: false
        },
    },
    watch: {
        item: {
            deep: true,
            handler(newVal, oldVal) {
                if(this.selectedCourse !== newVal.course){
                    this.isFromQuery = true
                }
                let fileObj = {
                    index: this.singleFileIndex,
                    data: newVal
                };
                this.changeFileByIndex(fileObj);
            }
        }
    },
    computed: {
        ...mapGetters(['getFileData']),
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isLastItem(){
            let nonErrorItems = this.getFileData.filter(item=>(!item.error))
            return (nonErrorItems.length < 2)
        },
        item() {
            return this.getFileData[this.singleFileIndex]
        },
        course:{
            get(){
                return this.item.course  
            },
            set(val){
                if(!!val && val.text){
                    this.item.course = val.text
                    this.selectedCourse = val.text
                }else{
                    this.item.course = val.text || ''
                }
            }
        }
    },
    methods: {
        ...mapActions(['changeFileByIndex', 'deleteFileByIndex']),
        deleteFile() {
            this.deleteFileByIndex(this.singleFileIndex)
        },
        searchCourses: debounce(function(ev){
            let term = ev.target.value.trim()
            if(!!term){
                universityService.getCourse({term, page:0}).then(data=>{
                    this.suggestsCourses = data;
                })
            }
        },300),
    },
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.uf-sEdit-item{
    box-shadow: 0 1px 1px 0 rgba(0, 0, 0, 0.14) !important;
    border-radius: 4px;
    position: relative;
    .uf_price{
        input[type=number]::-webkit-inner-spin-button, 
        input[type=number]::-webkit-outer-spin-button { 
            -webkit-appearance: none; 
        }
        .v-text-field__suffix{
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
        }
        .v-messages__message {
            line-height: 1.2;
        }
        .v-input__slot{
            margin-bottom:6px;
        }
    }
    .uf-sEdit-close{
        position: absolute;
        right: 10px;
        top: 10px;
        z-index: 99;
        font-size: 12px;
        color: #adadba;
        cursor: pointer;  
    }
    .v-input__slot {
        ::placeholder{
            font-size: 14px;
            color: #a1a3b0;
        }
        input{
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
        }
        .v-input__icon {
            justify-content: start;
            width: 18px;
            min-width: 18px;
        }
        .v-icon{
            color: @global-purple;
            font-size: 20px;
            transform: rotate(90deg);
        }
    }
}

</style>