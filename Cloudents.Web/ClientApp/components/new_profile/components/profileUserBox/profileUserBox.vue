<template>
    <div class="profileUserBox pa-5" v-if="currentProfileUser">
        <div class="profileUserBox_top_mobile" v-if="isMobile">
            <div class="profileUserBox_top_mobile_top">
                <a class="profileUserBox_top_mobile_link" @click="$router.go(-1)">
                    <v-icon v-text="'sbf-arrow-left-carousel'"/>
                </a>
            </div>
            <div class="profileUserBox_top_mobile_bottom">
                <div class="profileUserBox_top_mobile_right">
                    <h1 class="profileUserBox_top_mobile_userName text-truncate">
                        <span v-if="currentProfileUser.isTutor" v-language:inner="'profile_tutor'"/>
                        {{currentProfileUser.name}}
                    </h1>
                </div>
                <div class="profileUserBox_top_mobile_left">
                    <followBtn v-if="!isCurrentProfileUser"/>
                    <editSVG sel="edit" class="profileUserBox_top_mobile_left_edit" v-if="isMobile && isCurrentProfileUser" @click="openEditInfo()"/>
                    <div class="profileUserBox_top_mobile_left_followers">
                        <span v-if="currentProfileUser.followers" class="defaultState_content_followers" 
                        v-text="$Ph(currentProfileUser.followers > 1 ? 'profile_tutor_followers':'profile_tutor_follower',currentProfileUser.followers)"/>
                    </div>
                </div>
            </div>
        </div>

        <div class="profileUserBox_top">
            
            <div class="leftSide pUb_defaultState_img mr-6">
                <div class="pUb_dot" v-if="isOnline"></div>
                <uploadImage sel="photo" class="pUb_edit_img" v-if="isCurrentProfileUser" />
                <userAvatarRect
                    class="pUb_dS_img"
                    :userName="currentProfileUser.name"
                    :userImageUrl="currentProfileUser.image"
                    :width="isMobile? 130: 226"
                    :height="isMobile? 161 : 278"
                    :userId="currentProfileUser.id"
                    :fontSize="36"
                    :borderRadius="8"
                />
            </div>
                
            <div class="rightSide flex-grow-1">
                <div>
                    <div class="d-flex justify-space-between align-center">
                        <h1 class="userName text-truncate">
                        <span v-if="currentProfileUser.isTutor" class="mr-1" v-t="'profile_tutor'"></span>
                        <span>{{currentProfileUser.name}}</span>
                    </h1>
                    <editSVG sel="edit" class="pUb_edit_user mr-1" v-if="!isMobile && isCurrentProfileUser" @click="openEditInfo"/>
                </div>

                    <!-- courses teacher -->
                    <div class="course mt-2 text-truncate" v-if="currentProfileUser.isTutor && currentProfileUser.courses.length">
                        <span class="iTeach mr-1" v-t="'profile_my_courses'"></span>
                        <span class="courseName text-truncate">{{currentProfileUser.courses.toString().replace(/,/g, ", ")}}</span>
                    </div>

                    <!-- Rate And Follower -->
                    <div class="d-flex mt-2 mb-5">
                        <template v-if="currentProfileUser.isTutor">
                            <div class="pUb_dS_c_rating" v-if="currentProfileTutor.reviewCount">
                                <userRating class="c_rating" :showRateNumber="false" :rating="currentProfileTutor.rate" :size="'18'" />
                                <span @click="scrollToReviews" class="pUb_dS_c_r_span ml-1">{{$tc('resultTutor_review_one',currentProfileTutor.reviewCount)}}</span>
                            </div>
                            <div v-else class="pUb_dS_c_rating">
                                <starEmptySVG class="pUb_dS_c_rating_star"/>
                                <span class="no-reviews font-weight-bold caption" v-language:inner="'resultTutor_no_reviews'"></span>
                            </div>
                        </template>
                        <div class="ml-5">
                            <followBtn class="followBtnNew mr-2" v-if="!isCurrentProfileUser"/>
                            <!-- <span v-if="currentProfileUser.followers" class="defaultState_content_followers" 
                            v-text="$Ph(dynamicDictionay(currentProfileUser.followers,'profile_tutor_followers','profile_tutor_follower'),[currentProfileUser.followers])"/> -->
                        </div>
                    </div>

                    <!-- TUTOR BIO -->
                    <h4 v-if="currentProfileTutor.bio" class="pUb_middle_bio">{{currentProfileTutor.bio | truncate(isOpen, '...', textLimit)}}
                        <span class="d-none">{{currentProfileTutor.bio | restOfText(isOpen, '...', textLimit)}}</span>
                        <span sel="bio_more" v-if="readMoreVisible" @click="isOpen = !isOpen" class="pUb_middle_bio_readMore" v-language:inner="isOpen?'profile_read_less':'profile_read_more'"></span>
                    </h4>

                    <!-- courses student -->
                    <div class="course mt-2 text-truncate" v-if="!currentProfileUser.isTutor && currentProfileUser.courses.length">
                        <span class="profileUserBox_bottom_title mr-1">{{$t('profile_my_courses')}}:</span>
                        <span v-for="(course, index) in currentProfileUser.courses" :key="index">
                            {{course}}{{index + 1 == currentProfileUser.courses.length ? '' : ', '}}
                        </span>
                    </div>
                </div>
                <!-- <v-btn :to="{name: routeNames.EditCourse}" v-ripple="false" icon text v-if="isLogged && !currentProfileUser.isTutor">
                    <editSVG class="mr-1" v-if="isCurrentProfileUser" />
                </v-btn> -->

                <div class="profileUserSticky_btns d-flex justify-space-between align-end" v-if="currentProfileUser.isTutor">
                    <v-btn sel="send" height="42" width="246" :disabled="false" class="profileUserSticky_btn white--text" :class="{'isMyProfile':currentProfileTutor}" depressed rounded color="#4c59ff" @click="''">
                        <chatSVG class="profileUserSticky_btn_icon"/>
                        <div class="profileUserSticky_btn_txt" v-language:inner="'profile_send_message'"/>
                    </v-btn>
                    <div :class="{'flex-grow-1 ml-3': isCurrentProfileUser || !getProfile.user.calendarShared}">
                        <div class="d-flex justify-space-between align-end">
                            <button sel="coupon" :class="{'isMyProfileCoupon':!currentProfileTutor}" class="profileUserSticky_coupon" @click="''" v-t="'coupon_apply_coupon'"/>
                            <div class="profileUserSticky_pricing d-flex align-end">
                                <v-flex class="profileUserSticky_pricing_discount mr-2" v-if="isDiscount">
                                    {{tutorPrice ? $n(tutorPrice, 'currency') : $n(tutorDiscountPrice, 'currency')}}
                                </v-flex>
                                <v-flex class="profileUserSticky_pricing_price">
                                    <span class="profileUserSticky_pricing_price_number">{{isDiscount && tutorPrice !== 0  ? $n(tutorDiscountPrice, 'currency') : $n(tutorPrice, 'currency')}}</span>/<span class="profileUserSticky_pricing_price_hour" v-language:inner="'profile_points_hour'"/>
                                </v-flex>
                            </div>
                        </div>
                        <v-btn sel="calendar" height="42" width="246" :disabled="false" v-if="!isCurrentProfileUser && getProfile.user.calendarShared" @click="''" :class="{'isMyProfile':currentProfileTutor}" class="profileUserSticky_btn profileUserSticky_btn_book white--text mt-2" depressed rounded color="white">
                            <calendarSVG width="20" class="profileUserSticky_btn_icon"/>
                            <div class="profileUserSticky_btn_txt" v-language:inner="'profile_book_session'"/>
                        </v-btn>
                    </div>
                </div>
            </div>
        </div>

        <v-row class="bottom text-center pt-3" dense v-if="currentProfileTutor">
            <v-col class="bottomBox d-flex align-center justify-center">
                <starSVG class="mt-2" width="26" />
                <div class="ml-2" @click="isMobile ? scrollToReviews():''" >
                    <div class="number text-left">{{tutorStateRate(currentProfileTutor)}}</div>
                    <div class="type">dsadasdsdas</div>
                    <!-- <span class="pUb_t_ts_list_span pUb_t_ts_list_span_review" @click="isMobile? scrollToReviews():''" v-text="tutorStateRate(currentProfileTutor)"></span> -->
                </div>
            </v-col>
            <v-col class="bottomBox d-flex align-center justify-center">
                <resxSVG class="mt-2" width="26" />
                <div class="ml-2">
                    <div class="number text-left">324</div>
                    <div class="type">dsadasdsdas</div>
                    <!-- <span class="pUb_t_ts_list_span" v-text="$Ph(dynamicDictionay(currentProfileTutor.contentCount,'profile_resourses','profile_resourse'),currentProfileTutor.contentCount)"/> -->
                </div>
            </v-col>
            <v-col class="bottomBox d-flex align-center justify-center">
                <clockSVG class="mt-2" width="26" />
                <div class="ml-2">
                    <div class="number text-left">532</div>
                    <div class="type">dsadasdsdas</div>
                    <!-- <span class="pUb_t_ts_list_span" v-text="$Ph(dynamicDictionay(currentProfileTutor.lessons,'profile_sessions','profile_session'),currentProfileTutor.lessons)"/> -->
                </div>
            </v-col>
            <v-col class="bottomBox d-flex align-center justify-center">
                <studensSVG class="mt-2" width="26" />
                <div class="ml-2">
                    <div class="number text-left">243</div>
                    <div class="type">dsadasdsdas</div>
                    <!-- <span class="pUb_t_ts_list_span ml-1" v-text="$Ph(dynamicDictionay(currentProfileTutor.students,'profile_students','profile_student'),currentProfileTutor.students)"/> -->
                </div>
            </v-col>
        </v-row>

        <!-- <v-flex xs4 class="pUb_top_tutorState" v-if="currentProfileUser.isTutor">
            <div class="pUb_top_tS_list">
                <starSVG/>
                <span class="pUb_t_ts_list_span pUb_t_ts_list_span_review" @click="isMobile? scrollToReviews():''" 
                v-text="tutorStateRate(currentProfileTutor)"/>
            </div>
            <div class="pUb_top_tS_list" :class="[{'visibility_hidden':!currentProfileTutor.contentCount}]">
                <resxSVG/>
                <span class="pUb_t_ts_list_span" v-text="$Ph(dynamicDictionay(currentProfileTutor.contentCount,'profile_resourses','profile_resourse'),currentProfileTutor.contentCount)"/>
            </div>
            <div class="pUb_top_tS_list" :class="[{'visibility_hidden':!currentProfileTutor.lessons}]">
                <clockSVG/>
                <span class="pUb_t_ts_list_span" v-text="$Ph(dynamicDictionay(currentProfileTutor.lessons,'profile_sessions','profile_session'),currentProfileTutor.lessons)"/>
            </div>
            <div class="pUb_top_tS_list" :class="[{'visibility_hidden':!currentProfileTutor.students}]">
                <studensSVG/>
                <span class="pUb_t_ts_list_span ml-1" v-text="$Ph(dynamicDictionay(currentProfileTutor.students,'profile_students','profile_student'),currentProfileTutor.students)"/>
            </div>
        </v-flex> -->
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';

