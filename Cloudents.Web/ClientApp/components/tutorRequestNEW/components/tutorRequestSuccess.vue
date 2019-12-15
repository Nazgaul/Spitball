<template>
    <div class="tutorRequest-success">
        <v-icon @click="tutorRequestDialogClose" class="uf-close" v-html="'sbf-close'" />
        <div class="tutorRequest-success-middle">
            <img class="success-img" src="../images/success.png" alt="" />
            <template v-if="isTutor">
                <p class="message_1">
                    <span class="sentMessage" v-language:inner="'tutorRequest_message_success_tutor_1'"></span>
                    <span>{{firstName}}</span>
                </p>
                <p class="message_2" v-language:inner="'tutorRequest_message_success_tutor_2'"></p>
                <p class="message_3" v-language:inner="'tutorRequest_message_success_tutor_3'"></p>
            </template>

            <template v-else>
                <p class="message_1" v-language:inner="'tutorRequest_message_success_yaniv_1'"></p>
                <p class="message_2" v-language:inner="'tutorRequest_message_success_yaniv_2'"></p>
                <p class="message_3" v-language:inner="'tutorRequest_message_success_yaniv_3'"></p>
            </template>
        </div>

        <div class="tutorRequest-success-bottom">
            <v-btn sel="cancel_tutor_request" @click="tutorRequestDialogClose" class="tutorRequest-btn-back" color="white" depressed rounded>
                <span v-language:inner="'tutorRequest_message_success_btn_noThanks'"></span>
            </v-btn>

            <template>
                <v-btn v-if="isTutor" sel="send_whatsapp" @click="sendWhatsapp" class="tutorRequest-btn-whatsapp white--text" color="#46c16b" depressed rounded>
                    <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="28" height="28" viewBox="0 0 28 28">
                        <g fill-rule="evenodd" fill="none">
                            <path d="M0 .346h27.654V28H0z"/>
                            <path fill="#FFF" d="M7.18 23.882l.417.26a11.72 11.72 0 006.231 1.8c6.5 0 11.77-5.28 11.77-11.77s-5.28-11.77-11.77-11.77-11.77 5.28-11.77 11.77c0 2.64.865 5.143 2.5 7.236l.3.37-.606 3.117 2.938-1.012zM13.827 28a13.77 13.77 0 01-6.896-1.856L1.543 28l1.108-5.705A13.67 13.67 0 010 14.172C0 6.548 6.203.346 13.827.346s13.828 6.202 13.828 13.826S21.452 28 13.827 28z"/>
                            <path fill="#FFF" d="M7.007 8.986s.812-1.45 1.475-1.532 1.515-.082 1.746.36 1.258 3.007 1.258 3.007.176.442-.095.87-.88 1.048-.88 1.048-.34.442 0 .9.863 1.332 1.95 2.44 3.166 1.894 3.166 1.894.298.04.487-.152 1.218-1.504 1.218-1.504.33-.434.88-.18l2.922 1.462s.278.104.278.53-.17 1.475-.517 1.828-1.362 1.445-2.886 1.445-5.157-1.265-7.09-3.237-3.654-3.975-4.06-5.796-.352-2.643.15-3.393"/>
                        </g>
                    </svg>
                    <span class="btn-whatsapp-text" v-language:inner="'tutorRequest_message_success_btn_whatsapp'"></span>
                </v-btn>

                <v-btn v-else @click="showMoreTutors" sel="show_more_tutors" class="tutorRequest-btn-showme white--text" color="#4452fc" depressed rounded>
                    <span v-language:inner="'tutorRequest_message_success_btn_showMe'"></span>
                </v-btn>
            </template>
        </div>
        
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import { LanguageService } from '../../../services/language/languageService';
import analyticsService from '../../../services/analytics.service';


