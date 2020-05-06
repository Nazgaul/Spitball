<template>
    <div class="profileUserBox pa-4 pa-sm-5" v-if="currentProfileUser">
        <div class="profileUserBox_top d-block d-sm-flex justify-space-between">
            <div class="leftSide mr-sm-6 mb-2 mb-sm-0 d-flex justify-center">
                <div class="pUb_dot" sel="online_icon" v-if="isOnline"></div>
                <uploadImage sel="photo" class="pUb_edit_img" v-if="isCurrentProfileUser" />
                <userAvatarRect sel="avatar_image"
                    class="pUb_dS_img"
                    :userName="currentProfileUser.name"
                    :userImageUrl="currentProfileUser.image"
                    :width="isMobile? 130: 190"
                    :height="isMobile? 161 : 235"
                    :userId="currentProfileUser.id"
                    :fontSize="36"
                    :borderRadius="8"
                />
            </div>
            <div class="rightSide flex-grow-1">
                <div class="detailsWrap d-flex d-sm-block">
                    <div class="d-flex justify-space-between text-center text-sm-left">
                        <h1  sel="username_title" class="userName text-truncate mr-sm-2">
                            <span v-if="currentProfileUser.isTutor" class="mr-1" v-t="'profile_tutor'"></span>
                            <span>{{currentProfileUser.name}}</span>
                        </h1>

                        <div class="profileUserSticky_pricing text-center" v-if="!isMobile">
                            <template v-if="currentProfileUser.isTutor">
                                <div class="d-flex align-end justify-center">
                                    <div class="profileUserSticky_pricing_discount mr-2" v-if="isDiscount">
                                        {{tutorPrice ? currencySymbol(tutorPrice) : currencySymbol(tutorDiscountPrice)}}
                                    </div>
                                    <div class="profileUserSticky_pricing_price">
                                        <span class="profileUserSticky_pricing_price_number">{{isDiscount && tutorPrice !== 0  ? currencySymbol(tutorDiscountPrice) : currencySymbol(tutorPrice)}}</span>/<span class="profileUserSticky_pricing_price_hour" v-t="'profile_points_hour'"/>
                                    </div>
                                </div>
                                <button sel="coupon" :class="{'isMyProfileCoupon': isCurrentProfileUser}" v-if="currentProfileUser.isTutor" class="profileUserSticky_coupon" @click="globalFunctions.openCoupon" v-t="'coupon_apply_coupon'"/>
                            </template>
                            <div v-else>
                                <v-btn :to="{name: routeNames.EditCourse}" v-ripple="false" icon text v-if="isLogged && !currentProfileUser.isTutor">
                                    <editSVG class="mr-1" v-if="isCurrentProfileUser" />
                                </v-btn>
                            </div>
                        </div>
                    </div>

                    <!-- Rate And Follower -->
                    <div class="rateWrap d-flex mb-3 justify-center justify-sm-start" :class="[!currentProfileUser.isTutor ? 'mt-sm-n0' : 'mt-sm-n4']">
                        <template v-if="currentProfileUser.isTutor">
                            <div  class="pUb_dS_c_rating">
                                  <v-rating  v-model="currentProfileTutor.rate" color="#ffca54" background-color="#ffca54"
                                        :length="currentProfileTutor.reviewCount > 0  ? 5 : 1"
                                            :size="18" readonly />
                                    <span  span @click="scrollToReviews" class="pUb_dS_c_r_span ml-1">{{$tc('resultTutor_review_one',currentProfileTutor.reviewCount)}}</span>
                                <!-- <span class="no-reviews font-weight-bold caption" v-t="'resultTutor_no_reviews'"></span> -->
                            </div>
                        </template>
                        <div class="ml-3">
                            <followBtn sel="follow_btn" class="followBtnNew mr-sm-2" v-if="!isCurrentProfileUser"/>
                        </div>
                    </div>

                    <!-- courses teacher -->
                    <div sel="teach_courses" class="course mt-sm-3 mb-sm-3 mt-2 mb-3 text-truncate text-center text-sm-left" v-if="currentProfileUser.isTutor && currentProfileUser.courses.length">
                        <bdi class="iTeach mr-1" v-t="'profile_my_courses_teacher'"></bdi>
                        <span class="courseName text-truncate">{{currentProfileUser.coursesString}}</span>
                    </div>

                    <!-- TUTOR BIO -->
                    <div class="userBio mb-5 mb-sm-0 mr-sm-2" v-if="currentProfileTutor.bio">{{currentProfileTutor.bio | truncate(isOpen, '...', textLimit)}}
                        <div v-if="isOpen" class="my-4">
                            <div class="course mb-1 text-truncate text-center text-sm-left" v-if="currentProfileUser.isTutor && currentProfileUser.courses.length">
                                <bdi class="iTeach mr-1" v-t="'profile_my_courses'"></bdi>
                                <span class="courseName text-truncate">{{currentProfileUser.coursesString}}</span>
                            </div>
                            <div class="course text-truncate text-center text-sm-left" v-if="currentProfileUser.isTutor && currentProfileUser.courses.length">
                                <bdi class="iTeach mr-1" v-t="'profile_my_subjects'"></bdi>
                                <span class="courseName text-truncate">{{currentProfileTutor.subjects.toString().replace(/,/g, ", ")}}</span>
                            </div>
                        </div>
                        <div class="d-none">
                            <div>{{currentProfileTutor.bio | restOfText(isOpen, '...', textLimit)}}</div>
                        </div>
                        <span sel="bio_more" @click="isOpen = !isOpen" class="readMore" v-t="isOpen ? 'profile_read_less' : 'profile_read_more'"></span>
                    </div>

                    <!-- Courses Student -->
                    <div class="course mt-2 text-truncate" v-if="!currentProfileUser.isTutor && currentProfileUser.courses.length">
                        <span class="profileUserBox_bottom_title mr-1" v-t="'profile_my_courses_student'"></span>
                        <span v-for="(course, index) in currentProfileUser.courses" :key="index">
                            {{course}}{{index + 1 == currentProfileUser.courses.length ? '' : ', '}}
                        </span>
                    </div>
                </div>

                <div class="profileUserSticky_btns d-block d-sm-flex align-end text-center mt-sm-1" :class="{'student': !currentProfileUser.isTutor && isCurrentProfileUser}">
                    <template v-if="isMobile">
                        <div class="profileUserSticky_pricing mb-4" v-if="currentProfileUser.isTutor">
                            <div class="d-flex align-end justify-center">
                                <div class="profileUserSticky_pricing_discount mr-2" v-if="isDiscount">
                                    {{tutorPrice ? currencySymbol(tutorPrice) : currencySymbol(tutorDiscountPrice)}}
                                </div>
                                <div class="profileUserSticky_pricing_price">
                                    <span class="profileUserSticky_pricing_price_number">{{isDiscount && tutorPrice !== 0  ? currencySymbol(tutorDiscountPrice) : currencySymbol(tutorPrice)}}</span>/<span class="profileUserSticky_pricing_price_hour" v-t="'profile_points_hour'"/>
                                </div>
                            </div>
                            <button sel="coupon" :class="{'isMyProfileCoupon': isCurrentProfileUser}" class="profileUserSticky_coupon text-center mt-1" @click="globalFunctions.openCoupon" v-t="'coupon_apply_coupon'"/>
                        </div>
                        <div class="text-sm-right text-center mb-2" v-if="isCurrentProfileUser">
                            <editSVG sel="edit" class="pUb_edit_user" @click="openEditInfo"/>
                        </div>
                        <v-btn :to="{name: routeNames.EditCourse}" v-ripple="false" icon text v-if="isLogged && !currentProfileUser.isTutor">
                            <editSVG v-if="isCurrentProfileUser" />
                        </v-btn>
                    </template>
                    <v-btn sel="send" height="42" :width="isMobile ? 286 : 220" :disabled="isCurrentProfileUser" v-if="currentProfileUser.isTutor" class="profileUserSticky_btn white--text mr-sm-4" :class="{'isMyProfile': isCurrentProfileUser}" depressed rounded color="#4c59ff" @click="globalFunctions.sendMessage">
                        <chatSVG class="profileUserSticky_btn_icon"/>
                        <div class="profileUserSticky_btn_txt" v-t="'profile_send_message'"/>
                    </v-btn>
                    <div class="calendarBtnWrap align-center align-sm-end" :class="{'ml-3': !getProfile.user.calendarShared}">
                        <editSVG sel="edit" class="pUb_edit_user mr-1" v-if="isCurrentProfileUser && !isMobile" @click="openEditInfo"/>
                        <v-btn
                            @click="globalFunctions.openCalendar"
                            class="profileUserSticky_btn profileUserSticky_btn_book white--text mt-sm-2 mt-4"
                            :class="{'hideCalendarBtn': !getProfile.user.calendarShared}"
                            :disabled="!getProfile.user.calendarShared"
                            :width="isMobile ? 286 : 220"
                            sel="calendar"
                            height="42"
                            color="white"
                            depressed
                            rounded
                        >
                            <calendarSVG width="20" class="profileUserSticky_btn_icon"/>
                            <div class="profileUserSticky_btn_txt" v-t="calendarBtnResource"/>
                        </v-btn>
                    </div>
                </div>
            </div>
        </div>

        <v-row class="bottom text-center pt-3" dense v-if="currentProfileTutor">
            <v-col cols="6" sm="3" class="bottomBox d-flex align-center justify-center pa-2 pa-sm-0">
                <followersSvg class="icon" width="24" />
                <div class="ml-3" @click="isMobile ? scrollToReviews():''" >
                    <div class="number text-left">{{currentProfileUser.followers || 0}}</div>
                    <div class="type">{{$tc('profile_tutor_follower', currentProfileUser.followers)}}</div>
                </div>
            </v-col>
            <v-col cols="6" sm="3" class="bottomBox d-flex align-center justify-center pa-2 pa-sm-0">
                <onlineLessonSVG class="icon" width="17" />
                <div class="ml-3">
                    <div class="number text-left">{{currentProfileTutor.lessons || 0}}</div>
                    <div class="type" v-t="''">{{$tc('profile_session', currentProfileTutor.lessons)}}</div>
                </div>
            </v-col>
            <v-col cols="6" sm="3" class="bottomBox d-flex align-center justify-center pa-3 pa-sm-0">
                <studentsSVG class="icon" width="25" />
                <div class="ml-3">
                    <div class="number text-left">{{currentProfileTutor.students || 0}}</div>
                    <div class="type">{{$tc('profile_student', currentProfileTutor.students)}}</div>
                </div>
            </v-col>
            <v-col cols="6" sm="3" class="bottomBox d-flex align-center justify-center pa-2 pa-sm-0">
                <starSVG class="icon" width="22" />
                <div class="ml-3">
                    <div class="number text-left">{{currentProfileTutor.reviewCount || 0}}</div>
                    <div class="type">{{$tc('profile_reviews',currentProfileTutor.reviewCount)}}</div>
                </div>
            </v-col>
        </v-row>
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';

