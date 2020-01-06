<template>
    <div class="leave_review_wrap">
        <div class="d-flex">
            <v-icon class="body-2 review_close_icon d-flex d-sm-none" @click="closeReviewDialog">sbf-close</v-icon>
            <div class="review_title text-center font-weight-bold mx-auto" v-language:inner="reviewsTitle"></div>
        </div>
        <div class="review_sub_title text-center mt-1 mb-6">
            <span v-if="!showNextStep" v-html="$Ph('leaveReview_sub_title_step1', nameTutor)"></span>
            <span class="review_sub_title_step2" v-else v-language:inner="'leaveReview_sub_title_step2'"></span>
        </div>
        <div class="review_user_rate mb-5" v-if="!showNextStep">
            <user-avatar class="tutor-img-wrap mr-2" size="42" :userImageUrl="tutorImg" :user-name="nameTutor" :user-id="tutorId"/>
            <userRating 
                class="review_stars"
                :rating="rating"
                :rateNumColor="'#43425D'"
                :size="'28'"
                :readonly="false"
                :showRateNumber="false"
                :callbackFn="setRateStar"
                :rate-num-color="'#43425D'">
            </userRating>
            <span class="review_start_rate">{{ratingRate}}</span>
        </div>
        <div class="review_textarea">
            <div class="mb-1 review_error" v-show="reviewsError && btnDisabled" v-language:inner="'leaveReview_error'"></div>
            <v-textarea
                rows="4"
                outlined
                autofocus
                v-model="reviewText"
                name="input-review"
                hide-details
                auto-grow
                :placeholder="reviewPlaceholder"
            ></v-textarea>
        </div>
        <div class="text-center mt-5">
            <v-btn @click="closeReviewDialog" class="review_btn review_btn-back" depressed color="white" rounded>
                <span v-language:inner="btnText"/>
            </v-btn>
            <v-btn :loading="btnLoading" @click="goNextStep" class="review_btn review_btn-next white--text" depressed rounded color="#4452fc" sel="submit_tutor_request">
                <span v-language:inner="'leaveReview_send'"/>
            </v-btn>
        </div>
    </div>
</template>

