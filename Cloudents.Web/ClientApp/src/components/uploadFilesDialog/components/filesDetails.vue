<template>
    <div class="uf-sEdit">
        <v-row class="uf-sEdit-top ma-0 pa-0 pb-sm-1 px-3 justify-space-between" v-if="fileItems.length > 1 && !isError" dense>
            <v-col cols="12" sm="6" class="pa-0">
                <v-combobox
                    v-model="tutorCourse"
                    :items="suggestsCourses"
                    :rules="[rules.required]"
                    @keyup="searchCourses"
                    :label="$t('upload_label_course_name')"
                    :append-icon="''"
                    placeholder=" "
                    color="#4c59ff"
                    height="44"
                    hide-no-data outlined dense
                />
            </v-col>
            <v-col cols="6" sm="3" class="pa-0 pl-sm-4">
                <v-select
                    v-model="currentPrice"
                    :items="currentPriceItems"
                    v-if="isTutorSubscribe"
                    :rules="[rules.required]"
                    :label="$t('upload_file_price_label')"
                    :append-icon="'sbf-menu-down'"
                    placeholder=" "
                    color="#4c59ff"
                    height="44"
                    autocomplete="off"
                    hide-no-data
                    outlined
                    dense
                >
                </v-select>
                <v-text-field
                    v-model="currentPrice"
                    v-else
                    type="number"
                    :rules="[rules.required,rules.integer,rules.maximum,rules.minimum]"
                    :label="$t('upload_file_price_label')"
                    :suffix="currentPrice ? $t('upload_uf_price_pts') :''"
                    placeholder=" "
                    autocomplete="off"
                    color="#4c59ff"
                    height="44"
                    dense
                    outlined
                >
                </v-text-field>
            </v-col>
            <v-col cols="6" sm="3" class="pl-4">
                <v-btn @click="applyAll" class="uf-sEdit-top-btn" color="#4c59ff" height="44" block depressed rounded outlined>
                    <span v-t="'upload_uf_sEdit_top_btn'"/>
                </v-btn>
            </v-col>
        </v-row>
        <div class="uf-sEdit-items" :class="[isMobile ? 'pt-2 px-0' : 'py-4 px-4']">
            <v-form ref="filesDetailsForm">
                <transition-group name="slide-x-transition" class="spanTransition" >
                    <div class="fileWrapper" v-for="(fileItem, index) in fileItems" :key="fileItem.id">
                        <file-card v-if="!fileItem.error" :fileItem="fileItem" :singleFileIndex="index"/>
                        <file-card-error v-else :fileItem="fileItem" :singleFileIndex="index"/>                    
                    </div>
                </transition-group>
            </v-form>
        </div>
    </div>
</template>
<script>
import debounce from "lodash/debounce";
import courseService from '../../../services/courseService.js'

import { mapGetters, mapActions } from 'vuex';

import { validationRules } from '../../../services/utilities/formValidationRules';

import fileCard from './fileCard.vue';
import fileCardError from './fileCardError.vue';

export default {
    name: "filesDetails",
    components: {fileCard, fileCardError},
    data() {
        return {
            tutorCourse: '',
            suggestsCourses: [],
            someVal: '',
            priceForAll: '',
            fileItems: this.getFileData(),
            currentPrice: '',
            currentPriceItems: [
                { text: this.$t('upload_free_all'), value: 'Free' },
                { text: this.$t('upload_subscriber_only'), value: 'Subscriber' }
            ],
            rules: {
                required: (value) => validationRules.required(value),
                integer: (value) => validationRules.integer(value),
                maximum: (value) => validationRules.maxVal(value, 2147483647),
                minimum: (value) => validationRules.minVal(value,0),
            },
        }
    },
    props: {
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
        isTutorSubscribe() {
            return this.$store.getters.getIsTutorSubscription
        },
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
            if(this.currentPrice){
                let type = 'price'
                if(this.isTutorSubscribe) {
                    type = 'priceType'
                }
                this.setAllPrice({type, value: this.currentPrice})
            }
            if(!!this.tutorCourse){
                this.setAllCourse(this.tutorCourse)
            }
        },
        searchCourses: debounce(function(ev){
            let term = ev.target.value.trim()
            if(!term) {
                this.tutorCourse = ''
                this.suggestsCourses = []
                return 
            }
            if(!!term){
                courseService.getCourse({term, page:0}).then(data=>{
                    this.suggestsCourses = data;
                    if(this.suggestsCourses.length) {
                        this.suggestsCourses.forEach(course=>{
                            if(course.text === this.tutorCourse){
                                this.tutorCourse = course
                            }}) 
                    } else {
                        this.tutorCourse = term
                    }
                })
            }
        },200),
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
        align-items: baseline;
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
            border-radius: 6px;
        }
        input, .v-select__selections{
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
            
            .v-input__slot {
                ::placeholder{
                    font-size: 14px;
                    color: #a1a3b0;
                }
            }
        }
        .uf-sEdit-top-btn{
            font-size: 14px;
            font-weight: 600;
        }

   }
    .uf-sEdit-items{
        @media (max-width: @screen-xs) {
            height: 100%;
            max-height: unset;
        }
        background:#cfcfd0;
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
        .spanTransition .fileWrapper:last-child {
            .uf-sEdit-item {
                margin-bottom: 0 !important;

                @media (max-width: @screen-xs) {
                    margin-bottom: 8px !important;    
                }
            }
        }
    }
}
</style>