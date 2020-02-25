<template>
    <div class="tutor-start-wrap pb-4">
        <v-layout class="pt-4">
            <v-flex xs12 class="text-right px-4">
                <v-icon class="body-2 cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
    <v-layout column align-center>
        <v-flex xs12 class="pt-2">
            <v-progress-circular v-if="isStudentImage && studentImage" :width="2" indeterminate v-bind:size="35" color="#514f7d"/>
            <userAvatar v-else :user-name="studentName"  :userImageUrl="studentImage" :user-id="studentId" :size="'58'"/>
        </v-flex>
        <v-flex xs12 class="pt-3">
            <span class="tutor-start-wrap_title font-weight-bold" v-language:inner>tutor_start_dialog_your_student</span>
            <span class="tutor-start-wrap_title font-weight-bold">&nbsp;{{studentName}}</span>
        </v-flex>
        <v-flex xs12 v-if="!sessionFinished" style="text-align: center;" class="pt-2">
            <span class="" v-language:inner="'tutor_can_be_recorded'"></span>
        </v-flex>
        <v-flex xs12 v-if="showButton" class="pt-5">
            <v-btn height="48" class="start-session-btn ma-2 elevation-0 align-center justify-center"
                    large
                    :loading="buttonLoader"
                    :disabled="buttonState"
                    @click="startSession()">
                <timerIcon class="timer-icon mr-2"></timerIcon>
                <span class="">{{roomStateText}}</span>
            </v-btn>
        </v-flex>
        <v-flex class="font-weight-bold start-session-text" px-2 pt-6 v-else>
            {{roomStateText}}
        </v-flex>
        <v-flex xs12 v-if="sessionFinished" class="pt-3">
            <span class="subheading" v-language:inner="'tutor_start_dialog_session_time'"></span>
            <span class="subheading">&nbsp;{{sessionTime}}</span>
        </v-flex>
    </v-layout>
    </div>
</template>

<script>
    import {mapGetters, mapActions, mapState} from 'vuex';
    import timerIcon from '../../images/timer.svg';
    import videoStreamService from "../../../../services/videoStreamService";
    import {LanguageService} from "../../../../services/language/languageService";
    export default {
        name: "startSession-popUp-tutor",
        components: {timerIcon},
        props: {
            id: {
                required:true
            },
        },
        computed: {
            ...mapState(['tutoringMain']),
            ...mapGetters(['getStudyRoomData', 'getSessionStartClickedOnce', 'getTutorDialogState', 'getSessionTimeStart', 'getSessionTimeEnd']),
            studentName() {
                return this.getStudyRoomData.studentName;
            },
            sessionTime(){
                let end = this.getSessionTimeEnd;
                let start = this.getSessionTimeStart;
                return this.getTimeFromMs(end - start);
            },
            isStudentImage() {
                return (this.getStudyRoomData && this.getStudyRoomData.studentImage) ? false : true;
            },
            studentImage(){
                return this.getStudyRoomData.studentImage;
            },
            studentId(){
                return this.getStudyRoomData.studentId;
            },
            buttonLoader(){
                return this.getSessionStartClickedOnce && this.getTutorDialogState !== this.tutoringMain.startSessionDialogStateEnum.waiting
            },
            showButton(){
                let statesToShow = [
                    this.tutoringMain.startSessionDialogStateEnum.start,
                    this.tutoringMain.startSessionDialogStateEnum.waiting
                ]
                return statesToShow.indexOf(this.getTutorDialogState) > -1
            },
            roomStateText(){
                if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.start){
                    return LanguageService.getValueByKey('tutor_stream_btn_start_tutor')
                }else if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.waiting){
                    return LanguageService.getValueByKey('tutor_stream_btn_waiting_tutor')
                }else if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.disconnected){
                    return LanguageService.getValueByKey('tutor_stream_btn_disconnected_tutor')
                }else if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.finished){
                    return LanguageService.getValueByKey('tutor_stream_btn_finished_tutor')
                }else{
                    return LanguageService.getValueByKey('tutor_stream_btn_start_tutor')
                }
            },
            sessionFinished(){
                return this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.finished;
            },
            buttonState() {
                return this.getTutorDialogState !== this.tutoringMain.startSessionDialogStateEnum.start
            }
        },
        methods: {
            ...mapActions(['updateTutorStartDialog', 'setSesionClickedOnce']),
            startSession(){
                videoStreamService.enterRoom();
            },
            closeDialog() {
                let isExit = confirm(LanguageService.getValueByKey("login_are_you_sure_you_want_to_exit"),)
                if(isExit){
                    this.$router.push('/');
                }
            },
            getTimeFromMs(mills){
                let ms = 1000*Math.round(mills/1000); // round to nearest second
                let d = new Date(ms);
               return (`${d.getUTCHours()}:${d.getUTCMinutes()}:${d.getUTCSeconds()} `)
            }
        },
        beforeDestroy(){
            
            console.warn('DEBUG: 43 startSessionPopUpTutor: setSesionClickedOnce beforeDestroy,false before ')
            this.setSesionClickedOnce(false);
            console.warn('DEBUG: 43.1 startSessionPopUpTutor: setSesionClickedOnce beforeDestroy,false after ')

            global.onbeforeunload = function() {}
        }
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .pt-12{
        padding-top: 12px;
    }
    .tutor-start-wrap{
        @BtnBackground: #ffc739;
        background: @color-white;
        border-radius: 4px;
        box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
        width: 356px;
        padding: 0 5px;
        .tutor-start-wrap_title{
            font-size: 16px;
        }
        .start-session-btn{
            display: flex;
            height: 48px;
            width: 210px;
            color: @color-white;
            background-color: @BtnBackground!important;
            border-radius: 4px;
            letter-spacing: inherit;
            text-transform: initial;
            font-size: 14px !important;
            .timer-icon{
                fill: @color-white;
                max-width: 24px;
            }
        }
        .start-session-text{
            text-align: center;
        }
    }

</style>