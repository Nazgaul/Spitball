<template>
    <div class="tutorRequest-success">
        <v-icon @click="tutorRequestDialogClose" class="uf-close" v-html="'sbf-close'" />
        <div class="tutorRequest-success-middle">
            <img class="success-img" src="../images/success.png" alt="" />
            <template v-if="isTutor">
                <p class="message_1">
                    <span class="sentMessage" v-t="'tutorRequest_message_success_tutor_1'"></span>
                    <span>{{firstName}}</span>
                </p>
                <div class="message_3">
                    <p>{{$t('tutorRequest_message_success_tutor_3',[firstName])}}</p>
                    <p><bdi>{{tutorPhoneNumber}}</bdi></p>
                </div>
            </template>
            <template v-else>
                <p class="message_1" v-t="'tutorRequest_message_success_yaniv_1'"></p>
                <p class="message_2" v-t="'tutorRequest_message_success_yaniv_2'"></p>
                <p class="message_3" v-t="'tutorRequest_message_success_yaniv_3'"></p>
            </template>
        </div>

        <div class="tutorRequest-success-bottom">
            <v-btn sel="cancel_tutor_request" @click="tutorRequestDialogClose" class="tutorRequest-btn-back" color="white" depressed rounded>
                <span>{{btnText}}</span>
            </v-btn>
            <template v-if="!isTutor">
                <v-btn @click="showMoreTutors" sel="show_more_tutors" class="tutorRequest-btn-showme white--text" color="#4452fc" depressed rounded>
                    <span v-t="'tutorRequest_message_success_btn_showMe'"></span>
                </v-btn>
            </template>
        </div>
        
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import analyticsService from '../../../services/analytics.service';

export default {
    name:'tutorRequestSuccess',
    computed:{
        ...mapGetters(['accountUser', 'getCurrTutor', 'getSelectedCourse', 'getCurrentTutorPhoneNumber', 'getCourseDescription', 'getMoreTutors']),
        btnText() {
            return this.isTutor ? this.$t('tutorRequest_close') : this.$t('tutorRequest_message_success_btn_noThanks')
        },
        isTutor(){
            return !!this.getCurrTutor;
        },
        courseName() {
            return this.getSelectedCourse?.text;
        },
        tutorPhoneNumber() {
            return this.getCurrentTutorPhoneNumber;
        },
        tutorId() {
            return this.getCurrTutor?.userId;
        },
        studentId() {
            return this.accountUser?.id;
        },
        firstName() {
            return this.getCurrTutor?.name.split(' ')[0];
        }
    },
    methods: {
        ...mapActions(['updateRequestDialog', 'resetRequestTutor', 'sendTutorRequest']),
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
            this.resetRequestTutor()
        },
        showMoreTutors() {
            if(!this.accountUser) {
                analyticsService.sb_unitedEvent('After tutor Submit','Show me btn', `Student GUEST`);
            } else {
                analyticsService.sb_unitedEvent('After tutor Submit','Show me btn', `Student ${this.studentId}`);
            }
            this.$router.push({name: 'tutorLandingPage', params: {course: this.courseName}});
            this.tutorRequestDialogClose();
        },
        requestTutorFromRegisterStep() {
            // if there is an account, it's mean that the user not coming from register and dont need to call requestTutor api twice
            if(this.accountUser) return
            
            let serverObj = {
                captcha: null,
                text: this.getCourseDescription,
                name: null,
                email: null,
                phone: null,
                course: this.courseName || this.getSelectedCourse,
                tutorId: this.tutorId,
                moreTutors: this.getMoreTutors
            } 
            let self = this;
            this.sendTutorRequest(serverObj)
                .catch(err=>{
                    let serverResponse = err.response.data || { error : [self.$t('tutorRequest_request_error')]};
                    let errorMsg = serverResponse[Object.keys(serverResponse)[0]][0];
                    self.$store.dispatch('updateToasterParams',{
                        toasterText: errorMsg,
                        showToaster: true,
                        toasterType: 'error-toaster'
                    });
                })
        }
    },
    mounted() {
        this.requestTutorFromRegisterStep()
    }
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
            //align-items: center; 
            //justify-content: center;
            //@media (max-width: @screen-xs) {
            //  padding: 0;
            //}
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
    }
}
</style>