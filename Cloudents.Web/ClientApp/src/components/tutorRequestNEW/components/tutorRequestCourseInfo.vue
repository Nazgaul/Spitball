<template>
    <div class="tutorRequest-middle-courseInfo" v-if="isReady">
    <v-form v-model="validRequestTutorForm" ref="tutorRequestForm" :class="{'tutorProfile':isTutor}">
            <fieldset class="fieldset-textArea mb-6 px-2 py-1">
                <legend v-t="'tutorRequest_type_legend'"/>
                  <v-textarea sel="free_text"
                    :rows="isMobile?6:3"
                    v-model="description"
                    :rules="[rules.required, rules.maximumChars,rules.notSpaces]"/>
            </fieldset>
            <!-- <fieldset  class="fieldset-select px-2">
                <legend v-t="'tutorRequest_select_course_placeholder'"/>
                <v-combobox sel="course_request"
                    class="text-truncate"
                    @keyup="searchCourses"
                    flat hide-no-data
                    :append-icon="''"
                    v-model="tutorCourse"
                    :rules="[rules.required]"
                    :items="suggestsCourses"/>
            </fieldset> -->
    </v-form>
        <div class="tutorRequest-bottom mt-6">
            <v-btn @click="tutorRequestDialogClose" class="tutorRequest-btn-back" color="white" depressed rounded sel="cancel_tutor_request">
                <span v-t="'tutorRequest_cancel'"/>
            </v-btn>
            <v-btn :loading="isLoading" class="tutorRequest-btn-next" depressed rounded sel="submit_tutor_request"
             @click="!isLoggedIn? next() : sumbit()" color="#4452fc">
                <span v-if="!isLoggedIn" v-t="'tutorRequest_next'"/> 
                <span v-else v-t="'tutorRequest_submit'"/>
            </v-btn>
        </div>
    </div>
</template>

<script>
import {validationRules} from '../../../services/utilities/formValidationRules.js'
import courseService from '../../../services/courseService.js'
import debounce from "lodash/debounce";
import analyticsService from '../../../services/analytics.service'
import { mapActions, mapGetters } from 'vuex';
import * as componentConsts from '../../pages/global/toasterInjection/componentConsts.js';

