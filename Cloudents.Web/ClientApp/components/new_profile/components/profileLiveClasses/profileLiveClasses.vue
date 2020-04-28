<template>
    <div class="profileLiveClasses pa-sm-4 pa-0">

        <div class="mainTitle pl-4 pl-sm-0 pb-sm-4 pt-3 pt-sm-0 text-truncate">
            <span v-t="'profile_live_title'"></span>
            <span>{{tutorName}}</span>
        </div>

        <div class="liveRow pa-4 pa-sm-0 px-sm-2 py-sm-7 py-5 d-sm-flex d-block justify-space-between align-center" v-for="(session, index) in liveSessionsList" :key="index">
            <div class="leftSide mb-5 mb-sm-0 d-flex">
                <div class="icons">
                    <radioIcon v-if="$vuetify.breakpoint.xsOnly" width="30" />
                    <tvIcon width="90" v-else />
                </div>

                <div class="details ml-3 ml-sm-5 d-flex">
                    <div class="created mb-sm-3 mt-2 mt-sm-0">{{$d(new Date(session.created), 'long')}}</div>
                    <div class="liveName">{{session.name}}</div>
                </div>
            </div>

            <div class="rightSide d-sm-flex d-block align-end flex-shrink-0 text-center">
                <div class="price mb-2 mb-sm-0" :class="{'enroll': session.enrolled}">
                    <span class="numericPrice">{{$n(session.price, 'currency')}}</span>
                    <span v-t="'profile_per_hour'"></span>
                </div>
                <div class="action">
                    <v-btn
                        v-if="isMyProfile || session.enrolled"
                        @click="enterRoom(session.id)"
                        class="btn white--text"
                        width="140"
                        :height="$vuetify.breakpoint.xsOnly ? '42' : '38'"
                        color="#41c4bc"
                        :block="$vuetify.breakpoint.xsOnly"
                        depressed
                        rounded
                    >
                        <enterIcon class="enterIcon mr-sm-2" width="18" />
                        <span :class="{'flex-grow-1 pr-4': $vuetify.breakpoint.xsOnly}" v-t="'profile_enter_room'"></span>
                    </v-btn>
                    <v-btn
                        v-else
                        @click="enrollSession(session.id)"
                        class="btn white--text"
                        width="140"
                        :height="$vuetify.breakpoint.xsOnly ? '42' : '38'"
                        color="#4c59ff"
                        :block="$vuetify.breakpoint.xsOnly"
                        depressed
                        rounded
                    >
                        <span :class="{'flex-grow-1 pr-4': $vuetify.breakpoint.xsOnly}" v-t="'profile_enroll'"></span>
                    </v-btn>
                </div>
            </div>
        </div>
        
        <div class="showMore pa-3 pt-sm-4 pb-sm-0 text-center" v-if="liveSessions.length > 3">
            <button class="showBtn" v-t="isExpand ? 'profile_see_less' : 'profile_see_all'" @click="isExpand = !isExpand"></button>
        </div>

        <v-snackbar
            absolute
            top
            :timeout="5000"
            :color="color"
            @input="showSnack = false"
            :value="showSnack"
        >
            <div class="text-wrap white--text" v-t="toasterText"></div>
        </v-snackbar>

    </div>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames'

import tvIcon from './brodcast-copy-9.svg'
import radioIcon from './group-4-copy-5.svg'
import enterIcon from './enterRoom.svg'

