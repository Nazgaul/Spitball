<template>
    <div class="tutor-start-wrap pb-3">
        <v-layout row class="pt-3">
            <v-flex xs12 class="text-xs-right px-3">
                <v-icon class="caption cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
    <v-layout column align-center>
        <v-flex xs12 class="pt-2">
            <v-progress-circular v-if="isStudentImage && studentImage" :width="2" indeterminate v-bind:size="35" color="#514f7d"/>
            <userAvatar v-else :user-name="studentName"  :userImageUrl="studentImage" :user-id="studentId" :size="'58'"/>
        </v-flex>
        <v-flex xs12   class="pt-12">
            <span class="subheading font-weight-bold" v-language:inner>tutor_start_dialog_your_student</span>
            <span class="subheading font-weight-bold">&nbsp;{{studentName}}</span>
        </v-flex>
        <v-flex xs12  style="text-align: center;" class="pt-2">
            <span class="subheading" v-language:inner>tutor_entered_room</span>
        </v-flex>
        <v-flex xs12   class="pt-4">
            <v-btn class="start-session-btn elevation-0 align-center justify-center"
                    :loading="buttonLoader"
                    :disabled="buttonState"
                    @click="startSession()">
                <timerIcon class="timer-icon mr-2"></timerIcon>
                <span class="text-uppercase">{{buttonText}}</span>
            </v-btn>
        </v-flex>
    </v-layout>
    </div>
</template>

<script>
    import {mapGetters, mapActions, mapState} from 'vuex';
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
    import timerIcon from '../../images/timer.svg';
    import videoStreamService from "../../../../services/videoStreamService";
    import {LanguageService} from "../../../../services/language/languageService";
    export default {
        name: "startSession-popUp-tutor",
        components: {userAvatar, timerIcon},
        props: {
            id: {
                required:true
            },
        },
        computed: {
            ...mapState(['tutoringMain']),
            ...mapGetters(['getStudyRoomData', 'getSessionStartClickedOnce', 'getTutorDialogState']),
            studentName() {
                return this.getStudyRoomData.studentName;
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
            buttonText(){
                if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.start){
                    return LanguageService.getValueByKey('tutor_stream_btn_start_tutor')
                }else if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.waiting){
                    return LanguageService.getValueByKey('tutor_stream_btn_waiting_tutor')
                }else if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.needPayment){
                    return LanguageService.getValueByKey('tutor_stream_btn_needPayment_tutor')
                }else{
                    return LanguageService.getValueByKey('tutor_stream_btn_start_tutor')
                }
            },
            buttonState() {
                return this.getTutorDialogState !== this.tutoringMain.startSessionDialogStateEnum.start
            }
        },
        methods: {
            ...mapActions(['updateTutorStartDialog', 'setSesionClickedOnce', 'UPDATE_SEARCH_LOADING']),
            startSession(){
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
    .pt-12{
        padding-top: 12px;
    }
    .tutor-start-wrap{
        @BtnBackground: #ffc739;
        background: @color-white;
        border-radius: 4px;
        box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
        width: 356px;
        .start-session-btn{
            display: flex;
            height: 48px;
            width: 210px;
            color: @color-white;
            background-color: @BtnBackground!important;
            border-radius: 4px;
            .timer-icon{
                fill: @color-white;
                max-width: 24px;
            }
        }
    }

</style>