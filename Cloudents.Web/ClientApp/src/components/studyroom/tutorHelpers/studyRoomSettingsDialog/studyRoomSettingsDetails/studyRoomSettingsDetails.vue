<template>
    <div class="settingDetailsWrap ms-md-12 pa-4 pa-sm-0" :style="!roomName? 'visibility: hidden': ''">
        <div class="mb-5 settingDetails">
            <div class="settingTitle mb-2 mb-sm-3">{{roomName}}</div>
            <div>
                <table class="settingTable">
                    <tr>
                        <td class="" v-t="'studyRoomSettings_tutor_name'"></td>
                        <td class="ps-4">{{roomTutor.tutorName}}</td>
                    </tr>
                    <tr v-if="!isMyRoom">
                        <td class="" v-t="'studyRoomSettings_price'"></td>
                        <td class="ps-4 d-flex">
                            <span class="pe-2" v-if="roomPrice">{{roomPrice}}</span>
                            <span v-else v-t="'studyRoomSettings_free'"></span>
                            <button v-if="showApplyCouponBtn" class="couponBtn" v-t="'studyRoomSettings_apply_coupon'" @click="$store.commit('setComponent', 'applyCoupon')"></button>
                        </td>
                    </tr>
                    <tr>
                        <td class="" v-t="'studyRoomSettings_share'"></td>
                        <td class="ps-1">
                            <shareContent
                                class="settingShareContent pa-0"
                                :link="shareContentParams.link"
                                :twitter="shareContentParams.twitter"
                                :whatsApp="shareContentParams.whatsApp"
                                :email="shareContentParams.email"
                                :roomSetting="true"
                            />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="counterWrap">
            <template>
                <div class="mb-6 mb-sm-8" v-if="isRoomBroadcast && isRoomDisabled">
                    <div v-if="!waitingForTutor" class="counterTitle mb-2" v-t="'studyRoomSettings_start_in'"></div>
                    <sessionStartCounter v-if="!waitingForTutor" class="counter" @updateCounterFinish="waitingForTutor = true" />
                </div>
                <div class="mb-8" v-if="isWaiting" v-t="'studyRoomSettings_ready'"></div>
            </template>
        </div>

        <template>
            <div v-if="!isRoomTutor" style="max-width:320px;" class="mx-auto mx-sm-0">
                <v-btn :loading="loadings.student"  @click="studentEnterRoom()" class="joinNow white--text px-8"
                    :disabled="isJoinButton" height="50" block color="#5360FC" rounded depressed>
                    {{$t('studyRoomSettings_join_now')}}
                </v-btn>
            </div>

            <div v-else dense class="tutorActions text-center text-md-left d-sm-flex d-block">
                <div>
                    <v-btn 
                        :loading="loadings[roomModes.whiteboard]" 
                        @click="tutorActions(roomModes.whiteboard)" 
                        class="me-sm-2 me-0 mb-4"
                        color="#4c59ff" height="44" min-width="140" depressed rounded outlined :block="$vuetify.breakpoint.xsOnly">
                        <whiteboardSvg width="18" />
                        <div class="flex-grow-1 btnText ms-sm-1 ma-0 pe-2 pe-sm-0 ms-1">{{$t('studyRoomSettings_whiteboard')}}</div>
                    </v-btn>
                </div>
                <div>
                    <v-btn :loading="loadings[roomModes.present]"
                        @click="tutorActions(roomModes.present)" class="mx-sm-2 mx-0 mb-4"
                        color="#4c59ff" height="44" min-width="140" depressed rounded outlined :block="$vuetify.breakpoint.xsOnly">
                        <presentSvg width="18" />
                        <div class="flex-grow-1 btnText pe-2 pe-sm-0 ms-1">{{$t('studyRoomSettings_present')}}</div>
                    </v-btn>
                </div>
                <div>
                    <v-btn :loading="loadings[roomModes.fullview]" :disabled="!$store.getters.settings_getIsVideo"
                        @click="tutorActions(roomModes.fullview)" class="fullscreen ms-sm-2 ms-0 mb-4" :block="$vuetify.breakpoint.xsOnly"
                        color="#4c59ff" height="44" min-width="140" depressed rounded outlined>
                        <fullviewSvg width="18" />
                        <div class="flex-grow-1 btnText pe-2 pe-sm-0 ms-1" >{{$t('studyRoomSettings_full_view')}}</div>
                    </v-btn>
                </div>
            </div>
        </template>
        <registerToJoinDialog @closeRegisterToJoin="isRegisterToJoinDialog = false" v-if="isRegisterToJoinDialog"/>
    </div>
