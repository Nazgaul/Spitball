<template>
    <div class="settingDetailsWrap ml-md-12 pa-4 pa-sm-0" v-if="roomName">
        <div class="mb-5 settingDetails">
            <div class="settingTitle mb-2 mb-sm-3">{{roomName}}</div>
            <div>
                <table class="settingTable">
                    <tr>
                        <td class="" v-t="'studyRoomSettings_tutor_name'"></td>
                        <td class="pl-4">{{roomTutor.tutorName}}</td>
                    </tr>
                    <tr>
                        <td class="" v-t="'studyRoomSettings_price'"></td>
                        <td class="pl-4">{{$n(roomTutor.tutorPrice, 'currency')}}</td>
                    </tr>
                    <tr>
                        <td class="" v-t="'studyRoomSettings_share'"></td>
                        <td class="pl-1">
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
                    <div class="counterTitle mb-2" v-t="'studyRoomSettings_start_in'"></div>
                    <sessionStartCounter class="counter"/>
                    <!-- <sessionStartCounter class="counter" @updateCounterFinish="waitingForTutor = true" /> -->
                </div>
                <div class="mb-8" v-else v-t="'studyRoomSettings_ready'"></div>
            </template>
        </div>

        <template>
            <v-btn :loading="loadings.student" v-if="!isRoomTutor" @click="studentEnterRoom()" class="joinNow white--text px-8"
                :disabled="isRoomDisabled" height="50" block color="#5360FC" rounded depressed>
                {{$t('studyRoomSettings_join_now')}}
            </v-btn>

            <div v-else dense class="tutorActions text-center text-md-left d-sm-flex d-block">
                <div>
                    <v-btn 
                    :loading="loadings[roomModes.whiteboard]" 
                    @click="tutorActions(roomModes.whiteboard)" 
                    class="mr-sm-2 mr-0 mb-4"
                        color="#4c59ff" height="44" width="140" depressed rounded outlined :block="$vuetify.breakpoint.xsOnly">
                        <whiteboardSvg width="18" />
                        <div class="flex-grow-1 btnText ml-sm-1 ma-0 pr-2 pr-sm-0">{{$t('studyRoomSettings_whiteboard')}}</div>
                    </v-btn>
                </div>
                <div>
                    <v-btn :loading="loadings[roomModes.present]"
                        @click="tutorActions(roomModes.present)" class="mx-sm-2 mx-0 mb-4"
                        color="#4c59ff" height="44" width="140" depressed rounded outlined :block="$vuetify.breakpoint.xsOnly">
                        <presentSvg width="18" />
                        <div class="flex-grow-1 btnText pr-2 pr-sm-0">{{$t('studyRoomSettings_present')}}</div>
                    </v-btn>
                </div>
                <div>
                    <v-btn :loading="loadings[roomModes.fullview]" 
                        @click="tutorActions(roomModes.fullview)" class="ml-sm-2 ml-0 mb-4" :block="$vuetify.breakpoint.xsOnly"
                        color="#4c59ff" height="44" width="140" depressed rounded outlined>
                        <fullviewSvg width="18" />
                        <div class="flex-grow-1 btnText pr-2 pr-sm-0">{{$t('studyRoomSettings_full_view')}}</div>
                    </v-btn>
                </div>
            </div>
        </template>
    </div>
</template>

<script>
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
        fullviewSvg
    },
    props: {
        isRoomActive: {
            type: Boolean,
            required: true
        }
    },
    data() {
        return {
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
        isRoomBroadcast(){
            return this.$store.getters.getRoomIsBroadcast;
        },
        isRoomDisabled(){
            return !this.$store.getters.getJwtToken;
        },
        shareContentParams(){
            let urlLink = `${this.roomLink}?t=${Date.now()}` ;
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
        roomTutor() {
            return this.$store.getters?.getRoomTutor
        },
        roomLink() {
            // @idan - I think this better approach getting the room id with $route.params instead of passing props
            // TODO: Make room link getter from store
            return `${window.origin}/studyroom/${this.$route.params.id}`
        }
    },
    methods: {
        studentEnterRoom(){
            this.loadings.student = true;
            this.$store.dispatch('updateRoomIsJoined',true)
        },
        tutorActions(roomMode){
        
        let self = this
        this.selectedRoomMode = roomMode;
        // this.loadings[roomMode] = true;
        this.$store.dispatch('updateEnterRoom', this.$route.params.id)
            .then(() => {
                this.$store.dispatch('updateRoomIsJoined',true);
            }).catch(ex => {
                self.$appInsights.trackException({exception: new Error(ex)});
            })
        },

        whiteboard() {
            this.$store.dispatch('updateDialogEnter',false);
            this.selectedRoomMode = ''
        },
        present() {
            this.$store.dispatch('updateShareScreen', true)
            this.selectedRoomMode = ''
        },
        fullview() {
            this.$store.dispatch('updateToggleTutorFullScreen', true);
            this.selectedRoomMode = ''
        },
        
     },
    watch: {
        '$store.getters.getRoomIsActive':{
            deep:true,
            handler(val){
                if(val){
                    if(!this.isRoomTutor){
                        this.$store.dispatch('updateDialogEnter',false);
                        return
                    }
                    if(this.isRoomTutor && this.selectedRoomMode){
                        this.actions[this.selectedRoomMode]();
                        return
                    }
                    this.$store.dispatch('updateDialogEnter',false);
                }
            }
        }
    },
    beforeDestroy() {
        if(this.isRoomTutor){
            this.loadings[this.selectedRoomMode] = false;
            this.selectedRoomMode = '';
        }else{
            this.loadings.student = false;
        }
    },
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';

.settingDetailsWrap {
    flex-shrink: 0;
    color: @global-purple;
    .settingDetails {
        .settingTitle {
            font-size: 20px;
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
            display: flex;
            font-size: 32px;
            span {
                &:first-child {
                    margin-left: 0;
                }
                &:last-child {
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
        }
    }
}
</style>