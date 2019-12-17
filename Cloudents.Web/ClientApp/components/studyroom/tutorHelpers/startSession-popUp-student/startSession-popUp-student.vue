<template>
    <div class="student-start-wrap pb-5">
        <v-layout class="pt-2">
            <v-flex xs12 class="text-xs-right px-3">
                <v-icon class="caption cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout column align-center>
            <v-flex xs12 class="pt-2">
            <v-progress-circular v-if="!tutorImage" :width="2" indeterminate v-bind:size="35" color="#514f7d"/>
            <userAvatar v-else :user-name="tutorName" :userImageUrl="tutorImage" :user-id="tutorId" :size="'58'"/>
            </v-flex>
            <v-flex xs12 class="pt-12">
                <span class="subtitle-1 font-weight-bold" v-language:inner>tutor_start_dialog_your_tutor</span>
                <span class="subtitle-1 font-weight-bold">&nbsp;{{tutorName}}</span>
            </v-flex>
            <v-flex xs12 style="text-align: center;" class="pt-2">
                <span class="subtitle-1" v-language:inner>tutor_entered_room</span>
            </v-flex>
            <v-flex v-if="showButton" xs12 class="pt-4">
                <v-btn class="start-session-btn elevation-0 align-center justify-center"
                        large
                        :loading="getSessionStartClickedOnce"
                        :disabled="buttonState"
                        @click="joinSession()">
                    <timerIcon class="timer-icon mr-2"></timerIcon>
                    <!-- <span class="text-uppercase" v-language:inner="'tutor_btn_accept_and_start'"></span> -->
                    <span class="text-uppercase">{{roomStateText}}</span>
                </v-btn>
            </v-flex>
            <v-flex class="font-weight-bold start-session-text text-uppercase" v-else>
                {{roomStateText}}
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters, mapState } from 'vuex';
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
    import timerIcon from '../../images/timer.svg';
    import videoStreamService from "../../../../services/videoStreamService";
    import {LanguageService} from "../../../../services/language/languageService";

    export default {
        name: "startSession-popUp-student",
        components: {userAvatar, timerIcon},
        data() {
            return {
                clickedOnce: false
            };
        },
        props: {
            id: {
                required: true
            },
        },
        computed: {
            ...mapGetters(['getStudyRoomData', 'getSessionStartClickedOnce', 'getCurrentRoomState', 'getStudentDialogState']),
            ...mapState(['tutoringMainStore', 'tutoringMain']),
            tutorName() {
                return this.getStudyRoomData.tutorName;
            },
            tutorImage() {
                return this.getStudyRoomData.tutorImage;
            },
            tutorId() {
                return this.getStudyRoomData.tutorId;
            },
            isReady(){
                return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.ready;
            },
            showButton(){
                let statesToShow = [
                    this.tutoringMain.startSessionDialogStateEnum.start,
                    this.tutoringMain.startSessionDialogStateEnum.waiting,
                    this.tutoringMain.startSessionDialogStateEnum.needPayment
                ]
                return statesToShow.indexOf(this.getStudentDialogState) > -1
            },
            roomStateText(){
                if(this.getStudentDialogState === this.tutoringMain.startSessionDialogStateEnum.start){
                    return LanguageService.getValueByKey('tutor_stream_btn_start_session_student')
                }else if(this.getStudentDialogState === this.tutoringMain.startSessionDialogStateEnum.waiting){
                    return LanguageService.getValueByKey('tutor_stream_btn_waiting_student')
                }else if(this.getStudentDialogState === this.tutoringMain.startSessionDialogStateEnum.needPayment){
                    return LanguageService.getValueByKey('tutor_stream_btn_needPayment_student')
                }else if(this.getStudentDialogState === this.tutoringMain.startSessionDialogStateEnum.disconnected){
                    return LanguageService.getValueByKey('tutor_stream_btn_disconnected_student')
                }else if(this.getStudentDialogState === this.tutoringMain.startSessionDialogStateEnum.finished){
                    return LanguageService.getValueByKey('tutor_stream_btn_finished_student')
                }else{
                    return LanguageService.getValueByKey('tutor_stream_btn_waiting_student')
                }
            },
            buttonState() {
                return this.getStudentDialogState !== this.tutoringMain.startSessionDialogStateEnum.start && this.getStudentDialogState !== this.tutoringMain.startSessionDialogStateEnum.needPayment
            }
        },
        methods: {
            ...mapActions(['updateStudentStartDialog', 'setSesionClickedOnce', 'UPDATE_SEARCH_LOADING']),
            joinSession() {
                videoStreamService.enterRoom();
            },
            closeDialog() {
                let isExit = confirm(LanguageService.getValueByKey("login_are_you_sure_you_want_to_exit"),)
                if(isExit){
                    this.UPDATE_SEARCH_LOADING(true);
                    this.$router.push('/');
                }
            }
        },
        beforeDestroy(){
            this.setSesionClickedOnce(false);
            global.onbeforeunload = function() {}
        }
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .pt-12 {
        padding-top: 12px;
    }

    .student-start-wrap {
        @BtnBackground: #ffc739;
        background: @color-white;
        border-radius: 4px;
        box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
        width: 356px;
        padding: 0 5px;
        .start-session-btn {
            display: flex;
            height: 48px;
            width: 210px;
            color: @color-white;
            background-color: @BtnBackground!important;
            border-radius: 4px;
            letter-spacing: inherit;
            .timer-icon {
                fill: @color-white;
                max-width: 24px;
            }
        }
        .start-session-text{
            text-align: center;
        }
    }

</style>