<template>
    <div class="student-start-wrap pb-12">
        <v-layout class="pt-3">
            <v-flex xs12 class="text-right px-4">
                <v-icon class="body-2 cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout column align-center>
            <v-flex xs12  class="pt-2">
                <userAvatar :user-name="userName"  :userImageUrl="userImageUrl" :user-id="userId" :size="'58'" ></userAvatar>
            </v-flex>
            <v-flex xs12  class="pt-3">
                <span class="student-start-wrap_title font-weight-bold" v-t="'tutor_end_session'"></span>
            </v-flex>
            <v-card-actions class="pt-12">
                <button class="cancel-btn elevation-0 align-center justify-center me-2" @click="closeDialog()">
                    <span class="text-capitalize" v-t="'tutor_chrome_ext_btn_cancel'"></span>
                </button>
                <button class="end-session-btn elevation-0 align-center justify-center" @click="endSession()">
                    <stopIcon class="timer-icon me-2"></stopIcon>
                    <span class="text-capitalize" v-t="'tutor_stream_btn_end_session'"></span>
                </button>
            </v-card-actions>
        </v-layout>
    </div>
</template>

<script>
    import {mapGetters} from 'vuex';
    import stopIcon from '../../images/stop-icon.svg';
    export default {
        name: "endSessionConfirm",
        components: {stopIcon},
        data() {
            return {};
        },
        computed: {
            ...mapGetters(['accountUser']),
            userImageUrl(){
                if(this.accountUser.image.length > 1){
                    return `${this.accountUser.image}`
                }
                return ''
            },
            userName(){
                return this.accountUser && this.accountUser.name ? this.accountUser.name : '';
            },
            userId(){
                return this.accountUser && this.accountUser.id ? this.accountUser.id : null;
            }
        },
        methods: {
            closeDialog() {
                this.$store.dispatch('updateEndDialog',false)
            },
            endSession(){
                this.$store.dispatch('updateEndSession');
            }
        },
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .pt-12{
        padding-top: 12px;
    }
    .student-start-wrap{
        @greenBtnBackground: #00d983;
        background: @color-white;
        border-radius: 4px;
        box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
        width: 356px;
        .student-start-wrap_title{
            font-size: 16px;
        }
        .cancel-btn{
            min-width: 146px;
            display: flex;
            height: 48px;
            color: @textColor;
            font-size: 14px;
            background-color: transparent;
            border-radius: 4px;
            border: solid 1px rgba(0, 0, 0, 0.38);
            font-weight: bold;
        }
        .end-session-btn{
            display: flex;
            min-width: 146px;
            height: 48px;
            color: @color-white;
            background-color: #ff3e55;
            border-radius: 4px;
            font-size: 14px;
            .timer-icon{
                fill: @color-white;
                max-width: 24px;
                height: 24px; // overide global svg height
            }
        }
    }

</style>