export default {
    name: 'profileLiveClasses',
    props: {
        id: {
            required: true
        }
    },
    components: {
        tvIcon,
        radioIcon,
        enterIcon,
    },
    data() {
        return {
            liveSessions: [],
            showSnack: false,
            color: '',
            toasterText: '',
            isExpand: false
        }
    },
    computed: {
        liveSessionsList() {
            let liveList = this.liveSessions
            if(this.isExpand) {
                return liveList
            }
            return liveList.slice(0, 3)
        },
        tutorName() {
            return this.$store.getters.getProfile?.user?.firstName
        },
        isMyProfile(){
            let accountId = this.$store.getters?.accountUser?.id
            let profileId = this.$store.getters.getProfile?.user?.id
            return accountId == profileId
        },
        isLogged() {
            return this.$store.getters.getUserLoggedInStatus
        }
    },
    methods: {
        enrollSession(studyRoomId) {
            if(!this.isLogged) {
                this.$store.commit('setComponent', 'login')
                return
            }

            let session = {
                userId: this.id,
                studyRoomId
            }
            let self = this
            this.$store.dispatch('updateStudyroomLiveSessions', session)
                .then(() => {
                    self.toasterText = 'profile_enroll_success'
                    let currentSession = self.liveSessions.filter(s => s.id === studyRoomId)[0]
                    currentSession.enrolled = true
                }).catch(ex => {
                    self.color = 'error'
                    self.toasterText = 'profile_enroll_error'
                    self.$appInsights.trackException({exception: new Error(ex)});
                }).finally(() => {
                    self.showSnack = true
                })
        },
        enterRoom(studyRoomId) {
            this.$router.push({name: routeNames.StudyRoom, params: {id: studyRoomId} })
        },
        getLiveSessions() {
            let self = this;
            this.$store.dispatch('getStudyroomLiveSessions', this.id)
                .then(res => {
                    self.liveSessions = res
                }).catch(ex => {
                    self.$appInsights.trackException({exception: new Error(ex)});
                })
        }
    },
    created() {
        this.getLiveSessions()
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';

    .profileLiveClasses {
        max-width: 960px;
        background: #fff;
        margin: 54px auto 0;
        border-radius: 8px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
       
        // @media(max-width: @screen-sm) {
        //     margin: 54px 20px 0;
        // }
        @media(max-width: @screen-xs) {
            margin: 8px auto;
            box-shadow: none;
            border-radius: 0;
            background: transparent;
        }
        .mainTitle {
            color: @global-purple;
            font-weight: 600;
            .responsive-property(font-size, 18px, null, 16px);
            border-bottom: 1px solid #ebebeb;
            @media(max-width: @screen-xs) {
                border-bottom: none;
                font-weight: bold;
                background: #fff;
            }
        }

        .liveRow {
            border-bottom: 1px solid #ebebeb;

            &:last-child  {
                border-bottom: none;
            }
            @media(max-width: @screen-xs) {
                background: #fff;
                margin-bottom: 8px;
                border-bottom: none;
            }
            .details {
                font-weight: 600;
                flex-direction: column;
                padding: 0 50px 0 0;
                max-width: 450px;
                @media(max-width: @screen-xs) {
                    padding: 0;
                }
                .created {
                    color: @global-auth-text;
                    @media(max-width: @screen-xs) {
                        order: 1;
                    }
                }
                .liveName {
                    font-size: 16px;
                    color: @global-purple;
                }
            }
            .leftSide {
                .icons {
                    @media(max-width: @screen-xs) {
                        margin-top: 2px;
                    }
                }
            }
            .rightSide {
                .price {
                    vertical-align: bottom;
                    margin-right: 50px;
                    color: @global-purple;
                    @media(max-width: @screen-xs) {
                        margin-right: 0;
                    }
                    &.enroll {
                        font-weight: 600;
                        color: #bdc0d1;
                    }
                    .numericPrice {
                        font-size: 20px;
                        font-weight: bold;
                    }
                }
            }
            .action {
                .btn {
                    font-weight: 600;
                }
                .enterIcon {
                    fill: #fff;
                }
            }
        }
        .showMore {
            color: @global-purple;
            font-weight: 600;
            .showBtn {
                color: @global-auth-text;
                outline: none;
            }
            @media(max-width: @screen-xs) {
                background: #fff;
            }
        }
    }
</style>