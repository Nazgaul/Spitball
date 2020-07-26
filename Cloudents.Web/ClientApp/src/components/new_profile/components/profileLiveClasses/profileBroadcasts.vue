<template>
    <div v-if="broadcastSessions.length">
        <div class="profileBroadcast pa-4 pb-0 pa-sm-0">
            <div class="mainTitle text-sm-center mb-5">{{broadCastTitle}}</div>
            <div 
                v-for="session in sessionsList"
                class="broadcastList"
                :class="{'expandLastChild': isExpand}"
                :key="session.id">
                <div class="d-sm-flex listWrapper py-sm-5">
                    <div class="leftSide d-sm-flex me-sm-6">
                        <img class="cursor-pointer" @click="goStudyRoomLandingPage(session.id)"  :src="liveImage(session)" alt="" width="320" height="210">
                    </div>
                    <div class="rightSide d-flex flex-column justify-space-between flex-grow-1 px-3 pa-3 pa-sm-0 pt-2 pt-sm-0 pe-0 ps-0 pe-sm-4">
                        <div class="occurrenceWrap mb-5 mb-sm-2">
                            <div class="sessionTitle mb-2">{{session.name}}</div>
                            <div class="d-flex align-center flex-wrap flex-sm-nowrap">
                                <div class="d-flex align-center justify-space-between flex-grow-1 flex-sm-grow-0">
                                    <div class="d-flex align-center">
                                        <div class="occurrenceDot">{{$moment(session.created).format('ddd, DD MMM')}}</div>
                                        <!-- <div class="orangeDot" v-if="session.nextEvents"></div> -->
                                    </div>

                                    <div class="d-flex align-center">
                                        <div class="orangeDot mx-2" v-if="session.nextEvents || !isMobile"></div>
                                        <div class="occurrenceDot">{{$moment(session.nextEvents ? getEventDays(session).start : session.created).format('h:mm a')}}</div>
                                    </div>

                                    <div class="d-flex align-center" v-if="session.nextEvents">
                                        <div class="orangeDot mx-2"></div>
                                        <div class="occurrenceDot">{{$tc('session', getEventDays(session).times)}}</div>
                                    </div>
                                </div>

                                <div class="d-flex align-center" v-if="session.nextEvents">
                                    <div class="orangeDot mx-sm-2 me-2"></div>
                                    <div class="occurrenceDot">{{$t('live_every',[getEventDays(session).days])}}</div>
                                </div>
                            </div>
                        </div>
                        <div class="center" dir="auto">
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
                                    @click="goStudyRoomLandingPage(session.id)"
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
                                    @click="goStudyRoomLandingPage(session.id)"
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
        <stripe ref="stripe"></stripe>
    </div>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames';
import enterIcon from './enterRoom.svg'
import arrowDownIcon from './group-3-copy-16.svg'
import stripe from "../../../pages/global/stripe.vue";

export default {
    name: 'profileLiveClasses',
    components: {
        enterIcon,
        arrowDownIcon,
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
            isExpand: false,
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
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        textLimit(){
            return this.isMobile ? 110 : 200;
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
        goStudyRoomLandingPage(id){
            this.$router.push({
                name: routeNames.StudyRoomLanding,
                params: {id}
            })
        },
        liveImage(session) {
            return this.$proccessImageUrl(session.image, 320, 212, 'crop')
        },
        getEventDays({nextEvents}) {
            return this.$store.getters.getSessionRecurring(nextEvents)
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
        // margin-top: 12px;
        font-size: 16px;
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
                        height: 100%;
                    }
                }
            }
            .occurrenceWrap {
                .responsive-property(font-size, 14px, null, 16px);
                // .occurrenceDot {
                //     margin: 0 8px;
                // }
                // .occurrenceDot:first-child {
                //     margin-left: 0;
                // }
                // .occurrenceDot:last-child {
                //     margin-right: 0;
                // }
                .orangeDot {
                    width: 6px;
                    height: 6px;
                    right: 8px;
                    top: 7px;
                    border-radius: 50%;
                    background-color: #ff6f30;
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
    // .dateTime {
    //     color: #363637;
    //     font-weight: 600;
    //     font-size: 15px;
    //     vertical-align: middle;
    // }
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