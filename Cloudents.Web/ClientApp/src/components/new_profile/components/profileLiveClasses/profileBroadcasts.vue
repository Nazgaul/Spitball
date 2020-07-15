<template>
    <div v-if="broadcastSessions.length">
        <div class="profileBroadcast pa-4 pb-0 pa-sm-0">
            <div class="mainTitle text-sm-center mb-5">{{broadCastTitle}}</div>
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
                        <img :src="liveImage(session)" alt="">
                    </div>
                    <div class="rightSide d-flex flex-column justify-space-between flex-grow-1 pa-3 pt-2 pt-sm-2 pe-0 ps-0 pe-sm-4">

                        <div class="header d-flex justify-space-between mb-3" v-if="!isMobile">
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
                            <div class="sessionTitle mb-2" v-if="!isMobile">{{session.name}}</div>
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

                        <div class="bottom d-flex align-end justify-space-between text-center" :class="{'mt-5': session.description}">
                                <v-btn
                                    v-if="isMyProfile || session.enrolled"
                                    @click="enterRoomById(session.id)"
                                    class="white--text btn"
                                    rounded
                                    depressed
                                    color="#ff6f30"
                                    height="40"
                                >
                                    <enterIcon class="enterIcon me-sm-2" width="18" />
                                    <span :class="{'flex-sm-grow-1 ps-2': isMobile}" v-t="'enter'"></span>
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
                                    <span :class="{'flex-sm-grow-1 ps-2': isMobile}" v-t="'full'"></span>
                                </v-btn>
                                <v-btn
                                    v-else
                                    @click="enrollSession(session)"
                                    :loading="enrollBtnLoader"
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
                                <span class="number text-left ms-sm-1">{{$price(session.price.amount, session.price.currency, true)}}</span>
                            </div>
                            <div class="subscription" v-if="isTutorSubscription">
                                <span v-t="'subscriber'"></span>
                                <span class="number text-left ms-sm-1">{{$price(0, session.price.currency, true)}}</span>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

        <div class="showMore text-center mt-n2"  v-if="broadcastSessions.length > 2">
            <v-btn class="showBtn" color="#fff" fab depressed small dark @click="isExpand = !isExpand">
                <arrowDownIcon class="arrowIcon" :class="{'exapnd': isExpand}" width="22" />
            </v-btn>
        </div>

        <v-snackbar
            top
            :timeout="5000"
            :color="color"
            @input="showSnack = false"
            :value="showSnack"
        >
            <div class="text-center white--text">{{toasterText}}</div>
        </v-snackbar>
        <stripe ref="stripe"></stripe>
    </div>
</template>

<script>
import { StudyRoom } from '../../../../routes/routeNames'

import enterIcon from './enterRoom.svg'
import arrowDownIcon from './group-3-copy-16.svg'
import stripe from "../../../pages/global/stripe.vue";

export default {
    name: 'profileLiveClasses',
    components: {
        enterIcon,
        arrowDownIcon  ,
        stripe
    },
    props: {
        userId: {
            required: true
        }
    },
    data() {
        return {
            defOpen:false,
            showSnack: false,
            color: '',
            toasterText: '',
            isExpand: false,
            enrollBtnLoader: false,
        }
    },
    watch: {
        isExpand(val) {
            if(!val) {
                this.$vuetify.goTo(this.$parent.$refs.profileLiveClassesElement)
            }
        }
    },
    computed: {
        broadCastTitle() {
            return this.isMobile ? this.$t('my_live_classes_mobile') : this.$t('my_live_classes')
        },
        // buttonShowMore() {
        //     return !this.isExpand ?  this.$t('See all live classes') : this.$t('See less live classes')
        // },
        // liveImage() {
        //     return this.$proccessImageUrl(this.message.src, 190, 140, 'crop')
        //     // return this.isMobile ? require('./live-banner-mobile.png') : require('./live-banner-desktop.png')
        // },
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
        broadcastSessions() {
            return this.$store.getters.getProfileLiveSessions
        },
        liveSessionsList() {
            let liveList = this.broadcastSessions
            if(this.isExpand) {
                return liveList
            }
            return liveList.slice(0, 2)
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
            return this.isMobile ? 110 : 214;
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
        liveImage(session) {
            let imageUrl = `https://spitball-dev-function.azureedge.net/api/image/studyroom/${session.id}`
            return this.$proccessImageUrl(imageUrl, 330, 220, 'crop')
        },
        enterRoomById(id){
            let routeData = this.$router.resolve({
                name: StudyRoom,
                params: { id }
            });
            global.open(routeData.href, "_self");
        },
        async enrollSession(session) {
            if(!this.isLogged) {
                sessionStorage.setItem('hash','#broadcast');
                this.$store.commit('setComponent', 'register')
                return
            }
            let sessionObj = {
                userId: this.userId,
                studyRoomId: session.id
            }
            if (session.price?.amount && this.$store.getters.getProfileCountry !== 'IL' && !this.$store.getters.getIsSubscriber) {
                let x = await this.$store.dispatch('updateStudyroomLiveSessionsWithPrice', sessionObj);
                this.$refs.stripe.redirectToStripe(x);
                return;
            }
           
            let self = this
            this.enrollBtnLoader = true
            this.$store.dispatch('updateStudyroomLiveSessions', sessionObj)
                .then(() => {
                    self.toasterText = this.$t('profile_enroll_success')
                    let currentSession = self.broadcastSessions.filter(s => s.id === session.id)[0]
                    currentSession.enrolled = true
                }).catch(ex => {
                    self.color = 'error'
                    self.toasterText = this.$t('profile_enroll_error')
                    self.$appInsights.trackException(ex);
                }).finally(() => {
                    this.enrollBtnLoader = false
                    self.showSnack = true
                })
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
        }
    },
    created() {
        this.$store.dispatch('getStudyroomLiveSessions', this.userId)
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
        margin: 0 auto;
        font-weight: 600;
        color: #363637;
        
        @media(max-width: @screen-xs) {
            padding: 0;
            font-size: 23px;
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
        opacity: 0;
        visibility: hidden;
        transition: all .6s;
    }
    .toggleCheckbox[type=checkbox]:checked  {
        & ~.description {
            display: none !important;
        }
        & ~ .restOfText {
            //label
            line-height: 1.5;
            opacity: 1;
            height: auto;
            visibility: visible;
        }
        & ~label {
            display: none;
        }
    }
    .broadcastList {
        margin-bottom: 28px;
        // &:nth-child(3) {
        //     margin-bottom: 20px;   
        // }
        &.expandLastChild {
            &:nth-child(3) {
                margin-bottom: 28px;   
            }
            // &:last-child{
            //     margin-bottom: 20px;
            // }
            
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
                        font-size: 16px;
                        line-height: 1.5;
                        display: contents;
                    }
                    .readMore {
                        font-weight: 600;
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
                            font-size: 16px;
                            display: block;
                        }
                    }
                    .subscription {
                        padding: 0 0 0 10px;
                        @media(max-width: @screen-xs) {
                            font-size: 14px;
                        }
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
    margin-bottom: 60px;
    @media (max-width: @screen-xs) {
        margin-bottom: unset;
    }
    .showBtn {
        border: 1px solid #d4d6da !important;
    }
    .arrowIcon {
        padding-top: 1px;
        cursor: pointer;
        &.exapnd {
            transform: scaleY(-1);
        }
    }
}
</style>