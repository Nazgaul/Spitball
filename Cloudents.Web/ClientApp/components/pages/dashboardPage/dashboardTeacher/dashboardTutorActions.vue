<template>
    <div class="dashboardTutorActions pa-5 pb-0 mb-2 mb-sm-4">

        <div class="tutorInfo d-flex align-center justify-space-between flex-column flex-sm-row pb-6">
            <div class="leftSide d-flex align-md-center">
                <userAvatar
                    size="74"
                    class="mb-4"
                    :userId="userId"
                    :userName="userName"
                    :userImageUrl="userImage"
                    v-if="userImage"
                />
                <emptyUserIcon class="mb-4" v-else />
                <div class="infoWrap mx-5">
                    <div class="tutorName text-truncate mb-2">{{userName}}</div>
                    <button class="tutorUrl me-4 mb-4" :class="{'text-truncate': isMobile}" @click="$router.push(myProfileRedirect)">{{userUrl}}</button>
                    <v-btn 
                        class="btn align-self-end"
                        :to="myProfileRedirect"
                        v-if="isEditActionComplete"
                        rounded
                        outlined
                        depressed
                        color="#4c59ff"
                        width="120"
                        dense
                        height="34"
                    >
                        <span v-t="'dashboardTeacher_btn_edit'"></span>
                    </v-btn>
                </div>
            </div>
            <div class="rightSide mt-8 mt-sm-0">
                <video class="dashboardVideo" ref="dashboardTutor" :src="onBoardingVideo" width="250" height="150" poster="./images/group-14-copy-2@2x.png" controls></video>
            </div>
        </div>

        <div class="tutorLinks">
            <div class="linkWrap d-flex align-center justify-space-between py-4" v-for="action in tutorActionsFilterList" :key="action.name">
                <div class="linkBorded" :style="{background: action.color}"></div>
                <div class="link d-flex align-center" :class="{'mobileLayout': isMobile}">
                    <component 
                        :is="action.routeName ? 'router-link' : 'div'"
                        class="linkWrapper d-flex"
                        @click="action.method ? action.method() : ''" 
                        :to="action.routeName ? action.routeName : ''"
                    >
                        <circleArrow class="arrowIcon" width="23" :stroke="action.color" />
                    </component>
                    <div class="ms-sm-4 me-2 text-truncate" v-t="action.text"></div>
                </div>
                <v-btn 
                    v-if="!isMobile" 
                    @click="action.method ? action.method() : ''"
                    :to="action.routeName ? action.routeName : ''"
                    v-t="action.btnText"
                    class="btn"
                    rounded
                    outlined
                    exact
                    depressed
                    color="#4c59ff"
                    width="120"
                >
                </v-btn>
            </div>
        </div>

        <analyticOverview class="px-0" />


        <v-snackbar
            absolute
            top
            :timeout="4000"
            :value="verifyEmailState"
        >
            <div class="text-wrap" v-t="'dashboardTeacher_email_verify'"></div>
        </v-snackbar>
    </div>
</template>

<script>
import * as routeName from '../../../../routes/routeNames'
import constants from '../../../../store/constants/dashboardConstants'

const analyticOverview = () => import(/* webpackChunkName: "analyticsOverview" */'../../global/analyticOverview/analyticOverview.vue')

import emptyUserIcon from './images/emptyUser.svg'
import circleArrow from './images/circle-arrow.svg'

const colors = {
    blue: '#4c59ff',
    green: '#41c4bc',
    yellow: '#eac569',
}

