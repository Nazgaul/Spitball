<template>
    <div class="profileUserBox" v-if="currentProfileUser">
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
                    <h2 class="profileUserBox_top_mobile_userUniversity text-truncate" v-text="currentProfileUser.universityName"/>
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
            
            <v-flex v-bind="flexOrder" class="pUb_top_defaultState">
                <editSVG sel="edit" class="pUb_edit_user mr-1" v-if="!isMobile && isCurrentProfileUser" @click="openEditInfo()"/>
                <div class="pUb_defaultState_img">
                    <div class="pUb_dot" v-if="isOnline"/>
                    <uploadImage sel="photo" class="pUb_edit_img" v-if="isCurrentProfileUser"/>
                    <userAvatarRect class="pUb_dS_img" 
                                    :userName="currentProfileUser.name" 
                                    :userImageUrl="currentProfileUser.image" 
                                    :width="isMobile? 106 :116" 
                                    :height="isMobile? 126 : 138"
                                    :userId="currentProfileUser.id"
                                    :fontSize="36"
                                    :borderRadius="8"/>
                </div>
                
                <div class="pUb_defaultState_content text-truncate hidden-xs-only">
                    <div>
                        <h1 class="pUb_dS_c_userName text-truncate">
                        <span v-if="currentProfileUser.isTutor" class="mr-1" v-language:inner="'profile_tutor'"/> 
                        {{currentProfileUser.name}}
                        </h1>
                        <h2 class="pUb_dS_c_userUniversity text-truncate" v-text="currentProfileUser.universityName"/>
                        <template v-if="currentProfileUser.isTutor">
                            <div class="pUb_dS_c_rating" v-if="currentProfileTutor.reviewCount">
                                <userRating class="c_rating" :showRateNumber="false" :rating="currentProfileTutor.rate" :size="'18'" />
                                <span @click="scrollToReviews" class="pUb_dS_c_r_span ml-1" v-text="$Ph(currentProfileTutor.reviewCount === 1 ? 'resultTutor_review_one' : `resultTutor_reviews_many`, reviewsPlaceHolder(currentProfileTutor.reviewCount))"/>
                            </div>
                            <div v-else class="pUb_dS_c_rating">
                                <starEmptySVG class="pUb_dS_c_rating_star"/>
                                <span class="no-reviews font-weight-bold caption" v-language:inner="'resultTutor_no_reviews'"></span>
                            </div>
                        </template>
                    </div>
                    <div class="profileUserBox_defaultState_content_followers">
                        <followBtn class="mr-2" v-if="!isCurrentProfileUser"/>
                        <span v-if="currentProfileUser.followers" class="defaultState_content_followers" 
                        v-text="$Ph(dynamicDictionay(currentProfileUser.followers,'profile_tutor_followers','profile_tutor_follower'),[currentProfileUser.followers])"/>
                    </div>
                </div>
            </v-flex>
            <v-flex xs4 class="pUb_top_tutorState" v-if="currentProfileUser.isTutor">
                <div class="pUb_top_tS_list">
                    <starSVG/>
                    <span class="pUb_t_ts_list_span pUb_t_ts_list_span_review" @click="isMobile? scrollToReviews():''" v-text="tutorStateRate(currentProfileTutor)"/>
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
            </v-flex>
        </div>
        <v-flex v-if="currentProfileUser.description" sm9 xs12 class="profileUserBox_middle">
            <h3 class="pUb_middle_AboutMe" v-text="currentProfileUser.description"/>
            <template v-if="currentProfileUser.isTutor">
                <h4 v-if="currentProfileTutor.bio" class="pUb_middle_bio">{{currentProfileTutor.bio | truncate(isOpen, '...', textLimit)}}<span class="d-none">{{currentProfileTutor.bio | restOfText(isOpen, '...', textLimit)}}</span><span sel="bio_more" v-if="readMoreVisible" @click="isOpen = !isOpen" class="pUb_middle_bio_readMore" v-language:inner="isOpen?'profile_read_less':'profile_read_more'"/></h4>
            </template>
        </v-flex>
        <div class="profileUserBox_bottom" v-if="currentProfileUser.isTutor && currentProfileTutor.subjects.length">
            <span class="profileUserBox_bottom_title mr-1" v-language:inner="'profile_study'"/>
            <span v-for="(subject, index) in currentProfileTutor.subjects" :key="index">{{subject}}{{index + 1 == currentProfileTutor.subjects.length? '':' ,'}}</span>
        </div>
    </div>
</template>

<script>
import starSVG from './images/tStar.svg';
import starEmptySVG from './images/stars-copy.svg';

