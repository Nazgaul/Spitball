<template>
    <div class="profileBroadcast">
        <div class="mainTitle text-center mb-8" v-t="'my_live_classes'"></div>

        <div class="broadcastList mb-6" v-for="(session, index) in sessionsList" :key="index">
            <div class="d-flex wrapper">
                <div class="leftSide d-flex me-6">
                    <img src="./live-banner.png" alt="">
                </div>
                <div class="rightSide d-flex flex-column justify-space-between flex-grow-1 pa-3 ps-0">

                    <div class="header d-flex justify-space-between mb-4">
                        <div>
                            <v-icon size="20" color="#3b3b3c">sbf-dateIcon</v-icon>
                            <span class="dateTime ms-2">{{$d(session.created, 'tableDate')}}</span>
                        </div>
                        <div>
                            <v-icon size="20" color="#3b3b3c">sbf-clockIcon</v-icon>
                            <span class="dateTime ms-2">{{$d(session.created, 'broadcastHour')}}</span>
                        </div>
                    </div>

                    <div class="center">
                        <div class="sessionTitle mb-2">{{session.name}} {{session.isOpen}}</div>
                        <template>
                            <div class="description">
                                {{session.description | truncate(session.isOpen, '...', textLimit)}}
                            </div>
                            <div class="d-none">
                                {{session.description | restOfText(session.isOpen, '...', textLimit)}}
                            </div>
                            <span sel="bio_more" @click="readMoreOrLess(session)" class="readMore">{{readBtnText}}</span> 
                        </template>
                    </div>

                    <div class="bottom d-flex align-end justify-space-between mt-6">
                            <v-btn
                                v-if="isMyProfile || session.enrolled"
                                @click="enterRoom(session.id)"
                                class="white--text"
                                rounded
                                depressed
                                color="#ff6f30"
                                height="40"
                                width="220"
                            >
                                <enterIcon class="enterIcon mr-sm-2" width="18" />
                                <span :class="{'flex-grow-1 pl-2': isMobile}" v-t="'enter'"></span>
                            </v-btn>
                            <v-btn
                                v-else-if="session.isFull"
                                disabled
                                class="white--text"
                                rounded
                                depressed
                                color="#ff6f30"
                                height="40"
                                width="220"
                            >
                                <span :class="{'flex-grow-1 pl-2': isMobile}" v-t="'full'"></span>
                            </v-btn>
                            <v-btn
                                v-else
                                @click="enrollSession(session.id)"
                                class="white--text"
                                rounded
                                depressed
                                color="#ff6f30"
                                height="40"
                                width="220"
                            >
                                <span v-t="'enroll'"></span>
                            </v-btn>
                        <div>
                            <span v-t="'regular'"></span>
                            <span class="number">{{$price(session.price.amount, session.price.currency, true)}}</span>
                        </div>
                        <div>
                            <span v-t="'subscriber'"></span>
                            <span class="number">{{$price(0, session.price.currency, true)}}</span>
                        </div>
                    </div>

                </div>
            </div>
        </div>
        
        <div class="showMore text-center pb-3" v-if="liveSessions.length > 3">
            <arrowDownIcon class="arrowIcon" :class="{'exapnd': isExpand}" width="26" @click="isExpand = !isExpand"/>
        </div>

        <v-snackbar
            absolute
            top
            :timeout="5000"
            :color="color"
            @input="showSnack = false"
            :value="showSnack"
        >
            <div class="text-wrap white--text">{{toasterText}}</div>
        </v-snackbar>
    </div>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames'

import enterIcon from './enterRoom.svg'
import arrowDownIcon from './group-3-copy-12.svg'
export default {
    name: 'profileLiveClasses',
    components: {
        enterIcon,
        arrowDownIcon  
    },
    props: {
        id: {
            required: true
        }
    },
    data() {
        return {
            defOpen:false,
            liveSessions: [],
            showSnack: false,
            color: '',
            toasterText: '',
            isExpand: false
        }
    },
    computed: {
        readBtnText() {
            return this.isOpen ? this.$t('profile_read_less') : this.$t('profile_read_more')
        },
        isTutorSubscription() {
            return this.$store.getters.getProfileTutorSubscription
        },
        sessionsList() {
            return this.liveSessionsList.map((session, index) => {
                return {
                    ...session,
                    index,
                    isOpen: false
                }
            })
        },
        liveSessionsList() {
            let liveList = this.liveSessions
            if(this.isExpand) {
                return liveList
            }
            return liveList.slice(0, 3)
        },
        tutorFirstName() {
            return this.$store.getters.getProfile?.user?.firstName
        },
        isMyProfile(){
            return this.$store.getters.getIsMyProfile
        },
        isLogged() {
            return this.$store.getters.getUserLoggedInStatus
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        textLimit(){
            return this.isMobile ? 30 : 260;
        },
        isOpen :{
            get(){
                return this.defOpen
            },
            set(val){
                this.defOpen = val
            }
        },
    },
    methods: {
        enrollSession(studyRoomId) {
            if(!this.isLogged) {
                this.$store.commit('setComponent', 'register')
                return
            }

            let session = {
                userId: this.id,
                studyRoomId
            }
            let self = this
            this.$store.dispatch('updateStudyroomLiveSessions', session)
                .then(() => {
                    self.toasterText = this.$t('profile_enroll_success')
                    let currentSession = self.liveSessions.filter(s => s.id === studyRoomId)[0]
                    currentSession.enrolled = true
                }).catch(ex => {
                    self.color = 'error'
                    self.toasterText = this.$t('profile_enroll_error')
                    self.$appInsights.trackException(ex);
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
                    self.$emit('isComponentReady')
                }).catch(ex => {
                    self.$appInsights.trackException(ex);
                })
        },
        readMoreOrLess(session) {
            this.$set(session, 'isOpen', true);
            
        }
    },
    filters: {
        truncate(val = '', isOpen, suffix, textLimit){
            if (val.length > textLimit && !isOpen) {
                return val.substring(0, textLimit) +  suffix + ' ';
            } 
            if (val.length > textLimit && isOpen) {
                return val + ' ';
            }
            return val;
        },
        restOfText(val = '', isOpen, suffix, textLimit){
            if (val.length > textLimit && !isOpen) {
                return val.substring(textLimit) ;
            }
            if (val.length > textLimit && isOpen) {
                return '';
            }
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

.profileBroadcast {
    max-width: 960px;
    background: #fff;
    margin: 54px auto 0;

    @media(max-width: @screen-xs) {
        margin: 8px auto;
        background: transparent;
    }

    .mainTitle {
        font-size: 36px;
        padding: 0 270px;
        font-weight: 600;
        color: #363637;
    }

    .broadcastList {

        .wrapper{
            border-top: 2px solid #ff6f30;
            border-bottom: 2px solid #ebecef;
            .leftSide {
                height: 100%;
                img { 
                }
            }
            .rightSide {
                .header {
                    .dateTime {
                        color: #363637;
                        font-weight: 600;
                        font-size: 15px;
                    }
                }
                .center {
                    color: #363637;
                    .sessionTitle {
                        font-size: 19px;
                        font-weight: 600;
                    }
                    .description {
                        display: contents;
                    }
                    .readMore {
                        cursor: pointer;
                    }
                }
                .bottom {
                    color: #363637;

                    .number {
                        font-size: 18px;
                        font-weight: 600;
                    }
                }
            }
        }
    }
    .showMore {
        .arrowIcon {
            cursor: pointer;
            &.exapnd {
                transform: scaleY(-1);
            }
        }
    }
}
</style>