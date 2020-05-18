<template>
    <div class="profileLiveClasses pa-sm-4 pa-0" v-if="liveSessions.length">
        <div class="mainTitle px-4 py-2 pb-sm-6 text-truncate">
            <span v-t="'profile_live_title'"></span>
            <span>{{tutorFirstName}}</span>
        </div>

        <v-row class="headerRow text-center d-none d-sm-flex" dense>
            <v-col cols="6" class="pa-0"></v-col>
            <v-row cols="6" class="subscribers pa-0" dense>
                <v-col cols="4" class="pa-0">
                    <div class="pa-3" v-t="'profile_live_visitors_title'"></div>
                </v-col>
                <v-col cols="4" class="titleSubscriber pa-0" v-if="isTutorSubscription">
                    <div class="pa-3" v-t="'profile_live_subscribers_title'"></div>
                </v-col>
            </v-row>
        </v-row>

        <v-row 
            v-for="(session, index) in liveSessionsList"
            class="sessionRow text-center px-4 px-sm-0 pb-2 pb-sm-0"
            :class="{'pt-2': index && isMobile}"
            :key="index"
            dense
        >
            <v-col cols="12" sm="6" class="text-left d-flex flex-wrap mb-9 mb-sm-0 pa-4">
                <div class="icons d-flex mb-3" dense>
                    <radioIcon v-if="isMobile" width="30" />
                    <tvIcon width="90" v-else />
                    <div class="created ml-3 d-block d-sm-none">{{$d(session.created, 'long')}}</div>
                </div>

                <div class="details ml-sm-5 d-sm-flex" dense>
                    <div class="created mb-3 d-none d-sm-block">{{$d(session.created, 'long')}}</div>
                    <div class="">
                        <div class="liveName mb-2 text-truncate">{{session.name}}</div>
                        <div v-if="session.description">
                            <template v-if="isMobile">                             
                                <div class="description">
                                    {{session.description | truncate(isOpen, '...', textLimit)}}
                                </div>
                                <div class="d-none">
                                    {{session.description | restOfText(isOpen, '...', textLimit)}}
                                </div>
                                <span sel="bio_more" @click="isOpen = !isOpen" class="readMore" v-t="isOpen ? 'profile_read_less' : 'profile_read_more'"></span>                                    
                                
                            </template>
                            <div v-else class="description">{{session.description}}</div>
                        </div>
                    </div>
                </div>
            </v-col>

            <v-col cols="12" sm="6" class="pa-0">
                <v-row dense class="rowHeight pa-0 align-center">
                    <template v-if="isMobile">
                        <v-col cols="8" class="detailsMobile pa-0 d-flex align-center">
                            <v-row dense class="pa-0 ma-0 text-left flex-column">
                                <v-col class="d-flex align-center pa-0">
                                    <v-col class="pa-0">
                                        <div class="px-3 py-2" v-t="'profile_live_visitors_title'"></div>
                                    </v-col>
                                    <v-col class="pa-0">
                                        <div class="px-3 py-2 d-flex align-center" v-if="session.price">
                                            <span class="numericPrice mb-1">{{$n(session.price, 'currency')}}</span>
                                            <div class="d-flex align-end">
                                                <span>/</span>
                                                <span class="hour" v-t="'profile_points_hour'"></span>
                                            </div>
                                        </div>
                                        <div v-else>
                                            <div class="subscribeFree px-3 py-2" v-t="'profile_live_subscribers_free'"></div>
                                        </div>
                                    </v-col>
                                </v-col>
                                <v-col class="d-flex align-center pa-0">
                                    <v-col class="pa-0">
                                        <div class="px-3 py-2" v-t="'profile_live_subscribers_title'"></div>
                                    </v-col>
                                    <v-col class="pa-0 subscribeFree">
                                        <div class="px-3 py-2" v-t="'profile_live_subscribers_free'"></div>  
                                    </v-col>
                                </v-col>
                            </v-row>
                        </v-col>
                    </template>
                    <template v-else>
                        <v-col cols="4" class="pa-0 rowCol" :class="{'enroll': session.enrolled}">
                            <div v-if="session.price">
                                <span class="numericPrice mb-1">{{$n(session.price, 'currency')}}</span>
                                <div class="d-flex align-end">
                                    <span>/</span>
                                    <span class="hour" v-t="'profile_points_hour'"></span>
                                </div>
                            </div>
                            <div v-else class="subscribeFree">
                                <div class="" v-t="'profile_live_subscribers_free'"></div>
                            </div>
                        </v-col>
                        <v-col cols="4" class="pa-0 rowCol" :class="{'enroll': session.enrolled && isTutorSubscription}">
                            <div v-t="'profile_live_subscribers_free'" v-if="isTutorSubscription"></div>  
                        </v-col>
                    </template>
                    <v-col cols="4" class="pa-0 rowCol d-flex d-sm-block ma-auto pa-2">
                        <div class="action">
                            <v-btn
                                v-if="isMyProfile || session.enrolled"
                                @click="enterRoom(session.id)"
                                class="btn white--text"
                                :height="isMobile ? '46' : '38'"
                                color="#41c4bc"
                                block
                                depressed
                                :rounded="isMobile ? false : true"
                            >
                                <enterIcon class="enterIcon mr-sm-2" width="18" />
                                <span :class="{'flex-grow-1 pl-2': isMobile}" v-t="'profile_enter_room'"></span>
                            </v-btn>
                            <v-btn
                                v-else
                                @click="enrollSession(session.id)"
                                class="btn white--text"
                                :height="isMobile ? '46' : '38'"
                                color="#4c59ff"
                                block
                                depressed
                                :rounded="isMobile ? false : true"
                            >
                                <span v-t="'profile_enroll'"></span>
                            </v-btn>
                        </div>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>

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
            defOpen:false,
            liveSessions: [],
            showSnack: false,
            color: '',
            toasterText: '',
            isExpand: false
        }
    },
    computed: {
        isTutorSubscription() {
            return this.$store.getters.getProfileTutorSubscription
        },
        // tutorSubscriptionPrice() {
        //     return this.$store.getters.getProfileTutorSubscription
        // },
        liveSessionsList() {
            let liveList = this.liveSessions
            if(this.isExpand) {
                return liveList
            }
            return liveList.slice(0, 3)
        },
        tutorCurrency() {
            return this.$store.getters.getProfile?.user?.tutorData?.currency
        },
        tutorFirstName() {
            return this.$store.getters.getProfile?.user?.firstName
        },
        isMyProfile(){
            let accountId = this.$store.getters?.accountUser?.id
            let profileId = this.$store.getters.getProfile?.user?.id
            return accountId == profileId
        },
        isLogged() {
            return this.$store.getters.getUserLoggedInStatus
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        textLimit(){
            return this.isMobile ? 30 : 0;
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
    filters: {
        truncate(val, isOpen, suffix, textLimit){
            if (val.length > textLimit && !isOpen) {
                return val.substring(0, textLimit) +  suffix + ' ';
            } 
            if (val.length > textLimit && isOpen) {
                return val + ' ';
            }
            return val;
        },
        restOfText(val, isOpen, suffix, textLimit){
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

    .profileLiveClasses {
        max-width: 960px;
        background: #fff;
        margin: 54px auto 0;
        border-radius: 8px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);

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
            @media(max-width: @screen-xs) {
                border-bottom: none;
                font-weight: bold;
                background: #fff;
            }
        }
        .headerRow {
            color: #595475;
            font-size: 16px;
            font-weight: 600;
            border-bottom: 1px solid #ebebeb;
            .subscribers  {
                .titleSubscriber:nth-child(2) {
                    background: #f5f5f5;
                }
            }
        }
        .sessionRow {
            border-bottom: 1px solid #ebebeb;

            &:last-child {
                border-bottom: none;
            }

            .subscribers:nth-child(3) {
                color: @global-purple;
                font-size: 16px;
                font-weight: 600;
                background: #f5f5f5;
                @media(max-width: @screen-xs) {
                    background: #fff
                }   
            }
            @media(max-width: @screen-xs) {
                background: #fff;
                margin-bottom: 8px;
            }
            .details {
                flex-direction: column;
                min-width: 0;
                @media(max-width: @screen-xs) {
                    padding: 0;
                }

                .liveName {
                    font-weight: 600;
                    .responsive-property(font-size, 16px, null, 18px);
                    color: @global-purple;
                }
                .description {
                    display: inline-block;
                    color: @global-purple;
                    line-height: 22px;
                    .responsive-property(font-size, 13px, null, 14px);
                }
                .readMore {
                    color: @global-purple;
                    font-weight: 600;
                }
            }

            .created {
                color: @global-auth-text;
                font-weight: 600;
            }
            .rowHeight {
                height: 100%;
                color: @global-purple;
                @media(max-width: @screen-xs) {
                    margin-right: 0;
                }
                .rowCol {
                    align-self: stretch;
                    display: flex;
                    justify-content: center;
                    align-items: center;

                    .numericPrice {
                        font-size: 22px;
                        font-weight: bold;
                    }
                    &.enroll {
                        font-weight: 600;
                        color: #bdc0d1 !important;
                    }
                    .hour {
                        font-size: 16px
                    }
                    &:nth-child(2) {
                        background: #f5f5f5;
                        color: @global-purple;
                        font-size: 16px;
                        font-weight: 600;
                        @media (max-width: @screen-xs) {
                            background: inherit;
                        }
                    }
                    .action {
                        .btn {
                            font-weight: 600;
                            @media (max-width: @screen-xs) {
                                border-radius: 8px;
                            }
                        }
                        .enterIcon {
                            fill: #fff;
                        }
                    }
                    .subscribeFree {
                        font-weight: 600;
                        font-size: 16px;
                    }
                }
                .detailsMobile {
                    font-weight: 600;
                    .numericPrice {
                        font-size: 18px;
                        font-weight: bold;
                    }
                    .hour {
                        font-weight: normal;
                    }
                    .subscribeFree {
                        font-size: 16px;
                    }
                }
            }
        }
        .icons {
            @media(max-width: @screen-xs) {
                width: 100%;
                margin-top: 2px;
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