import clockSVG from './images/tClock.svg';
import studensSVG from './images/tStudents.svg';
import resxSVG from './images/tResx.svg';
import editSVG from './images/edit.svg';
import { mapGetters, mapActions } from 'vuex';
import userRating from '../../profileHelpers/profileBio/bioParts/userRating.vue'
import { LanguageService } from "../../../../services/language/languageService";
import userAvatarRect from '../../../helpers/UserAvatar/UserAvatarRect.vue';
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
    },
    data() {
        return {
            defOpen:false,
        }
    },
    computed: {
        ...mapGetters(['getProfile','accountUser','getUserStatus']),
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
            return this.isMobile? 76 : 140;
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
        flexOrder(){
            if(this.currentProfileUser.isTutor){
                if(this.isMobile){
                    return {xs4:true}
                }else{
                    return {xs9: true}
                }
            }else{
                return {xs12:true}
            }
        }
    },
    methods: {
        ...mapActions(['updateEditDialog']),
        reviewsPlaceHolder(reviews) {
            return reviews === 0 ? reviews.toString() : reviews;
        },
        tutorStateRate(tutorData){
            let rate = tutorData.rate.toFixed(1);
            let reviews = tutorData.reviewCount;
            if(reviews < 1){
                return LanguageService.getValueByKey('resultTutor_collecting_review');
            }
            let dictionary = reviews > 1? LanguageService.getValueByKey('profile_reviews'): LanguageService.getValueByKey('profile_single_review')
            return `${rate} (${reviews} ${dictionary.toLowerCase()})`
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
    width: 100%;
    height: auto;
    border-radius: 8px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    background-color: #ffffff;
    margin-bottom: 24px;
    padding: 16px;
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
            .profileUserBox_top_mobile_userUniversity{
                font-size: 14px;
                font-weight: normal;
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
        @media (max-width: @screen-xs) {
            // justify-content: center;
            justify-content: flex-start;
            height: 126px;
            padding-left: 14px;
            position: relative;
            margin-bottom: 16px;
        }
        margin-bottom: 22px;
        .pUb_edit_user_top{
            position: absolute;
            right: 0;
        }


        height: 138px;
        .pUb_top_defaultState{
            display: flex;
            position: relative;
            @media (max-width: @screen-xs) {
                flex-basis: 0;
            }
            @media (max-width: @screen-xss) {
                margin-right: 24px;
            }

            .pUb_edit_user{
                position: absolute;
                top: 0;
                right: 0;
                cursor: pointer;
            }
            .pUb_defaultState_img{
                margin-right: 16px;
                position: relative;
                .pUb_dot{
                    position: absolute;
                    left: 8px;
                    top: 8px;
                    width: 12px;
                    height: 12px;
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
            .pUb_defaultState_content{
                color: #43425d;
                font-stretch: normal;
                font-style: normal;
                letter-spacing: normal;
                line-height: normal;
                display: flex;
                flex-direction: column;
                justify-content: space-between;
                .profileUserBox_defaultState_content_followers{
                    display: flex;
                    align-items: center;
                    height: 26px;
                    .defaultState_content_followers{
                        font-size: 14px;
                        font-weight: 600;
                    }
                }

                .pUb_dS_c_userName{
                    font-size: 18px;
                    font-weight: bold;
                    display: flex;
                    flex-wrap: wrap;
                    line-height: 1;
                    padding-bottom: 6px;
                }
                .pUb_dS_c_userUniversity{
                    font-size: 14px;
                    font-weight: normal;
                    line-height: 1.64;
                    padding-bottom: 6px;
                }
                .pUb_dS_c_rating{
                    display: inline-flex;
                    align-items: center;

                    i{
                        font-size: 18px !important;
                    }
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
                        font-size: 12px;
                        color:#4c59ff;
                        font-weight: 600;
                    }
                }
            }
        }
        .pUb_top_tutorState{
            @media (max-width: @screen-xs) {
                border-left: none;
                padding: 6px 0;
                justify-content: space-around;
                margin-bottom: 0;

            }

            .flexSameSize();
            border-left: solid 1px #dddddd;
            padding-left: 16px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            max-width: 162px;
            min-width: 162px;
            margin-bottom: 34px;
            .pUb_top_tS_list{
                padding-top: 3px;
                .pUb_t_ts_list_span{
                    vertical-align: text-top;
                    margin-left: 6px;
                    font-size: 12px;
                    color: #43425d;
                    &.pUb_t_ts_list_span_review{
                        @media (max-width: @screen-xs) {
                            cursor: pointer;
                            color:#4c59ff;
                            font-weight: 600;
                        }
                    }
                }
            }
        }
        
    }
    .profileUserBox_middle{
        @media (max-width: @screen-xs) {
            padding: 0 14px;
            padding-right: 10px;
        }

        color: #43425d;
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
        .pUb_middle_bio{
            margin: 0;
            padding: 0;
            // padding-bottom: 8px;
            padding-bottom: 12px;
            @media (max-width: @screen-xs) {
               padding-bottom: 14px;
            }
            font-size: 14px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: 1.57;
            letter-spacing: normal;
                word-break: break-word;
            .pUb_middle_bio_readMore{
                font-weight: 600;
                cursor: pointer;
            }
        }
    }
    .profileUserBox_bottom{
        // margin-top: 14px;
        color: #43425d;
        font-size: 14px;
        .profileUserBox_bottom_title{
            font-weight: bold;
        }
        @media (max-width: @screen-xs) {
            padding: 0 14px 12px;
            // margin-top: 10px;
        }
    } 
}
</style>