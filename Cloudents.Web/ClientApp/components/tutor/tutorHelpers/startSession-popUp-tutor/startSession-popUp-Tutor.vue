<template>
    <v-layout row wrap>
        <v-flex xs12 sm12 md12 class="text-xs-right">
            <v-icon class="subheading">sbf-close</v-icon>
        </v-flex>
        <v-flex xs12 sm12 md12>
            <userAvatar :user-name="studentName"  :userImageUrl="studentImage" :user-id="studentId" :size="'58'" ></userAvatar>
        </v-flex>
        <v-flex xs12 sm12 md12>
            <span class="subheading font-weight-bold">Your tutor, Tony Stark</span>
            <span class="subheading">has entered the room</span>
        </v-flex>
        <v-flex xs12 sm12 md12>
            <v-btn class="start-session-btn">
                <timerIcon class="timer-icon"></timerIcon>
                <span class="text-uppercase">Accept and start</span>
            </v-btn>
        </v-flex>
    </v-layout>
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

</style>