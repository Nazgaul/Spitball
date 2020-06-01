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
                    <div class="tutorName mb-2">{{userName}}</div>
                    <button class="tutorUrl text-truncate me-4 mb-4">{{userUrl}}</button>
                    <v-btn 
                        class="btn align-self-end"
                        :to="{
                            name: profileName,
                            params: {
                                id: userId,
                                name: userName
                            }
                        }"
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
                <video class="dashboardVideo" src="" width="250" height="150" poster="./images/bitmap.png"></video>
            </div>
        </div>

        <div class="tutorLinks">
            <div class="linkWrap d-flex align-center justify-space-between py-4" v-for="action in tutorActionsFilterList" :key="action.name">
                <div class="linkBorded" :style="{background: action.color}"></div>
                <div class="link d-flex align-center" :class="{'mobileLayout': isMobile}">
                    <component :is="isMobile ? 'router-link' : 'div'" class="linkWrapper d-flex" @click="action.method ? action.method() : ''" :to="isMobile ? action.routeName : null">
                        <circleArrow class="arrowIcon" width="23" :stroke="action.color" />
                    </component>
                    <div class="ms-sm-4 me-2 text-truncate" v-t="action.text"></div>
                </div>
                <v-btn v-if="!isMobile" class="btn" rounded outlined exact depressed color="#4c59ff" width="120" @click="action.method ? action.method() : ''" :to="action.routeName" v-t="action.btnText"></v-btn>
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
                    routeName: { name: routeName.Feed }//TODO: stripe // need to do 
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
                    routeName: { name: routeName.MyStudyRooms }
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
            return `${window.location.origin}/${this.userName}`
        }
    },
    methods: {
        openPhoneDialog() {
            this.$store.commit('setComponent', 'phoneVerify')
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
                this.$store.commit('setEmailTask', constants.EMAIL)
            }).catch(ex => {
                console.log(ex);
            })
        }
    },
    created() {
        this.$store.dispatch('updateTutorLinks')
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

                @media (max-width: @screen-xs) {
                    width: 100%;
                }
                .dashboardVideo {
                    width: 100%;
                    height: 100%;
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
                        transform: none /*rtl:scaleX(-1)*/;
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