export default {
    name: 'tutorRequestCourseInfo',
    data() {
        return {
            isReady:false,
            isFromMounted: false,
            isFromQuery:false,
            isLoading:false,
            validRequestTutorForm: false,
            description:'',
            tutorCourse: '',
            suggestsCourses: [],
            rules: {
                maximumChars: (value) => validationRules.maximumChars(value, 255),
                notSpaces: (value) => validationRules.notSpaces(value),
                required: (value) => validationRules.required(value),
            },
        }
    },
    computed: {
        ...mapGetters(['getCourseDescription',
                       'getSelectedCourse',
                       'accountUser',
                       'getCurrTutor',
                       'getTutorRequestAnalyticsOpenedFrom']),
        isLoggedIn(){
            return !!this.accountUser
        },
        isTutor(){
            return !!this.getCurrTutor
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }
    },
    methods: {
        ...mapActions(['updateRequestDialog',
                       'updateCourseDescription',
                       'updateSelectedCourse',
                       'resetRequestTutor',
                       'sendTutorRequest']),
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
            this.resetRequestTutor()
        },
        searchCourses: debounce(function(ev){
            this.isFromMounted = false;
            let term = ev.target.value.trim()
            if(!term) {
                this.tutorCourse = ''
                this.suggestsCourses = []
                return 
            }
            if(!!term){
                courseService.getCourse({term}).then(data=>{
                    this.suggestsCourses = data;
                    if(this.suggestsCourses.length) {
                        this.suggestsCourses.forEach(course=>{
                            if(course.text === this.tutorCourse){
                                this.tutorCourse = course
                                this.updateSelectedCourse(this.tutorCourse)
                            }}) 
                    } else {
                        this.tutorCourse = term
                        this.updateSelectedCourse(this.tutorCourse)
                    }
                })
            }
        },300),
        next(){
            if(this.$refs.tutorRequestForm.validate()){
                this.updateCourseDescription(this.description)
                this.updateSelectedCourse(this.tutorCourse)
                this.updateRequestDialog(false);
                
                this.$store.commit('setIsFromTutorStep', true)
                this.$store.commit('setComponent', 'register')
                let analyticsObject = {
                    userId: this.isLoggedIn ? this.accountUser.id : 'GUEST',
                    course: this.tutorCourse,
                    fromDialogPath: this.getTutorRequestAnalyticsOpenedFrom.path,
                    fromDialogComponent: this.getTutorRequestAnalyticsOpenedFrom.component
                };
                analyticsService.sb_unitedEvent('Request Tutor Next', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
            }
        },
        sumbit(){
            if(this.$refs.tutorRequestForm.validate()){
                this.isLoading = true;
                let tutorId;
                if(this.getCurrTutor) {
                    tutorId = this.getCurrTutor.userId || this.getCurrTutor.id
                }
            let analyticsObject = {
                userId: this.isLoggedIn ? this.accountUser.id : 'GUEST',
                course: this.tutorCourse,
                fromDialogPath: this.getTutorRequestAnalyticsOpenedFrom.path,
                fromDialogComponent: this.getTutorRequestAnalyticsOpenedFrom.component
            };
                let serverObj = {
                    captcha: null,
                    text: this.description,
                    name: null,
                    email: null,
                    phone: null,
                    course: this.tutorCourse? this.tutorCourse.text || this.tutorCourse : null,
                    tutorId: tutorId,
               
                }                    
                let self = this;
                this.sendTutorRequest(serverObj)
                    .catch(()=>{
                        self.$store.commit('addComponent',componentConsts.WENT_WRONG)
                    })
                    .finally(()=>{
                        this.isLoading = false;
                        analyticsService.sb_unitedEvent('Request Tutor Submit', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
                        this.updateSelectedCourse(this.tutorCourse)
                    })
            }
        },
    },
    mounted() {
        if(this.getCourseDescription){
            this.description = this.getCourseDescription;
        }
        if(this.getSelectedCourse){
            this.tutorCourse = this.getSelectedCourse;
        }
        if(this.$route.params && this.$route.params.course){
            let queryCourse = this.$route.params.course;
            this.tutorCourse = queryCourse
            this.isFromQuery = true;
            this.searchCourses(queryCourse)
            this.isReady = true;
        }
        if((this.$route.query && this.$route.query.Course) || (!!this.getSelectedCourse && this.getSelectedCourse.text)){
            let queryCourse;
            if(this.$route.query && this.$route.query.Course){
                queryCourse = this.$route.query.Course;
            }
            if(!!this.getSelectedCourse && this.getSelectedCourse.text){
                this.tutorCourse = this.getSelectedCourse;
                this.isFromMounted = true;
                this.isReady = true;
                return
            }
            this.tutorCourse = queryCourse
            this.isFromQuery = true;
            this.searchCourses(queryCourse)
            this.isReady = true;
        } else{
            this.isReady = true
        }
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .tutorRequest-middle-courseInfo{
        width: 100%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: space-between;
      //  height: 100%;
        form{
            width: 100%;
            margin-top: 34px;
            @media (max-width: @screen-xs) {
                margin-top: 40px;
            }
        }
        .tutorProfile{
            margin-top: 6px;
            @media (max-width: @screen-xs) {
                margin-top: 26px;
            }
            .v-input__slot{
                margin-bottom: 8px;
            }
            label{
                font-size: 14px;
                letter-spacing: -0.3px;
                color:@global-purple;
            }
        }
    .tutorRequest-bottom{
        display: flex;
        justify-content: center;
        .v-btn{
            @media (max-width: @screen-xs) {
              min-width: 120px;  
            }
            min-width: 140px;
            height: 40px !important;
            padding: 0 32px !important;
            text-transform: capitalize  !important;
        }
        .tutorRequest-btn-back{
            margin: 6px 8px;
            color: @global-blue;
            font-weight: 600;
            border: 1px solid @global-blue !important;
        }
        .tutorRequest-btn-next{
            margin: 6px 8px;
            color: white !important;
            font-size: 14px;
            font-weight: 600;
            letter-spacing: -0.3px;
        }
    }
        fieldset{
            width: 100%;
            border-radius: 6px;
            border: 1px #b8c0d1 solid;

            legend{
                padding: 0 4px;
                font-size: 12px;
                letter-spacing: -0.26px;
                color:@global-purple;
            }
            &.fieldset-textArea{
                height: 100px;
            @media (max-width: @screen-xs) {
                height: 144px;
            }
                font-size: 16px;
                .v-input{
                    margin: 0;
                    padding: 0;
                    .v-input__slot{
                        @-moz-document url-prefix(){
                            margin-bottom: 0;
                            @media (max-width: @screen-xs) {
                                margin-bottom: 8px;
                            }
                        }
                    }
                }
                .v-text-field__details{
                    @media (max-width: @screen-xs) {
                        padding-top: 0;
                    }
                    @-moz-document url-prefix(){
                       padding-top: 0;
                    }
                    font-weight: 600;
                    height: -webkit-fill-available;
                    .v-messages__message{
                        @-moz-document url-prefix(){
                            line-height: 1.5;
                        }
                    }
                }
                .v-text-field>.v-input__control>.v-input__slot:before {
                    border-style: none;
                }
                .v-text-field>.v-input__control>.v-input__slot:after {
                    border-style: none;
                }
                textarea{
                    width: 100%;
                    resize: none;
                    outline: none;
                    color:@global-purple;
                    font-weight: 600;
                    padding-left: 4px;
                    padding-top: 4px;
                }
                ::placeholder{
                    color: @global-placeholder;
                    font-weight: normal;
                }
            }
            &.fieldset-select{
                height: 58px;
                @media (max-width: @screen-xs) {
                    margin-top: 26px;
                }
                .v-text-field__details{
                    padding-top: 12px;
                    height: -webkit-fill-available;
                    @-moz-document url-prefix(){
                        height: 100px;
                    }
                }
                .v-text-field>.v-input__control>.v-input__slot:before {
                    border-style: none;
                }
                .v-text-field>.v-input__control>.v-input__slot:after {
                    border-style: none;
                }
                ::placeholder{
                    color: @global-placeholder;
                    font-weight: normal;
                }
                .v-input{
                    margin: 0;
                    font-weight: 600;
                    padding: 2px 0 0 4px;

                    .v-input__control {
                        .v-input__slot{
                            margin: 0;
                            padding: 0;
                            .v-select__slot{
                                input{
                                    .giveMeEllipsisOne();
                                }
                            }
                            .v-input__append-inner{
                                .v-input__icon{
                                    i{
                                        color:@global-purple;
                                    }
                                }
                            }
                        }
                    }
                }  
            }
        }
    }

</style>