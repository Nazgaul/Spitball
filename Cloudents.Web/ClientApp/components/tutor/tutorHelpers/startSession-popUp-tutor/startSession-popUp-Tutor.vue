<template>
    <div class="tutor-start-wrap pb-5">
        <v-layout row class="pt-2">
            <v-flex xs12 sm12 md12 class="text-xs-right px-3">
                <v-icon class="caption cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
    <v-layout column align-center>
        <v-flex xs12 sm12 md12 class="pt-2">
            <userAvatar :user-name="studentName"  :userImageUrl="studentImage" :user-id="studentId" :size="'58'" ></userAvatar>
        </v-flex>
        <v-flex xs12 sm12 md12 class="pt-12">
            <span class="subheading font-weight-bold" v-language:inner>tutor_start_dialog_your_tutor</span>
            <span class="subheading font-weight-bold">Tony Stark</span>
        </v-flex>
        <v-flex xs12 sm12 md12 class="pt-2">
            <span class="subheading" v-language:inner>tutor_entered_room</span>
        </v-flex>
        <v-flex xs12 sm12 md12 class="pt-4">
            <button class="start-session-btn elevation-0 align-center justify-center">
                <timerIcon class="timer-icon mr-2"></timerIcon>
                <span class="text-uppercase" v-language:inner>tutor_btn_accept_and_start</span>
            </button>
        </v-flex>
    </v-layout>
    </div>
</template>

<script>
    import {mapGetters, mapActions} from 'vuex';
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
    import timerIcon from '../../images/timer.svg';
    export default {
        name: "startSession-popUp-tutor",
        components: {userAvatar, timerIcon},
        data() {
            return {};
        },
        computed: {
            ...mapGetters(['getStudyRoomData']),
            studentName() {
                return this.getStudyRoomData.studentName;
            },
            studentImage(){
                return this.getStudyRoomData.studentImage;
            },
            studentId(){
                return this.getStudyRoomData.studentId;
            }
        },
        methods: {
            ...mapActions(['updateTutorStartDialog']),
            closeDialog() {
                this.updateTutorStartDialog(false)
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
        @greenBtnBackground: #00d983;
        background: @color-white;
        border-radius: 4px;
        box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
        width: 356px;
        .start-session-btn{
            display: flex;
            height: 48px;
            width: 210px;
            color: @color-white;
            background-color:  @greenBtnBackground;
            border-radius: 4px;
            .timer-icon{
                fill: @color-white;
                max-width: 24px;
            }
        }
    }

</style>