import starSVG from './images/star.svg';
import studentsSVG from './images/students.svg';
import onlineLessonSVG from './images/onlineLesson.svg';
import followersSvg from './images/followers.svg';
import editSVG from './images/edit.svg';
import chatSVG from '../profileUserSticky/images/chatIcon_mobile.svg';
import calendarSVG from '../profileUserSticky/images/calendarIcon.svg';

import * as routeNames from '../../../../routes/routeNames'

import userAvatarRect from '../../../helpers/UserAvatar/UserAvatarRect.vue';
import uploadImage from '../../profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue';
import followBtn from '../followBtn/followBtn.vue';

export default {
    name:'profileUserBox',
    components:{
        starSVG,
        studentsSVG,
        onlineLessonSVG,
        followersSvg,
        userAvatarRect,
        uploadImage,
        editSVG,
        followBtn,
        chatSVG,
        calendarSVG
    },
    props: {
        globalFunctions:{}
    },
    data() {
        return {
            defOpen:false,
            routeNames
        }
    },
    computed: {
        ...mapGetters(['getProfile','accountUser','getUserStatus', 'getUserLoggedInStatus']),
        isLogged() {
            return this.getUserLoggedInStatus
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        currentProfileUser(){
            if(!!this.getProfile){
                return this.getProfile.user
            }else{
                return false;
            }
        },
        currentProfileTutor(){
            if(!!this.currentProfileUser){
                return this.currentProfileUser.tutorData
            }else{
                return false;
            }
        },
        isOnline(){
            return this.getUserStatus[this.currentProfileUser.id] || false;
        },
        textLimit(){
            return this.isMobile ? 140 : 200;
        },
        isOpen :{
            get(){
                return this.defOpen
            },
            set(val){
                this.defOpen = val
            }
        },
        isCurrentProfileUser(){
            if (!!this.currentProfileUser && !!this.accountUser ){
                return this.currentProfileUser.id == this.accountUser.id;
            }else{
                return false;
            }
        },
        isDiscount() {
            return !!this.getProfile && (this.getProfile.user.tutorData.discountPrice || this.getProfile.user.tutorData.discountPrice === 0)
        },
        tutorDiscountPrice() {
            return !!this.getProfile && this.getProfile.user.tutorData.discountPrice ? this.getProfile.user.tutorData.discountPrice : null;
        },
        tutorPrice() {
            if (this.getProfile.user?.tutorData) {
                return this.getProfile.user.tutorData.price;
            }
            return 0;
        },
        calendarBtnResource() {
            return this.isCurrentProfileUser ? 'profile_my_book_session' : 'profile_book_session'
        }
    },
    methods: {
        ...mapActions(['updateEditDialog']),
        currencySymbol(amount) {
            let options = { style: 'currency', currency: this.currentProfileUser.tutorData.currency, minimumFractionDigits: 0 };
            let numberFormat = new Intl.NumberFormat('he-IL', options);

            return numberFormat.format(amount)
        },
        openEditInfo() {
            this.updateEditDialog(true);
        },
        scrollToReviews(){
            if(!this.currentProfileTutor.reviewCount > 0){
                return
            }
            let scrollIntoViewOptions = {
                behavior: 'smooth',
                block: 'center',
            }
            document.querySelector('.profileReviewsBox').scrollIntoView(scrollIntoViewOptions);
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
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.profileUserBox {
    max-width: 762px;
    // width: 100%;
    margin: 0 auto;
    border-radius: 8px;
    box-shadow: 0 0 24px 0 rgba(0, 0, 0, 0.38);
    background-color: #ffffff;
    position: relative;
    z-index: 2;
    @media (max-width: @screen-xs) {
        border-radius: 0;
        box-shadow: none;
        padding: 0;
    }
    .profileUserBox_top{
        margin-bottom: 20px;
        @media (max-width: @screen-xs) {
            // justify-content: center;
            justify-content: flex-start;
            // height: 126px;
            position: relative;
            margin: -100px 0 16px 0;
        }
    }

    .leftSide {
        position: relative;
        margin: 0 auto;
        width: max-content;
        @media (max-width: @screen-xs) {
            padding: 8px 6px;
            background: #fff;
            border-radius: 8px;
        }
        .pUb_dot {
            position: absolute;
            left: 8px;
            top: 8px;
            width: 16px;
            height: 16px;
            border-radius: 50%;
            background-color: #6aff70;
            @media (max-width: @screen-xs) {
                left: 10px;
                top: 10px;
                width: 12px;
                height: 12px;
            }
        }
        .pUb_dS_img{
            pointer-events: none !important;
        }
        .pUb_edit_img{
            position: absolute;
            right: 4px;
            text-align: center;
            width: 36px;
            height: 46px;
            border-radius: 4px;
            background-color: #fff;
        }
    }

    .rightSide {
        color: @global-purple;
        min-width: 0;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        .detailsWrap {
            @media (max-width: @screen-xs) {
                flex-direction: column;
            }
            .userName{
                .responsive-property(font-size, 24px, null, 22px);
                font-weight: 600;
                width: 100%;
            }
            .course {
                font-weight: 600;
                .responsive-property(font-size, 16px, null, 14px);
            }
            .rateWrap {
                @media (max-width: @screen-xs) {
                    order: 1;
                }
                .pUb_dS_c_rating{
                    display: inline-flex;
                    align-items: center;
                    .no-reviews {
                        margin-left: 5px;
                        color: @global-purple;
                        font-size: 12px !important;
                        margin-top: 2px;
                    }
                    .pUb_dS_c_rating_star {
                        width: 18px;
                        vertical-align: sub !important;
                    }
                    .c_rating{
                        flex: 0 0 auto;
                        &.rating-container{
                            .v-rating{
                                .v-icon{
                                    padding-right: 1px;
                                }
                            }
                        }
                    }
                    .pUb_dS_c_r_span{
                        cursor: pointer;
                        color:#43425d;
                        font-weight: 600;
                    }
                }
            }
            .userBio {
                line-height: 1.64;
                font-weight: normal; // html h4 
                @media (max-width: @screen-xs) {
                    order: 1;
                }
                .readMore {
                    color: #43425d;
                    font-weight: 600;
                    cursor: pointer;
                }
                .course {
                    font-weight: 600;
                    font-size: 14px;

                    .courseName {
                        font-weight: normal;
                    }
                    // .responsive-property(font-size, 16px, null, 14px);
                }
            }
        }
        .calendarBtnWrap {
            display: flex;
            flex-direction: column;
        }
        .profileUserSticky_btns{
            &.why_learn_user_btn{
                margin-top: 34px !important;
            }
            &.student {
                margin: 0 0 0 auto;
                cursor: pointer;
            }
            .pUb_edit_user{
                cursor: pointer;
            }
            .profileUserSticky_btn{
                margin: 0;
                width: 100%;
                border-radius: 26px;
                .v-btn__content{
                    // justify-content: flex-start;
                    justify-content: start;
                    // text-align: initial;
                }
                &.isMyProfile{
                    // visibility: hidden;
                    color: white !important;
                    border: none !important;
                    svg{
                    path{
                        fill: white;
                    }
                    }
                }
                .profileUserSticky_btn_icon{
                    line-height: 0;
                }
                .profileUserSticky_btn_txt{
                    font-size: 14px;
                    font-weight: 600;
                    text-transform: initial;
                    flex-grow: 1;
                }
                &.profileUserSticky_btn_book{
                    color: #4c59ff !important;
                    border: solid 1.5px #4c59ff !important;
                    &.hideCalendarBtn{
                        visibility: hidden;
                        color: white !important;
                        border: none !important;
                    svg{
                        path{
                            fill: #4c59ff !important;
                        }
                    }
                    }
                }
                &.profileUserSticky_btn_find{
                    .v-btn__content{
                        justify-content: center;
                    }
                }
            }
        }
        .profileUserSticky_pricing{
            .profileUserSticky_pricing_price{
                .profileUserSticky_pricing_price_hour{
                    font-size: 16px;
                    font-weight: 600;
                }
                .profileUserSticky_pricing_price_currency{
                    font-size: 18px;
                    font-weight: bold;
                }
                .profileUserSticky_pricing_price_number{
                    font-size: 26px;
                    font-weight: bold;
                }
            }
            .profileUserSticky_pricing_discount{
                font-size: 20px;
                color: #b2b5c9;
                text-decoration: line-through;
            }
        }
        .profileUserSticky_coupon{
            outline: none;
            font-weight: 600;
            font-size: 12px;
            color: @global-purple;
            &.isMyProfileCoupon{
                color: #c5c8cf;
                cursor: initial;
            }
        }
        // move to followBtn.vue style insted if we dont need anymore for old version of profile
        .followBtnNew {
            outline: none;
            font-weight: 600;
            display: flex;
            align-items: center;
            line-height: normal;
        }
    }

    .pUb_edit_user_top{
        position: absolute;
        right: 0;
    }
    .pUb_top_defaultState{

        .pUb_edit_user{
            cursor: pointer;
            position: absolute;
            top: 0;
            right: 0;
        }
    }

    .pUb_middle_AboutMe{
        font-size: 18px;
        font-weight: 600;
        font-stretch: normal;
        font-style: normal;
        letter-spacing: normal;
            word-break: break-word;
        @media (max-width: @screen-xs) {
            font-size: 16px;
            line-height: 1.4;
        }
        padding-bottom: 12px;
    }
    .bottom {
        border-top: 1px solid #ddd;
        @media (max-width: @screen-xs) {
            border-top: none;
        }
        .bottomBox {
            color: @global-purple;
            font-size: 28px;
            .icon {
                align-self: baseline;
                margin-top: 8px;
            }
            .number {
                font-weight: 600;
            }
            .type {
                font-size: 14px;
            }
        }
    }
}
</style>