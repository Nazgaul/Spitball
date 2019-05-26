<template>
    <div class="btn-wrapper">
        <v-flex v-show="isTutor">
            <button v-show="!roomIsActive && !waitingStudent " class="create-session text-capitalize" color="primary" :class="{'disabled': roomIsPending || needPayment}" @click="enterRoom()">
                <timerIcon class="timer-icon mr-2"></timerIcon>
                <span v-language:inner v-show="needPayment">tutor_stream_btn_pending_tutor</span>
                <span v-language:inner v-show="!needPayment" >tutor_stream_btn_start_session</span>
            </button>
            <button class="create-session" v-show="!roomIsActive && waitingStudent">
                <span v-language:inner>tutor_stream_btn_waiting</span>
            </button>

            <button v-show="roomIsActive && !waitingStudent" class="end-session" @click="endSession()">
                <stopIcon class="stop-icon mr-2"></stopIcon>
                <span v-language:inner>tutor_stream_btn_end_session</span>
            </button>
        </v-flex>
        <v-flex v-show="!isTutor">
            <button v-show="!roomIsActive && !waitingStudent" class="create-session" color="primary" :class="{'disabled': roomIsPending && !needPayment}" @click="enterRoom()">
                <timerIcon class="timer-icon mr-2"></timerIcon>
                <span>
                        <span v-language:inner v-show="needPayment">tutor_stream_btn_add_payment</span>
                        <span v-language:inner v-show="!needPayment">tutor_stream_btn_join_session</span>
                    </span>
            </button>

            <button v-show="roomIsActive" class="end-session" @click="endSession()">
                <stopIcon class="stop-icon mr-2"></stopIcon>
                <span v-language:inner>tutor_stream_btn_end_session</span>
            </button>
        </v-flex>
    </div>
</template>

<script>
    import {mapGetters, mapActions, mapState} from 'vuex';
    import tutorService from "../../tutorService";
    import videoStreamService from "../../../../services/videoStreamService";
    import timerIcon from '../../images/timer.svg';
    import stopIcon from '../../images/stop-icon.svg';
    export default {
        name: "startEndSessionBtn",
        components:{
            timerIcon,
            stopIcon

        },
        props: {
            id: {
                type: String,
                required: true
            },
        },
        computed: {
            ...mapState(['tutoringMainStore']),
            ...mapGetters([
                              'activeRoom',
                              'roomLoading',
                              'getCurrentRoomState',
                              'getStudyRoomData',
                              'getJwtToken',
                              'accountUser',
                          ]),
            roomIsPending() {
                return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.pending;
            },
            roomIsActive() {
                return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.active;
            },
            waitingStudent() {
                return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.loading;
            },
            isTutor() {
                return this.getStudyRoomData ? this.getStudyRoomData.isTutor : false;
            },
            needPayment() {
                return this.getStudyRoomData ? this.getStudyRoomData.needPayment : false;
            },
            accountUserID() {
                if (this.accountUser && this.accountUser.id) {
                    return this.accountUser.id;
                }
            }
        },
        methods: {
            ...mapActions([
                              'updateCurrentRoomState',
                              'updateReviewDialog',
                              'setRoomId',
                              'updateToasterParams',
                              'setSesionClickedOnce'
                          ]),

            minimize(type) {
                this.visible[`${type}`] = !this.visible[`${type}`];
            },
            // move all this function inside to service
            enterRoom() {
                videoStreamService.enterRoom();
            },
            endSession() {
                tutorService.endTutoringSession(this.id)
                            .then((resp) => {
                                console.log('ended session', resp);
                                this.setSesionClickedOnce(false)
                                if (!this.isTutor && this.getAllowReview) {
                                    this.updateReviewDialog(true);
                                }
                            }, (error) => {
                                console.log('error', error);
                            });
            },
            addDevicesToTrack() {
                videoStreamService.addDevicesTotrack();
            },
            // Create a new chat
            createVideoSession() {
                const self = this;
                // remove any remote track when joining a new room
                let clearEl = document.getElementById('remoteTrack');
                if (clearEl) {
                    clearEl.innerHTML = "";
                }
                self.addDevicesToTrack();
            },
        },


    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
.btn-wrapper{
    .create-session{
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 12px 32px;
        margin: 24px 0;
        border-radius: 4px;
        background-color: @yellowNew;
        font-size: 16px;
        font-weight: bold;
        line-height: 1.23;
        letter-spacing: 0.5px;
        color: @color-white;
        outline: none;
        &.disabled{
            pointer-events: none;
            background-color:  @yellowNew;
        }
    }
    .end-session{
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 8px 14px;
        text-transform: uppercase;
        margin: 24px 0;
        border-radius: 4px;
        box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.16);
        border: solid 1px rgba(255, 59, 59, 0.29);
        background-color: rgba(255, 72, 72, 0.11);
        font-size: 14px;
        font-weight: bold;
        line-height: 1.23;
        letter-spacing: 0.5px;
        color: rgba(255, 72, 72, 0.66);
        outline: none;
        //&.disabled{
        //  pointer-events: none;
        //  background-color: rgba(0, 217, 131, 0.24);
        //}
    }
    .stop-icon{
        fill: rgba(255, 59, 59, 0.72);
    }
    .timer-icon{
        height: 24px;
        width: 24px;
        fill: #FFF;
    }
}
</style>