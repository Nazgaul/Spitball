<template>
    <div class="settingDetailsWrap ml-md-12 pa-3 pa-sm-0">
        <div class="mb-5 settingDetails">
            <div class="settingTitle mb-4" v-t="'studyRoomSettings_class_name'"></div>

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
                <div class=" mb-8" v-if="!isRoomActive">
                    <div class="counterTitle mb-2" v-t="'studyRoomSettings_start_in'"></div>
                    <sessionStartCounter class="counter" @updateCounterFinish="$emit('updateRoomIsActive', true)" />
                </div>
                <div class="mb-8" v-else v-t="'studyRoomSettings_ready'"></div>
            </template>
        </div>

        <template>
            <v-btn
                v-if="!isMyProfile"
                class="joinNow white--text px-8"
                @click="$store.dispatch('updateEnterRoom', $route.params.id)"
                :disabled="!isRoomActive"
                height="50"
                block
                color="#5360FC"
                rounded
                depressed
            >
                {{$t('studyRoomSettings_join_now')}}
            </v-btn>

            <div v-else dense class="tutorActions text-center text-md-left d-block d-sm-flex">
                <v-col>
                    <v-btn
                        @click="tutorActions('whiteboard')"
                        color="#4c59ff"
                        height="46"
                        width="140"
                        depressed
                        rounded
                        outlined
                    >
                        <whiteboardSvg width="18" />
                        <div class="flex-grow-1">{{$t('studyRoomSettings_whiteboard')}}</div>
                    </v-btn>
                </v-col>
                <v-col>
                    <v-btn
                        @click="tutorActions('present')"
                        color="#4c59ff"
                        height="46"
                        width="140"
                        depressed
                        rounded
                        outlined
                    >
                        <presentSvg width="18" />
                        <div class="flex-grow-1">{{$t('studyRoomSettings_present')}}</div>
                    </v-btn>
                </v-col>
                <v-col>
                    <v-btn
                        @click="tutorActions('fullview')"
                        color="#4c59ff"
                        height="46"
                        width="140"
                        depressed
                        rounded
                        outlined
                    >
                        <fullviewSvg width="18" />
                        <div class="flex-grow-1">{{$t('studyRoomSettings_full_view')}}</div>
                    </v-btn>
                </v-col>
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
            actions: {
                whiteboard: this.whiteboard,
                present: this.present,
                fullview: this.fullview,
            }
        }
    },
    computed: {
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
        isMyProfile() {
            return this.roomTutor.tutorId === this.$store.getters.accountUser?.id
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
         tutorActions(name) {
            let self = this
            this.$store.dispatch('updateEnterRoom', this.$route.params.id)
                .then(() => {
                    // self.$emit('updateRoomIsActive', true)
                    self.actions[name]();
                }).catch(ex => {
                    self.$appInsights.trackException({exception: new Error(ex)});
                })
         },
        whiteboard() {
            this.$store.commit('setComponent', '')
        },
        present() {
            this.$store.dispatch('updateShareScreen', true)
        },
        fullview() {
            this.$store.dispatch('updateToggleTutorFullScreen', true);
        },
        
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
            font-size: 24px;
        }
        .settingTable {
            width: 100%;
            border-spacing: 0 20px;
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
            justify-content: space-between;
            font-size: 34px;
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
        }
    }
}
</style>