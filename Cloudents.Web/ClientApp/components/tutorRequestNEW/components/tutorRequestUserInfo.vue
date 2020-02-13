<template>
    <div class="tutorRequest-middle-userInfo">
        <div class="tR-span" v-html="$Ph('tutorRequest_tell_tutor',getCurrTutor? getCurrTutor.name : generalName)"/>
    <v-form v-model="validRequestTutorForm" ref="tutorRequestForm">
            <fieldset class="fieldset-user-name px-2">
                <legend v-language:inner="'tutorRequest_name'"/>
                <v-text-field 
                    :rules="[rules.required,rules.notSpaces]" 
                    v-model="guestName" 
                    class="userName"
                    autocomplete="off"/>
            </fieldset>

            <v-layout justify-space-between class="userInfo-bottom mb-2">
                <v-flex md8 xs12>
                <fieldset class="fieldset-user-email px-2">
                    <legend v-language:inner="'tutorRequest_email'"/>
                    <v-text-field 
                        :rules="[rules.required, rules.email,rules.notSpaces]" 
                        v-model="guestMail" type="email" 
                        class="userEmail"
                        autocomplete="off"/>
                </fieldset>    
                </v-flex>
                <v-flex md4 xs12>
                    <fieldset class="fieldset-user-phone px-2">
                    <legend v-language:inner="'tutorRequest_phoneNumber'"/>
                    <v-text-field 
                        :rules="[rules.required,rules.phone,rules.notSpaces]"
                        type="tel"
                        maxlength="13"
                        autocomplete="off"
                        v-model="guestPhone" 
                        class="userPhone"/>
                </fieldset>
                </v-flex>
            </v-layout>
        <vue-recaptcha  
              size="invisible"
              class="pb-md-4"
              :sitekey="siteKey"
              ref="recaptcha"
              @verify="onVerify"
              @expired="onExpired()">
        </vue-recaptcha>
    </v-form>

        <div class="tutorRequest-bottom">
            <v-btn @click="goBack" class="tutorRequest-btn-back" color="white" depressed rounded>
                <span v-language:inner="'tutorRequest_back'"/>
            </v-btn>
            <v-btn :loading="isLoading" @click="submit(!isAuthUser)" class="tutorRequest-btn-next" depressed rounded color="#4452fc" sel="submit_tutor_request">
                <span v-language:inner="'tutorRequest_send'"/>
            </v-btn>
        </div>
    </div>
</template>

<script>
import {LanguageService} from '../../../services/language/languageService.js'
import {validationRules} from '../../../services/utilities/formValidationRules.js'
import { mapActions,mapGetters } from 'vuex';
import VueRecaptcha from 'vue-recaptcha';
import analyticsService from '../../../services/analytics.service'