<script>
    import { mapActions, mapGetters, mapState } from 'vuex';

    import { LanguageService } from "../../../../services/language/languageService";
    import utilitiesService from "../../../../services/utilities/utilitiesService";

    import userRating from '../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';

    export default {
        components: {userRating, userAvatar},
        name: "leaveReview",
        data() {
            return {
                reviewText: '',
                reviewsError: false,
                reviewInputHidden: false,
                reviewSent: false,
                btnLoading: false,
                showNextStep: false,
                roomId: 0,
                rating: 0,
                reviewPlaceholder: LanguageService.getValueByKey("leaveReview_review_placeholder"),
            };
        },
        computed: {
            ...mapState(['tutoringMain']),
            ...mapGetters(['getReview', 'getStudyRoomData']),
            tutorImg() {
                if(this.getStudyRoomData && this.getStudyRoomData.tutorImage){
                    return utilitiesService.proccessImageURL(this.getStudyRoomData.tutorImage, 54, 54);
                }
                return '';
            },
            tutorId() {
                return this.getStudyRoomData ?  this.getStudyRoomData.tutorId : '';
            },
            tutorName() {
                return this.getStudyRoomData ?  this.getStudyRoomData.tutorName : '';
            },
            nameTutor() {
                return this.getStudyRoomData.tutorName;
            },
            btnDisabled() {
                return this.rating === 0;
            },
            ratingRate() {
                return this.rating > 2.5 ? 'strong' : 'weak';
            },
            reviewsTitle() {
                return this.showNextStep ? 'leaveReview_title_step2' : 'leaveReview_title_step1';
            },
            // reviewsSubTitle() {
            //     if(this.showNextStep) {
            //         return LanguageService.getValueByKey('leaveReview_sub_title_step2');
            //     } else {
            //         return this.$Ph('leaveReview_sub_title_step1', this.nameTutor);
            //     } 
            // },
            btnText() {
                return this.showNextStep ? 'leaveReview_noThanks' : 'leaveReview_back';
            }
        },
        methods: {
            ...mapActions(['submitReview', 'updateReviewDialog', 'updateReviewStars', 'updateReview', 'updateStudentStartDialog', 'setStudentDialogState']),
            toggleReviewInput() {
                return this.reviewInputHidden = !this.reviewInputHidden;
            },
            setRateStar(val) {
                this.rating = val;
                if(val > 0) {
                    this.updateReviewStars(val);
                }
            },
            goNextStep() {
                if(this.reviewText.length > 0 && this.rating > 0) {
                    this.sendReview();
                } else if(this.reviewText.length <= 0 && this.rating > 0) {
                    this.showNextStep = true;
                } else {
                    this.reviewsError = true;
                }
            },
            sendReview() {
                this.btnLoading = true;
                let stars = this.getReview.rate;
                this.submitReview({
                    roomId: this.getStudyRoomData.roomId,
                    review: this.reviewText,
                    rate: stars,
                    tutor: this.tutorId
                })
                .then((resp) => {
                    if(!!resp) {
                        this.reviewSent = true;
                        this.btnLoading = false;
                        this.updateReview(null);
                        this.closeReviewDialog();
                    }
                },
                (error) => {
                    console.log('error sending review', error);
                    this.btnLoading = false;
                }
                ).finally(() => {
                    this.btnLoading = false;
                });
            },
            closeReviewDialog() {
                this.updateReviewDialog(false);
                let self = this;
                setTimeout(()=>{
                    self.setStudentDialogState(this.tutoringMain.startSessionDialogStateEnum.finished);
                    self.updateStudentStartDialog(true);
                }, 400);
            }

        },
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .leave_review_wrap {
        padding: 14px 20px;
        background-color: @color-white;
        .review_close_icon {
            color: #a4a3be;
        }
        .review_title {
            font-size: 20px;
            color: @global-purple;
        }
        .review_sub_title {
            color: @global-purple;

            .review_sub_title_step2 {
                // font-size: 16px;
            }
        }
        .review_user_rate {
            display: flex;
            align-items: center;

            .review_stars {
                flex-grow: 0;
                margin-right: 4px;
            }
            .review_start_rate {
                color: @global-purple;
                font-weight: 600;
            }
        }
        .review_error {
            font-size: 12px;
            color: #ff0000;
        }
        .review_textarea {
            padding-left: 50px;

            @media (max-width: @screen-xs) {
                padding-left: unset;
            }
        }
        .review_btn {
            min-width: 122px !important; //vuetify
            margin: 6px 8px;
            &.review_btn-back {
                border: 1px solid #4452fc !important;
                color: #4452fc;
            }
            .v-btn__content {
                font-size: 18px;
            }
        }




        
        // .v-text-field--outline > .v-input__control > .v-input__slot {
        //     border: 1px solid rgba(0, 0, 0, 0.19);
        //     font-size: 14px;
        //     &:hover {
        //         border: 1px solid rgba(0, 0, 0, 0.19) !important;
        //     }
        // }
        // .v-textarea.v-text-field--enclosed textarea {
        //     margin-top: 12px;
        // }

        // .white-text {
        //     color: @color-white;
        // }
        // .blue-text {
        //     color: @global-blue;
        // }
        // .middle-layout-wrapper{
        //     direction: ltr /*rtl:ltr*/;
        //     &.border-grey {
        //         border-top: 1px solid rgba(67, 66, 93, 0.2);
        //         border-bottom: 1px solid rgba(67, 66, 93, 0.2);
        //     }
        //     .image-container{
        //         margin-right:24px/*rtl:ignore*/;
        //         padding: 16px 0/*rtl:ignore*/;
        //         .tutor-img-wrap{
        //             max-width: 54px;
        //         }
        //     }
            
        // }
        
        // .submit-review {
        //     height: 42px;
        // }
        // #submit-review-id{
        //     &:disabled {
        //         background-color: #f0f0f7!important; //vuetify
        //     }
        // }
        // .review-heading {
        //     height: 46px;
        //     background-color: @systemBackgroundColor;
        //     .heading-text {
        //         color: @global-purple;
        //     }
        // }
    }

</style>