<template>
    <div class="tutor-start-wrap pb-4">
        <v-layout class="pt-4">
            <v-flex xs12 class="text-right px-4">
                <v-icon class="body-2 cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout column align-center>
            <span v-if="$store.getters.getRoomIsBroadcast && !isRoomNow">{{time.days}}:{{time.hours}}:{{time.minutes}}:{{time.seconds}}</span>
            <v-flex xs12 style="text-align: center;" class="pt-2">
                <span v-language:inner="'tutor_can_be_recorded'"></span>
            </v-flex>
            <v-flex xs12 class="pt-5">
                <v-btn height="48" class="start-session-btn ma-2 elevation-0 align-center justify-center"
                        large
                        :disabled="!isRoomNow"
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
                isRoomNow:false,
                interVal:null,
                time:{
                    days:'00',
                    hours:'00',
                    minutes:'00',
                    seconds:'00'
                }
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
            ...mapGetters(['getSessionStartClickedOnce', 'getSessionTimeStart', 'getSessionTimeEnd']),
            sessionTime(){
                let end = this.getSessionTimeEnd;
                let start = this.getSessionTimeStart;
                return this.getTimeFromMs(end - start);
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
            },
            setParamsInterval(){
                this.interVal = setInterval(this.getNow, 1000);
                this.getNow();
            },
            getNow() {
                let countDownDate = new Date(this.$store.getters.getRoomDate).getTime();
                let now = new Date();
                let distance = countDownDate - now;
                
                const second = 1000;
                const minute = second * 60;
                const hour = minute * 60;
                const day = hour * 24;
                

                this.time.days = Math.floor(distance / (day)).toLocaleString('en-US', {minimumIntegerDigits: 2});
                this.time.hours = Math.floor((distance % (day)) / (hour)).toLocaleString('en-US', {minimumIntegerDigits: 2});
                this.time.minutes = Math.floor((distance % (hour)) / (minute)).toLocaleString('en-US', {minimumIntegerDigits: 2});
                this.time.seconds = Math.floor((distance % (minute)) / second).toLocaleString('en-US', {minimumIntegerDigits: 2});
                if (distance < 0) {
                    clearInterval(this.interVal);
                    this.isRoomNow = true;
                }
            }
        },
        beforeDestroy(){
            this.isLoading = false;
            this.setSesionClickedOnce(false);
            global.onbeforeunload = function() {}
        },
        created() {
            if(this.$store.getters.getRoomIsBroadcast){
                this.setParamsInterval();
            }else{
                this.isRoomNow = true;
            }
        },
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