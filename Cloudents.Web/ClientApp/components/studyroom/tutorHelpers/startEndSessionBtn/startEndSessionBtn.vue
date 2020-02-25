<template>
    <div class="btn-wrapper">
        <v-flex v-show="isTutor">
            <button v-show="roomIsActive && !waitingStudent" class="end-session" @click="endSession()">
                <stopIcon class="stop-icon mr-2"></stopIcon>
                <span v-language:inner>tutor_stream_btn_end_session</span>
            </button>
        </v-flex>
        <v-flex v-show="!isTutor">
            <button v-show="roomIsActive" class="end-session" @click="endSession()">
                <stopIcon class="stop-icon mr-2"></stopIcon>
                <span v-language:inner>tutor_stream_btn_end_session</span>
            </button>
        </v-flex>
    </div>
</template>

<script>
    import {mapGetters, mapActions, mapState} from 'vuex';
    import stopIcon from '../../images/stop-icon.svg';
    export default {
        name: "startEndSessionBtn",
        components:{
            stopIcon
        },
        computed: {
            ...mapState(['tutoringMain']),
            ...mapGetters(['getCurrentRoomState','getStudyRoomData']),
            roomIsActive() {
                return this.getCurrentRoomState === this.tutoringMain.roomStateEnum.active;
            },
            waitingStudent() {
                return this.getCurrentRoomState === this.tutoringMain.roomStateEnum.loading;
            },
            isTutor() {
                return this.getStudyRoomData ? this.getStudyRoomData.isTutor : false;
            },
        },
        methods: {
            ...mapActions(["updateEndDialog"]),
            endSession() {
                this.updateEndDialog(true);
            },
        },


    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
.btn-wrapper{
    .end-session{
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 12px 42px;
        text-transform: capitalize;
        border-radius: 4px;
        box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.19);
        border: solid 1px rgba(255, 255, 255, 0.75);
        background-color: rgba(0, 0, 0, 0.23);
        font-size: 14px;
        font-weight: bold;
        line-height: 1.23;
        letter-spacing: 0.5px;
        color: #ffffff;
        outline: none;
        .stop-icon{
            fill: #ffffff;
        }
    }

}
</style>