import * as routeNames from '../../../../routes/routeNames';

import starSVG from './images/tStar.svg';
import starEmptySVG from './images/stars-copy.svg';
import clockSVG from './images/tClock.svg';
import studensSVG from './images/tStudents.svg';
import resxSVG from './images/tResx.svg';
import editSVG from './images/edit.svg';
import chatSVG from '../profileUserSticky/images/chatIcon_mobile.svg';
import calendarSVG from '../profileUserSticky/images/calendarIcon.svg';

import userAvatarRect from '../../../helpers/UserAvatar/UserAvatarRect.vue';
import userRating from '../../profileHelpers/profileBio/bioParts/userRating.vue'
import uploadImage from '../../profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue';
import followBtn from '../followBtn/followBtn.vue';

export default {
    name:'profileUserBox',
    components:{
        starSVG,
        clockSVG,
        studensSVG,
        resxSVG,
        userRating,
        userAvatarRect,
        uploadImage,
        editSVG,
        followBtn,
        starEmptySVG,
        chatSVG,
        calendarSVG
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
            return this.isMobile ? 76 : 220;
        },
        isOpen :{
            get(){
                return this.defOpen
            },
            set(val){
                this.defOpen = val
            }
        },
        readMoreVisible(){
            if(!!this.currentProfileTutor){
                return this.currentProfileTutor.bio.length >=  this.textLimit
            }else{
                return false;
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
    },
    methods: {
        ...mapActions(['updateEditDialog']),
        // reviewsPlaceHolder(reviews) {
        //     return reviews === 0 ? reviews.toString() : reviews;
        // },
        tutorStateRate(tutorData){
            let rate = tutorData.rate.toFixed(0);
            let reviews = tutorData.reviewCount;
            if(reviews < 1){
                return this.$t('resultTutor_collecting_review');
            }
            // let dictionary = reviews > 1? this.$t('profile_reviews'): this.$t('profile_single_review')
            return `${rate}`
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
        },
        dynamicDictionay(number,multipleDictionay,singleDictionay){
            return number > 1 ? multipleDictionay : singleDictionay;
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
.profileUserBox{
    max-width: 800px;
    width: 100%;
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
        padding-top: 14px;
        margin-bottom: 8px;

    }
    .visibility_hidden{
        visibility: hidden;
    }
    .profileUserBox_top_mobile{
        display: flex;
        padding: 0 14px;
        padding-bottom: 8px;
        padding-right: 12px;
        flex-direction: column;
        color: #43425d;
        .profileUserBox_top_mobile_top{
            display: flex;
            justify-content: space-between;
            width: 100%;
            padding-bottom: 20px;
            .profileUserBox_top_mobile_link{
                transform: none /*rtl:rotate(180deg)*/ ;
                cursor: pointer;
                i{
                    font-size: 20px;
                    color: #69687d;
                }
            }
        }
        .profileUserBox_top_mobile_bottom{
            display: flex;
            justify-content: space-between;
        }

        .profileUserBox_top_mobile_right{
            .profileUserBox_top_mobile_userName{
                font-size: 18px;
                font-weight: bold;
                letter-spacing: normal;
                line-height: 1.4;
            }
        }
        .profileUserBox_top_mobile_left{
            text-align: end;
            .profileUserBox_top_mobile_left_edit{
                vertical-align: bottom;
            }
            .profileUserBox_top_mobile_left_followers{
                font-weight: 600;
                line-height: 2;
            }
        }
    }
    .profileUserBox_top{
        display: flex;
        justify-content: space-between;
        margin-bottom: 34px;
        @media (max-width: @screen-xs) {
            // justify-content: center;
            justify-content: flex-start;
            height: 126px;
            padding-left: 14px;
            position: relative;
            margin-bottom: 16px;
        }
    }

        .leftSide {
            position: relative;
            .pUb_dot {
                position: absolute;
                left: 8px;
                top: 8px;
                width: 16px;
                height: 16px;
                border-radius: 50%;
                background-color: #6aff70;
            }
            .pUb_dS_img{
                pointer-events: none !important;
            }
            .pUb_edit_img{
                position: absolute;
                right: 0;
                text-align: center;
                width: 36px;
                height: 46px;
                border-radius: 4px;
                background-color: rgba(255, 255, 255, 0.38);
            }
        }

    .rightSide {
        color: @global-purple;
        min-width: 0;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        // align-items: center;
        // height: 26px;
        .defaultState_content_followers{
            font-size: 14px;
            font-weight: 600;
        }
        .userName{
            .responsive-property(font-size, 24px, null, 22px);
            font-weight: 600;
            // display: flex;
            // flex-wrap: wrap;
            // line-height: 1;
            // padding-bottom: 6px;
        }
        .course {
            font-weight: 600;
            .responsive-property(font-size, 16px, null, 14px);
            .courseName {
            }
        }
        .pUb_dS_c_rating{
            display: inline-flex;
            align-items: center;

            // i{
            //     font-size: 18px !important;
            // }
            .no-reviews {
                margin-left: 5px;
                color: #43425d;
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
                color:#4c59ff;
                font-weight: 600;
            }
        }
        .pUb_middle_bio{
            line-height: 1.64;
            font-weight: normal; // html h4 
        }

        .profileUserSticky_btns{
            &.why_learn_user_btn{
                margin-top: 34px !important;
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
                    font-size: 16px;
                    font-weight: 600;
                    text-transform: initial;
                    flex-grow: 1;
                }
                &.profileUserSticky_btn_book{
                    color: #4c59ff !important;
                    border: solid 1.5px #4c59ff !important;
                    &.isMyProfile{
                    // color: white !important;
                    // border: none !important;
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
            .profileUserSticky_coupon{
                outline: none;
                font-weight: 600;
                color: #4c59ff;
                &.isMyProfileCoupon{
                    color: #c5c8cf;
                    cursor: initial;
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
                        font-size: 28px;
                        font-weight: bold;
                    }
                }
                .profileUserSticky_pricing_discount{
                    font-size: 20px;
                    color: #b2b5c9;
                    text-decoration: line-through;
                    padding-left: 6px;
                }
            }
        }

        // move to followBtn.vue style insted if we dont need anymore for old version of profile
        .followBtnNew {
                padding: 0;
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
            // display: flex;
            // position: relative;
            // @media (max-width: @screen-xs) {
            //     flex-basis: 0;
            // }
            // @media (max-width: @screen-xss) {
            //     margin-right: 24px;
            // }

            .pUb_edit_user{
                position: absolute;
                top: 0;
                right: 0;
                cursor: pointer;
            }
            // .pUb_defaultState_img{
      
 
            // }
            .pUb_defaultState_content{
                // color: #43425d;
                // font-stretch: normal;
                // font-style: normal;
                // letter-spacing: normal;
                // line-height: normal;
                // display: flex;
                // flex-direction: column;
                // justify-content: space-between;
                .profileUserBox_defaultState_content_followers{
                //     display: flex;
                //     align-items: center;
                //     height: 26px;
                //     .defaultState_content_followers{
                //         font-size: 14px;
                //         font-weight: 600;
                //     }
                // }

                // .pUb_dS_c_userName{
                //     font-size: 18px;
                //     font-weight: bold;
                //     display: flex;
                //     flex-wrap: wrap;
                //     line-height: 1;
                //     padding-bottom: 6px;
                // }
                // .pUb_dS_c_rating{
                //     display: inline-flex;
                //     align-items: center;

                //     i{
                //         font-size: 18px !important;
                //     }
                //     .no-reviews {
                //         margin-left: 5px;
                //         color: #43425d;
                //         font-size: 12px !important;
                //         margin-top: 2px;
                //     }
                //     .pUb_dS_c_rating_star {
                //         width: 18px;
                //         vertical-align: sub !important;
                //     }
                //     .c_rating{
                //         flex: 0 0 auto;
                //         &.rating-container{
                //         .v-rating{
                //            .v-icon{
                //               padding-right: 1px;
                //            }
                //         }
                //      }
                //     }
                //     .pUb_dS_c_r_span{
                //         cursor: pointer;
                //         font-size: 12px;
                //         color:#4c59ff;
                //         font-weight: 600;
                //     }
                // }
            }
        }
        // .pUb_top_tutorState{
        //     @media (max-width: @screen-xs) {
        //         border-left: none;
        //         padding: 6px 0;
        //         justify-content: space-around;
        //         margin-bottom: 0;

        //     }

        //     .flexSameSize();
        //     border-left: solid 1px #dddddd;
        //     padding-left: 16px;
        //     display: flex;
        //     flex-direction: column;
        //     justify-content: space-between;
        //     max-width: 162px;
        //     min-width: 162px;
        //     margin-bottom: 34px;
        //     .pUb_top_tS_list{
        //         padding-top: 3px;
        //         .pUb_t_ts_list_span{
        //             vertical-align: text-top;
        //             margin-left: 6px;
        //             font-size: 12px;
        //             color: #43425d;
        //             &.pUb_t_ts_list_span_review{
        //                 @media (max-width: @screen-xs) {
        //                     cursor: pointer;
        //                     color:#4c59ff;
        //                     font-weight: 600;
        //                 }
        //             }
        //         }
        //     }
        // }
        
    }
    // .profileUserBox_middle{
        // @media (max-width: @screen-xs) {
        //     padding: 0 14px;
        //     padding-right: 10px;
        // }

        // color: #43425d;
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

    // }
    .profileUserBox_bottom{
        // margin-top: 14px;
        // color: #43425d;
        // font-size: 16px;
        // .profileUserBox_bottom_title{
        //     font-weight: bold;
        // }
        // &.course {
        //   .giveMeEllipsis(2, 18);
        // }
        // @media (max-width: @screen-xs) {
            // padding: 0 14px 12px;
            // margin-top: 10px;
        // }
    }
    .bottom {
        // margin-top: 10px;
        // padding-top: 10px;
        border-top: 1px solid #ddd;

        .bottomBox {
            color: @global-purple;
            font-size: 32px;
            svg {
                align-self: baseline;
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