export default {
    name:'tutorRequestUserInfo',
    components:{VueRecaptcha},
    data() {
        return {
            siteKey: '6LfyBqwUAAAAAM-inDEzhgI2Cjf2OKH0IZbWPbQA',
            recaptcha: '',
            validRequestTutorForm: false,
            guestName: '',
            guestMail: '',
            guestPhone: '',
            rules: {
                required: (value) => validationRules.required(value),
                email: (value) => validationRules.email(value),
                phone: (value) => validationRules.phoneValidate(value),
                notSpaces: (value) => validationRules.notSpaces(value),
            },
            isLoading: false,
            generalName: LanguageService.getValueByKey("tutorRequest_yaniv"),
        }
    },
    computed: {
        ...mapGetters(['getTutorRequestAnalyticsOpenedFrom','getCourseDescription','getSelectedCourse','accountUser','getCurrTutor','getMoreTutors', 'getGuestName', 'getGuestMail', 'getGuestPhone']),
        isAuthUser(){
            return !!this.accountUser;
        },
    isMobile(){
      return this.$vuetify.breakpoint.smAndDown;
    },
    },
    methods: {
        ...mapActions(['updateTutorReqStep','sendTutorRequest', 'setGuestName', 'setGuestMail', 'setGuestPhone']),
        goBack() {
            this.setGuestName(this.guestName);
            this.setGuestMail(this.guestMail);
            this.setGuestPhone(this.guestPhone);
            this.updateTutorReqStep('tutorRequestCourseInfo')
        },
        submit(guest){
            if(this.$refs.tutorRequestForm.validate()){
                if(guest){
                    this.$refs['recaptcha'].execute()
                }else{
                    this.sendRequest();
                }
            }
        },
        sendRequest(){
            let self = this
            if(this.$refs.tutorRequestForm.validate()){
                self.isLoading = true
                let tutorId;
                if(this.getCurrTutor) {
                    tutorId = this.getCurrTutor.userId || this.getCurrTutor.id
                }
            let analyticsObject = {
                userId: this.isAuthUser ? this.accountUser.id : 'GUEST',
                course: this.getSelectedCourse,
                fromDialogPath: this.getTutorRequestAnalyticsOpenedFrom.path,
                fromDialogComponent: this.getTutorRequestAnalyticsOpenedFrom.component
            };

                
                
                let serverObj = {
                    captcha: (self.recaptcha)? self.recaptcha : null,
                    text: (self.getCourseDescription)? self.getCourseDescription : null,
                    name: (self.guestName)? self.guestName : null,
                    email: (self.guestMail)? self.guestMail : null,
                    phone: (self.guestPhone)? self.guestPhone : null,
                    course: (self.getSelectedCourse)? self.getSelectedCourse.text || self.getSelectedCourse : null,
                    moreTutors: self.getMoreTutors,
                    tutorId: tutorId
                }
                this.sendTutorRequest(serverObj).finally(()=>{
                    self.isLoading= false
                    analyticsService.sb_unitedEvent('Request Tutor Submit', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
                  if(self.$refs['recaptcha']){
                      self.$refs['recaptcha'].reset();
                    }
                })
            } 
        },
        onVerify(response) {
            this.recaptcha = response;
            this.sendRequest()
            this.$refs['recaptcha'].reset();
        },
        onExpired() {
            this.recaptcha = "";
            this.$refs['recaptcha'].reset();
        },
    },
    mounted(){
        this.guestName = this.getGuestName;
        this.guestMail = this.getGuestMail;
        this.guestPhone = this.getGuestPhone;
    },
    created() {
         let captchaLangCode = global.lang === 'he' ? 'iw' : 'en';
        this.$loadScript(`https://www.google.com/recaptcha/api.js?onload=vueRecaptchaApiLoaded&render=explicit&hl=${captchaLangCode}`);
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .tutorRequest-middle-userInfo{
        width: 100%;
        //display: flex;
        //flex-direction: column;
        //align-items: center;
        //justify-content: space-between;
       // height: 100%;
        @media (max-width: @screen-xs) {
            margin-top: 8px;
            letter-spacing: -0.38px;
        }
        fieldset{
            // width: 100%;
            border-radius: 4px;
            border: 1px #b8c0d1 solid;
            height: 58px;
            .v-text-field__details{
                padding-top: 4px;
            }
            .v-text-field>.v-input__control>.v-input__slot:before,
             .v-text-field>.v-input__control>.v-input__slot:after {
                border-style: none;
            }
            legend{
                padding: 0 4px;
                font-size: 12px;
                letter-spacing: -0.26px;
            }
        }
        form{
            margin-bottom: 8px;
            width: 100%;
            @media (max-width: @screen-xs) {
                height: 100%;
                margin-top: 30px;
                margin-bottom: 36px;
            }
            .v-input{
                padding-left: 4px!important;
            }
        }
        .tR-span{
                font-size: 16px;
                color:@global-purple;
                margin: 0 auto;
                margin-bottom: 26px;
                text-align: center;
                @media (max-width: @screen-xs) {
                    margin-bottom: 36px;
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
            padding: 0px 32px !important;
            text-transform: capitalize  !important;
        }
        .tutorRequest-btn-back{
            margin: 6px 8px;
            color: @global-blue;
            border: 1px solid @global-blue !important;
        }
        .tutorRequest-btn-next{
            margin: 6px 8px;
            color: white !important;
            font-size: 16px;
            font-weight: 600;
            letter-spacing: -0.3px;
        }
    }
        .fieldset-user-name{
            margin-bottom: 26px;
            padding-top: 2px;
            .userName{
                font-weight: 600;
                margin: 0;
                padding: 0;
                font-size: 16px;
                color:@global-purple;
            }
        }
        .userInfo-bottom{
            display: flex;
            width: 100%;
            @media (max-width: @screen-xs) {
                flex-direction: column;
            }
            .fieldset-user-email{
                padding-top: 2px;
                margin-right: 14px;
                .userEmail{
                    font-weight: 600;
                    width: 258px;
                    margin: 0;
                    padding: 0;
                    font-size: 16px;
                    color:@global-purple;
                }
                @media (max-width: @screen-xs) {
                    margin-right: 0;
                }
            }
            .fieldset-user-phone{
            @media (max-width: @screen-xs) {
                margin-top: 26px;
            }
            padding-top: 2px;
                .userPhone{
                    font-weight: 600;
                    margin: 0;
                    padding: 0;
                    font-size: 16px;
                    color:@global-purple;
                }
            }
        }
    }

</style>