</template>

<script>
import registerToJoinDialog from '../../../layouts/registerToJoinDialog/registerToJoinDialog.vue';
import sessionStartCounter from '../../sessionStartCounter/sessionStartCounter.vue'
import shareContent from '../../../../pages/global/shareContent/shareContent.vue'

import whiteboardSvg from '../images/whiteboard.svg'
import presentSvg from '../images/present.svg'
import fullviewSvg from '../images/fullview.svg'

export default {
    components: {
        sessionStartCounter,
        shareContent,
        whiteboardSvg,
        presentSvg,
        fullviewSvg,
        registerToJoinDialog
    },
    props: {
        isRoomActive: {
            type: Boolean,
            required: true
        }
    },
    data() {
        return {
            isRegisterToJoinDialog:false,
            waitingForTutor:false,
            clickOccur: false,
            loadings:{
                whiteboard: false,
                present: false,
                fullview: false,
                student:false,
            },
            selectedRoomMode:'',
            actions: {
                whiteboard: this.whiteboard,
                present: this.present,
                fullview: this.fullview,
            },
            roomModes:{
                whiteboard:'whiteboard',
                present: 'present',
                fullview: 'fullview',
            }
        }
    },
    computed: {
        isLoggedIn(){
            return this.$store.getters.getUserLoggedInStatus;
        },
        isJoinButton(){
            if(!this.isLoggedIn){
                return false;
            }else{
                return this.isRoomDisabled
            }
        },
        isWaiting(){
            if(!this.isRoomTutor){
                return (this.isRoomDisabled && this.waitingForTutor) || (this.isRoomDisabled && !this.isRoomBroadcast)
            }else{
                return false
            }
        },
        isRoomBroadcast(){
            return this.$store.getters.getRoomIsBroadcast;
        },
        isRoomDisabled(){
            return !this.$store.getters.getJwtToken;
        },
        shareContentParams(){
            let urlLink = `${window.origin}${this.$route.fullPath}?t=${Date.now()}`;
            let paramObJ = {
                link: urlLink,
                twitter: this.$t('shareContent_share_profile_twitter',[this.roomTutor.tutorName,urlLink]),
                whatsApp: this.$t('shareContent_share_profile_whatsapp',[this.roomTutor.tutorName,urlLink]),
                email: {
                    subject: this.$t('shareContent_share_profile_email_subject',[this.roomTutor.tutorName]),
                    body: this.$t('shareContent_share_profile_email_body',[this.roomTutor.tutorName,urlLink]),
                }
            }
            return paramObJ
        },
        isRoomTutor(){
            return this.$store.getters.getRoomIsTutor;
        },
        roomName() {
            return this.$store.getters?.getRoomName
        },
        currencySymbol() {
            return this.$store.getters.accountUser?.currencySymbol
        },
        roomPrice(){
            let priceObj = this.roomTutor?.tutorPrice
            if(priceObj?.amount > 0){
                // TODO: Currency Change
                return this.$price(priceObj.amount, priceObj.currency)
                // return this.$n(this.roomTutor.tutorPrice, {'style':'currency','currency': this.currencySymbol, minimumFractionDigits: 0, maximumFractionDigits: 0})
            }
            return 0
        },
        isMyRoom() {
            return this.roomTutor?.tutorId === this.$store.getters.accountUser?.id
        },
        roomTutor() {
            return this.$store.getters.getRoomTutor
        },
        showApplyCouponBtn(){
            if(this.isRoomBroadcast) return false;
            else{
                return this.isLoggedIn && this.roomPrice
            }
        }
        // roomLink() {
        //     // @idan - I think this better approach getting the room id with $route.params instead of passing props
        //     // TODO: Make room link getter from store
        //     return `${window.origin}/studyroom/${this.$route.params.id}`
        // }
    },
    watch: {
        isRoomDisabled(val){
            if(val){
                this.loadings.student = false;
                this.$store.commit('setComponent', 'errorToaster_sessionEnded');
            }
        }
    },
    methods: {
        studentEnterRoom(){
            if(!this.isLoggedIn){
                this.isRegisterToJoinDialog = true;
            }else{
                this.loadings.student = true;
                this.$store.dispatch('updateRoomIsJoined',true)
            }
        },
        tutorActions(roomMode){
        if(Object.values(this.loadings).some(loader=>loader)){
            return
        }
        let self = this
        this.selectedRoomMode = roomMode;
        this.loadings[roomMode] = true;
        this.$store.dispatch('updateEnterRoom', this.$route.params.id)
            .then(() => {
                this.$store.dispatch('updateRoomIsJoined',true);
                this.actions[this.selectedRoomMode]();
            }).catch(ex => {
                self.$appInsights.trackException(ex);
            })
        },

        whiteboard() {
            let roomModes = this.$store.getters.getRoomModeConsts
            this.$store.dispatch('updateActiveNavEditor',roomModes.WHITE_BOARD)
            this.selectedRoomMode = ''
        },
        present() {
            let roomModes = this.$store.getters.getRoomModeConsts
            this.$store.dispatch('updateActiveNavEditor',roomModes.SCREEN_MODE)
            this.selectedRoomMode = ''
        },
        fullview() {
            this.$store.dispatch('updateToggleTutorFullScreen',true)
            this.selectedRoomMode = ''
        }
    },
    beforeDestroy() {
        this.waitingForTutor = false;
        if(this.isRoomTutor){
            this.loadings[this.selectedRoomMode] = false;
            this.selectedRoomMode = '';
        }else{
            this.loadings.student = false;
        }
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';

.settingDetailsWrap {
    flex-shrink: 0;
    color: @global-purple;
    .settingDetails {
        .settingTitle {
            font-size: 26px;
        }
        .settingTable {
            width: 100%;
            border-spacing: 0 12px;
            font-weight: 600;

            td {
                &:first-child {
                    width: 120px;
                }
            }
            .settingShareContent {
                height: 19px;
                svg {
                    width: 16px !important;
                }
                .btnWrapper {
                    .shareBtns:first-child {
                         svg {
                            width: 8px !important;
                        }
                    }
                }
                .option.link  {
                    visibility: hidden !important;
                }
            }
            .couponBtn {
                outline: none;
                color: #bbb;
            }
        }
    }

    .counterWrap {
        max-width: 300px;
        .counterTitle {
            font-weight: 600;
            font-size: 16px;
        }
        .counter {
            // display: flex;
            font-size: 32px;
            span {
                &:first-child {
                    margin-left: 0;
                }
                &:last-child {
                    /*rtl:ignore*/
                    margin-right: 0;
                }
                margin:0 14px;
            }
        }

        .joinNow {
            font-weight: 600;
        }
    }
    .tutorActions {
        
        button {
            font-weight: 600;
            .btnText {
                margin-bottom: 2px;

            }
            &.fullscreen {
                &:disabled {
                    path {
                        stroke: rgba(0, 0, 0, 0.26) !important
                    }
                }
            }
        }
    }
}
</style>