export default {
    name: 'dashboardTutorActions',
    components: {
        analyticOverview,
        emptyUserIcon,
        circleArrow
    },
    data() {
        return {
            video: null,
            verifyEmailState: false,
            profileName: routeName.Profile,
            linksItems: {
                [constants.PHONE]: {
                    color: colors.blue,
                    text: 'dashboardTeacher_link_text_phone',
                    btnText: 'dashboardTeacher_btn_text_phone',
                    method: this.openPhoneDialog
                },
                [constants.EMAIL]: {
                    color: colors.blue,
                    text: 'dashboardTeacher_link_text_email',
                    btnText: 'dashboardTeacher_btn_text_email',
                    method: this.verifyEmail
                },
                [constants.EDIT]: {
                    color: colors.blue,
                    text: 'dashboardTeacher_link_text_edit',
                    btnText: 'dashboardTeacher_btn_text_edit',
                    routeName: { name: routeName.Profile }
                },
                [constants.BOOK]: {
                    color: colors.blue,
                    text: 'dashboardTeacher_link_text_book',
                    btnText: 'dashboardTeacher_btn_text_book',
                    method: this.bookSession
                },
                [constants.COURSES]: {
                    color: colors.blue,
                    text: 'dashboardTeacher_link_text_courses',
                    btnText: 'dashboardTeacher_btn_text_courses',
                    routeName: { name: routeName.EditCourse }
                },
                [constants.STRIPE]: {
                    color: colors.blue,
                    text: 'dashboardTeacher_link_text_stripe',
                    btnText: 'dashboardTeacher_btn_text_stripe',
                    method: this.addStripe
                },
                [constants.CALENDAR]: {
                    color: colors.green,
                    text: 'dashboardTeacher_link_text_calendar',
                    btnText: 'dashboardTeacher_btn_text_calendar',
                    routeName: { name: routeName.MyCalendar }
                },
                [constants.TEACH]: {
                    color: colors.green,
                    text: 'dashboardTeacher_link_text_teach',
                    btnText: 'dashboardTeacher_btn_text_teach',
                    routeName: { name: routeName.MyCalendar }
                },
                [constants.SESSIONS]: {
                    color: colors.yellow,
                    text: 'dashboardTeacher_link_text_session',
                    btnText: 'dashboardTeacher_btn_text_session',
                    routeName: { name: routeName.MyStudyRoomsBroadcast }
                },
                [constants.UPLOAD]: {
                    color: colors.yellow,
                    text: 'dashboardTeacher_link_text_upload',
                    btnText: 'dashboardTeacher_btn_text_upload',
                    routeName: { name: routeName.MyContent }
                },
            }
        }
    },
    computed: {
        isEditActionComplete() {
            return this.$store.getters.getTutorListActions[constants.EDIT]?.value
        },
        bookSessionTutorId() {
            return this.$store.getters.getTutorListActions[constants.BOOK]?.tutorId
        },
        tutorActionsFilterList() {
            return this.tutorActionsList.filter(action => action.value === false)
        },
        tutorActionsList() {
            let list = this.$store.getters.getTutorListActions
            return Object.keys(list).map(item => {
                return {
                    ...this.linksItems[item],
                    ...list[item],
                    name: item
                }
            })
        },
        onBoardingVideo() {
            return require('./OnboardingVideo.mp4').default
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        userId() {
            return this.$store.getters.getAccountId
        },
        userName() {
            return this.$store.getters.getAccountName
        },
        userImage() {
            return this.$store.getters.getAccountImage
        },
        userUrl() {
            return `${window.location.origin}/profile/${this.userId}/${this.userName}`
        },
        myProfileRedirect() {
            return {
                name: this.profileName,
                params: {
                    id: this.userId,
                    name: this.userName
                }
            }
        }
    },
    methods: {
        openPhoneDialog() {
            this.$store.commit('setComponent', 'verifyPhone')
        },
        bookSession() {
            this.$router.push({
                name: routeName.Profile,
                params: {
                    name: 'admin',
                    id: this.bookSessionTutorId
                }
            })
        },
        verifyEmail() {
            this.$store.dispatch('verifyTutorEmail').then(() => {
                this.verifyEmailState = true
                this.$store.commit('setEmailTaskComplete')
            }).catch(ex => {
                self.$appInsights.trackException(ex);
            })
        },
        addStripe() {
            window.location = '/stripe-connect'
        },
        addEventToVideo() {
            this.$ga.event("Dashboard", "Watching spitball video");
        }
    },
    beforeDestroy() {
        this.video.addEventListener("play", this.addEventToVideo);
    },
    created() {
        this.$store.dispatch('updateTutorLinks')
    },
    mounted() {
        this.video = this.$refs.dashboardTutor
        this.video.addEventListener("play", this.addEventToVideo);
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';

    .dashboardTutorActions {
        background: #fff;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 8px;
        margin: 0 auto;
        @media (max-width: @screen-xs) {
            box-shadow: none;
            padding: 14px;
            border-radius: 0;
        }
        .tutorInfo {
            border-bottom: 2px solid #f5f5f5;
            font-weight: 600;

            .leftSide {
                min-width: 0;
                width: 100%;
                .infoWrap {
                    min-width: inherit;
                }
            }
            .rightSide {

                .dashboardVideo {
                    @media (max-width: @screen-xs) {
                        height: 100%;
                        width: 100%;
                        object-fit: cover;
                    }
                }
            }
            .tutorName {
                font-size: 22px;
                color: @global-purple;
            }
            .tutorUrl {
                color: @global-auth-text;
                outline: none;
                text-align: left;
                @media (max-width: @screen-xs) {
                    width: 100%;
                }
            }
        }
        .tutorLinks {

            .linkWrap {
                position: relative;
                border-bottom: 2px solid #f5f5f5;
                .linkBorded {
                    position: absolute;
                    left: -20px;
                    width: 5px;
                    height: 45px;
                    border-radius: 0 6px 6px 0;
                }
                .link {
                    min-width: 0;
                    font-size: 15px;
                    color:#69687d;
                    font-weight: 600;
                    .arrowIcon {
                        transform: scaleX(1)/*rtl:append:scaleX(-1)*/; 
                        width: auto; //only to support the rtl rule
                    }

                    &.mobileLayout {
                        justify-content: space-between;
                        flex: 1;
                        .linkWrapper {
                            order: 1;
                        }
                    }
                }
            }
        }
        .btn {
            font-weight: 600;
        }
    }
</style>