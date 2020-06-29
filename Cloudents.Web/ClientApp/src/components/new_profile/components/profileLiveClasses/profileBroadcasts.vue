<template>
    <div v-if="liveSessions.length">
        <div class="profileBroadcast pa-4 pb-0 pa-sm-0">
            <div class="mainTitle text-sm-center mb-8" v-t="'my_live_classes'"></div>
            <div 
                v-for="session in sessionsList"
                class="broadcastList"
                :class="{'expandLastChild': isExpand}"
                :key="session.id">
                <template v-if="isMobile">
                    <div class="sessionTitle mb-sm-2 mb-3">{{session.name}}</div>
                    <div class="header d-flex justify-space-between mb-2">
                        <div>
                            <v-icon size="20" color="#3b3b3c">sbf-dateIcon</v-icon>
                            <span class="dateTime ms-1">{{$d(session.created, 'tableDate')}}</span>
                        </div>
                        <div>
                            <v-icon size="20" color="#3b3b3c">sbf-clockIcon</v-icon>
                            <span class="dateTime ms-1">{{$d(session.created, 'broadcastHour')}}</span>
                        </div>
                    </div>
                </template>

                <div class="d-sm-flex listWrapper">
                    <div class="leftSide d-sm-flex me-sm-6">
                        <img :src="liveImage" alt="">
                    </div>
                    <div class="rightSide d-flex flex-column justify-space-between flex-grow-1 pa-3 pe-0 ps-0">

                        <div class="header d-flex justify-space-between mb-4" v-if="!isMobile">
                            <div>
                                <v-icon size="20" color="#3b3b3c">sbf-dateIcon</v-icon>
                                <span class="dateTime ms-1">{{$d(session.created, 'tableDate')}}</span>
                            </div>
                            <div>
                                <v-icon size="20" color="#3b3b3c">sbf-clockIcon</v-icon>
                                <span class="dateTime ms-1">{{$d(session.created, 'broadcastHour')}}</span>
                            </div>
                        </div>

                        <div class="center">
                            <div class="sessionTitle mb-3" v-if="!isMobile">{{session.name}}</div>
                            <input type="checkbox" value="false" class="toggleCheckbox" :id="session.index" />
                            <template>
                                <div class="description">
                                    {{session.description | truncate(isOpen, '...', textLimit)}}
                                </div>
                                <label :for="session.index" v-if="session.description && session.description.length >= textLimit" sel="bio_more" class="readMore">{{readBtnText}}</label>
                                <div class="restOfText">
                                    {{session.description}}
                                </div>
                            </template>
                        </div>

                        <div class="bottom d-flex align-end justify-space-between text-center" :class="{'mt-6': session.description}">
                                <v-btn
                                    v-if="isMyProfile || session.enrolled"
                                    @click="enterRoom(session.id)"
                                    class="white--text btn"
                                    rounded
                                    depressed
                                    color="#ff6f30"
                                    height="40"
                                >
                                    <enterIcon class="enterIcon mr-sm-2" width="18" />
                                    <span :class="{'flex-sm-grow-1 pl-2': isMobile}" v-t="'enter'"></span>
                                </v-btn>
                                <v-btn
                                    v-else-if="session.isFull"
                                    disabled
                                    class="white--text btn"
                                    rounded
                                    depressed
                                    color="#ff6f30"
                                    height="40"
                                >
                                    <span :class="{'flex-sm-grow-1 pl-2': isMobile}" v-t="'full'"></span>
                                </v-btn>
                                <v-btn
                                    v-else
                                    @click="enrollSession(session.id)"
                                    class="white--text btn"
                                    rounded
                                    depressed
                                    color="#ff6f30"
                                    height="40"
                                >
                                    <span v-t="'enroll'"></span>
                                </v-btn>
                            <div class="subscription">
                                <span v-t="'regular'"></span>
                                <span class="number text-left ms-sm1">{{$price(session.price.amount, session.price.currency, true)}}</span>
                            </div>
                            <div class="subscription">
                                <span v-t="'subscriber'"></span>
                                <span class="number text-left ms-sm-1">{{$price(0, session.price.currency, true)}}</span>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

        <div class="showMore text-center pb-3" v-if="liveSessions.length > 2">
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
            isExpand: false,
        }
    },
    computed: {
        liveImage() {
            return this.isMobile ? require('./live-banner-copy@3x.png') : require('./live-banner.png')
        },
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
            return liveList.slice(0, 2)
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
            return this.isMobile ? 110 : 260;
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
            this.$router.push({name: routeNames.StudyRoom, params: { id: studyRoomId } })
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
        // restOfText(val = '', isOpen, suffix, textLimit){
        //     if (val.length > textLimit && !isOpen) {
        //         return val.substring(textLimit) ;
        //     }
        //     if (val.length > textLimit && isOpen) {
        //         return '';
        //     }
        // }
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
        // padding: 0 270px;
        max-width: 400px;
        margin: 0 auto;
        font-weight: 600;
        color: #363637;
        
        @media(max-width: @screen-xs) {
            padding: 0;
            font-size: 30px;
            text-align: left;
            max-width: 350px;
            margin: 0;
        }
    }
    .toggleCheckbox[type=checkbox] {
        display: none;
    }
    .restOfText {
        height: 0;
        visibility: hidden;
        transition: all .3s;
    }
    .toggleCheckbox[type=checkbox]:checked  {
        & ~.description {
            display: none !important;
        }
        & ~ .restOfText {
        //label
            line-height: 1.5;
            height: auto;
            visibility: visible;
        }
        & ~label {
            display: none;
        }
    }
    .broadcastList {
        margin-bottom: 28px;
        &:nth-child(3) {
            margin-bottom: 20px;   
        }
        &.expandLastChild {
            &:nth-child(3) {
                margin-bottom: 28px;   
            }
            &:last-child{
                margin-bottom: 20px;
            }
            
        }
        .listWrapper{
            border-top: 2px solid #ff6f30;
            border-bottom: 2px solid #ebecef;
            .leftSide {
                height: 100%;
                img {
                    object-fit: contain;
                    @media(max-width: @screen-xs) {
                        width: 100%;
                    }
                }
            }
            .rightSide {
                .center {
                    color: #363637;

                    .description {
                        line-height: 1.5;
                        display: contents;
                    }
                    .readMore {
                        cursor: pointer;
                    }
                }
                .bottom {
                    color: #363637;
                    .btn {
                        width: 40%;
                        max-width: 220px;
                    }
                    .number {
                        font-size: 18px;
                        font-weight: 600;
                        @media(max-width: @screen-xs) {
                            display: block;
                        }
                    }
                    .subscription {
                        padding: 0 10px;
                    }
                }
            }
        }
    }
    .dateTime {
        color: #363637;
        font-weight: 600;
        font-size: 15px;
        vertical-align: middle;
    }
    .sessionTitle {
        font-size: 19px;
        font-weight: 600;
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
</style>