export default {
    name:'tutorRequestSuccess',
    computed:{
        ...mapGetters(['accountUser', 'getCurrTutor', 'getSelectedCourse', 'getCurrentTutorPhoneNumber']),

        isTutor(){
            return !!this.getCurrTutor;
        },
        courseName() {
            return this.getSelectedCourse.text;
        },
        tutorPhoneNumber() {
            return this.getCurrentTutorPhoneNumber;
        },
        defaultMessage() {
            return LanguageService.getValueByKey('whatsapp_message');
        },
        tutorId() {
            return this.getCurrTutor.userId;
        },
        studentId() {
            return this.accountUser.id;
        },
        firstName() {
            return this.getCurrTutor.name.split(' ')[0];
        }
    },
    methods: {
        ...mapActions(['updateRequestDialog', 'resetRequestTutor']),
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
            this.resetRequestTutor()
        },
        sendWhatsapp() {
            if(!this.accountUser) {
                analyticsService.sb_unitedEvent('Request Tutor Submit','Whatsapp btn', `Tutor ${this.tutorId}, Student GUEST`);
            } else {
                analyticsService.sb_unitedEvent('Request Tutor Submit','Whatsapp btn', `Tutor ${this.tutorId}, Student ${this.studentId}`);
            }
            window.open(`https://api.whatsapp.com/send?phone=${this.tutorPhoneNumber}&text=%20${this.defaultMessage}`);
            this.tutorRequestDialogClose();
        },
        showMoreTutors() {
            if(!this.accountUser) {
                analyticsService.sb_unitedEvent('Request Tutor Submit','Show me btn', `Student GUEST`);
            } else {
                analyticsService.sb_unitedEvent('Request Tutor Submit','Show me btn', `Student ${this.studentId}`);
            }
            this.$router.push({name: 'tutorLandingPage', params: {course: this.courseName}});
            this.tutorRequestDialogClose();
        },
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
.tutorRequest-success{
        position: relative;
        width: 100%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: space-between;
        height: 100%;
        min-height: 100%;
        text-align: center;
        .uf-close {
            position: absolute;
            right: -10px;
            top: 10px;
            font-size: 12px;
            color: #adadba;
        }
        .tutorRequest-success-middle{
            padding: 46px 0 64px 0;
            height: 100%;
            align-items: center; 
            justify-content: center;
            @media (max-width: @screen-xs) {
              padding: 0;
            }
            .success-img{
                width: 140px;
                height: 88px;
                object-fit: contain;
            }
            p{
                margin: 0;
                padding: 0;
                font-size: 16px;
                font-weight: 600;
                line-height: 1.75;
                color: @global-purple;
                white-space: pre-line;
            }
            .message_1 {
                color: #5560ff;
                margin-top: 30px;
            }
            .message_2 {
                @media (max-width: @screen-xs) {
                    white-space: normal;
                }
            }
            .message_3 {
                    margin-top: 10px;
            }
        }
    .tutorRequest-success-bottom{
        @media (max-width: @screen-xs) {
            display: flex;
        }
        .v-btn{
            @media (max-width: @screen-xs) {
              min-width: 140px;  
            }
            min-width: 140px;
            height: 40px !important;
            padding: 0px 32px !important;
            text-transform: capitalize  !important;
            font-weight: 600;
        }
        .tutorRequest-btn-back{
            margin: 6px 8px;
            font-weight: 600;
            color: @global-blue;
            border: 1px solid @global-blue !important;
        }
        .tutorRequest-btn-showme {
            margin: 6px 8px;
        }
        .tutorRequest-btn-whatsapp {
            margin: 6px 8px;
            font-weight: normal;
            .v-btn__content {
                padding-left: 10px;
                svg {
                    left: -24px;
                    top: -5px;
                    width: 26px;
                    position: absolute;
                    @media (max-width: @screen-xs) {
                        left: -25px;
                        top: -6px;
                    }
                }
                .btn-whatsapp-text {
                    padding-left: 5px;
                }
            }
        }
    }
}
</style>