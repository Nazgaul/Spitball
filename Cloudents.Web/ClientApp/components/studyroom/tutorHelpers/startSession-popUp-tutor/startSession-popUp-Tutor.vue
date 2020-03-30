<template>
    <div class="tutor-start-wrap pb-4">
        <v-layout class="pt-4">
            <v-flex xs12 class="text-right px-4">
                <v-icon class="body-2 cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout column align-center>
            <!-- <v-flex xs12 class="pt-2">
                <userAvatar :user-name="studentName"  :userImageUrl="studentImage" :user-id="studentId" :size="'58'"/>
            </v-flex>
            <v-flex xs12 class="pt-3 px-5 text-truncate">
                <span class="tutor-start-wrap_title font-weight-bold" v-language:inner>tutor_start_dialog_your_student</span>
                <span class="tutor-start-wrap_title font-weight-bold">&nbsp;{{studentName}}</span>
            </v-flex> -->
            <v-flex xs12 style="text-align: center;" class="pt-2">
                <span v-language:inner="'tutor_can_be_recorded'"></span>
            </v-flex>
            <v-flex xs12 class="pt-5">
                <v-btn height="48" class="start-session-btn ma-2 elevation-0 align-center justify-center"
                        large
                        :loading="isLoading"
                        @click="startSession()">
                    <timerIcon class="timer-icon mr-2"></timerIcon>
                    <span v-t="'tutor_stream_btn_start_tutor'"></span>
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import {mapGetters, mapActions, mapState} from 'vuex';
    import timerIcon from '../../images/timer.svg';
    import {LanguageService} from "../../../../services/language/languageService";
    export default {
        name: "startSession-popUp-tutor",
        data() {
            return {
                isLoading:false,
            }
        },
        components: {timerIcon},
        props: {
            id: {
                required:true
            },
        },
        computed: {
            ...mapState(['tutoringMain']),
            ...mapGetters(['getSessionStartClickedOnce', 'getTutorDialogState', 'getSessionTimeStart', 'getSessionTimeEnd']),
            // roomStudent(){
            //     return this.$store.getters.getRoomStudent;
            // },
            
            // studentName() {
            //     return this.roomStudent?.studentName;
            // },
            sessionTime(){
                let end = this.getSessionTimeEnd;
                let start = this.getSessionTimeStart;
                return this.getTimeFromMs(end - start);
            },
            // studentImage(){
            //     return this.roomStudent?.studentImage;
            // },
            // studentId(){
            //     return this.roomStudent?.studentId;
            // },
            // roomStateText(){
            //     return LanguageService.getValueByKey('tutor_stream_btn_start_tutor')

            //     // if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.start){
            //     //     return LanguageService.getValueByKey('tutor_stream_btn_start_tutor')
            //     // }else if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.waiting){
            //     //     return LanguageService.getValueByKey('tutor_stream_btn_waiting_tutor')
            //     // }else if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.disconnected){
            //     //     return LanguageService.getValueByKey('tutor_stream_btn_disconnected_tutor')
            //     // }else if(this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.finished){
            //     //     return LanguageService.getValueByKey('tutor_stream_btn_finished_tutor')
            //     // }else{
            //     //     return LanguageService.getValueByKey('tutor_stream_btn_start_tutor')
            //     // }
            // },
            sessionFinished(){
                return this.getTutorDialogState === this.tutoringMain.startSessionDialogStateEnum.finished;
            },
        },
        methods: {
            ...mapActions(['setSesionClickedOnce']),
            startSession(){
                this.isLoading = true;
                this.$store.dispatch('updateEnterRoom',this.id)
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
            this.isLoading = false;
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