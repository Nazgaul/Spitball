<template>
    <div class="uf-sEdit">
        <v-layout row wrap justify-space-between class="uf-sEdit-top px-3">
            <v-flex xs12 sm6 :class="[{'pl-3':!isMobile}]">
                <v-combobox 
                    :placeholder="coursePlaceHolder"
                    class="text-truncate"
                    @keyup="searchCourses"
                    flat hide-no-data
                    :append-icon="''"
                    v-model="courseForAll"
                    :items="suggestsCourses"
                    :rules="[rules.matchCourse]"/>
            </v-flex>
            <v-flex xs6 sm3 :class="[{'pl-3':!isMobile}]">
                <v-text-field v-model="priceForAll" placeholder="Price"></v-text-field>
            </v-flex>
            <v-flex xs5 sm3 :class="[{'pl-3':!isMobile}]">
                <v-btn @click="applyAll" class="uf-sEdit-top-btn" color="white" depressed round>
                    <span v-language:inner="'upload_uf_sEdit_top_btn'"/>
                </v-btn>
            </v-flex>
        </v-layout>
        <div :class="['uf-sEdit-items',isMobile?'py-3':'pt-3',isMobile? 'px-2': 'px-3']" class="">
            <div v-if="isError" class="uf-sEdit-items-error my-2" v-language:inner="'upload_uf_sEdit_items_error'"/>
            <transition-group name="slide-x-transition">
                <div v-for="(fileItem, index) in fileItems" :key="fileItem.id">
                    <file-card v-if="!fileItem.error" :fileItem="fileItem" :singleFileIndex="index"/>
                    <file-card-error v-else :fileItem="fileItem" :singleFileIndex="index"/>                    
                </div>
            </transition-group>
        </div>
    </div>
</template>
<script>
import { mapGetters, mapActions } from 'vuex';
import debounce from "lodash/debounce";

import {LanguageService} from '../../../services/language/languageService.js'
import universityService from '../../../services/universityService.js';

import fileCard from './fileCard.vue';
import fileCardError from './fileCardError.vue';

export default {
    name: "filesDetails",
    components: {fileCard,fileCardError},
    data() {
        return {
            coursePlaceHolder: LanguageService.getValueByKey("upload_uf_course_name"),
            someVal: '',
            priceForAll: '',
            fileItems: this.getFileData(),
            courseForAll:'',
            rules: {
                matchCourse:() => (
                    (   this.suggestsCourses.length && 
                        this.suggestsCourses.some(course=>course.text === this.courseForAll.text)
                        )) 
                    || LanguageService.getValueByKey("tutorRequest_invalid"),
            },
            suggestsCourses:[]
        }
    },
    props: {
        propName: '',
        showError: {
            type: Boolean,
            default: false
        },
        errorText: 'testin error',
        callBackmethods: {
            type: Object,
            default: {},
            required: false
        },
    },
    computed: {
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
.uf-sEdit{
    @media (max-width: @screen-xs) {
        height: 100%;
        display: flex;
        flex-direction: column;
    }
   .uf-sEdit-top{
       @media (max-width: @screen-xs) {
           flex: 0;
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
        }
    }
}
</style>