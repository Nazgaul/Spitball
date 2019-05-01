<template>
    <div class="leave-review-wrap">
        <v-layout align-center justify-space-between class="review-heading px-3">
            <v-flex xs11 sm11 md11 grow>
                <span class="heading-text font-weight-bold" v-language:inner>leaveReview_title</span>
            </v-flex>
            <v-flex xs1 sm1 md1 class="text-xs-right">
                <v-icon class="body-2 review-close-icon" @click="closeReviewDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <div v-if="!reviewSent">
            <v-layout row wrap class="pt-4 pb-2" :class="{'pt-5 pb-4': $vuetify.breakpoint.xsOnly}">
                <v-flex xs12 sm12 md12 class="text-xs-center text-sm-center">
                    <span class="review-title font-weight-bold" v-language:inner>leaveReview_subtitle</span>
                </v-flex>
                <v-flex xs12 sm12 md12 class="text-xs-center text-sm-center pt-2">
                    <span class="body-2" v-language:inner>leaveReview_press_star</span>
                    <span class="body-2 pl-1">{{tutorName}}</span>
                </v-flex>
            </v-layout>
            <v-layout align-center justify-center class="py-2">
                <v-flex xs12 sm7 md7 class="text-xs-center d-inline-flex border-grey py-3">
                    <user-avatar size="54" :userImageUrl="tutorImage" :user-name="tutorId"/>
                    <userRating :rating="1"
                                :rateNumColor="'#43425D'"
                                :size="'30'"
                                :readonly="false"
                                :showRateNumber="false"
                                :callbackFn="setRateStar"
                                :rate-num-color="'#43425D'"></userRating>
                </v-flex>
            </v-layout>
            <v-layout v-if="reviewInputHidden" align-center justify-center class="pt-2"
                      :class="{'pt-4': $vuetify.breakpoint.xsOnly}">
                <v-flex @click="toggleReviewInput()" xs12 sm8 md8 class="text-xs-center  cursor-pointer">
                <span class="mr-2">
                    <v-icon class="blue-text body-2">sbf-edit-icon</v-icon>
                </span>
                    <span class="blue-text body-2" v-language:inner>leaveReview_write</span>
                </v-flex>
            </v-layout>
            <transition v-else name="fade">
                <v-layout align-center justify-center class="pt-3 px-3">
                    <v-flex xs12 sm12 md12 class="text-xs-center">
                        <v-textarea
                                rows="1"
                                outline
                                v-model="reviewText"
                                name="input-review"
                                auto-grow
                                :placeholder="reviewPlaceholder"
                        ></v-textarea>
                    </v-flex>
                </v-layout>
            </transition>
            <v-layout align-center justify-center class="pt-4 pb-3">
                <v-flex xs12 sm6 md6 class="text-xs-center">
                    <v-btn @click="sendReview()"
                           :loading="btnLoading"
                           color="#4452FC"
                           round
                           class="white-text elevation-0 py-2 submit-review">
                        <span class="text-capitalize px-4 subheading" v-language:inner>leaveReview_btn_send_review</span>
                    </v-btn>
                </v-flex>
            </v-layout>
        </div>
        <finalReviewStep v-else :tutorId="tutorId"></finalReviewStep>
    </div>
</template>

<script>
    import userRating from '../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import { mapActions, mapGetters } from 'vuex';
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
    import finalReviewStep from './leaveReviewHelpers/finalScreen.vue';
    import utilitiesService from "../../../../services/utilities/utilitiesService";
    import { LanguageService } from "../../../../services/language/languageService";

    export default {
        components: {userRating, userAvatar, finalReviewStep},
        name: "leaveReview",
        data() {
            return {
                reviewText: '',
                reviewInputHidden: true,
                reviewSent: false,
                btnLoading: false,
                reviewVal: 0,
                roomId: 0,
                reviewPlaceholder: LanguageService.getValueByKey("leaveReview_review_placeholder")
            };
        },
        computed: {
            ...mapGetters(['getActiveConversationObj', 'getReview', 'getStudyRoomData']),
            tutorImage() {
                return utilitiesService.proccessImageURL(this.getActiveConversationObj.image, 54, 54);
            },
            tutorId() {
                return this.getActiveConversationObj && this.getActiveConversationObj.userId && this.getActiveConversationObj.userId.toString();
            },
            tutorName() {
                return this.getActiveConversationObj.name;
            }

        },
        methods: {
            ...mapActions(['submitReview', 'updateReviewDialog', 'updateReviewStars', 'updateReview']),
            toggleReviewInput() {
                return this.reviewInputHidden = !this.reviewInputHidden;
            },
            setRateStar(val){
                console.log('rate',val);
                if(val >0){
                    this.updateReviewStars(val)
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
                              }
                          },
                          (error) => {
                              console.log('error sending review', error);
                              this.btnLoading = false;
                          }
                    ).finally((done) => {
                    this.btnLoading = false;

                });
            },
            closeReviewDialog() {
                this.updateReviewDialog(false);
            }

        },
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .leave-review-wrap {
        width: 100%;
        background-color: @color-white;
        .review-close-icon {
            color: #a4a3be;
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: 1px solid rgba(0, 0, 0, 0.19);
            font-size: 14px;
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
        .v-textarea.v-text-field--enclosed textarea {
            margin-top: 12px;
        }

        .white-text {
            color: @color-white;
        }
        .blue-text {
            color: @colorBlue;
        }
        .border-grey {
            border-top: 1px solid rgba(67, 66, 93, 0.2);
            border-bottom: 1px solid rgba(67, 66, 93, 0.2);
        }
        .submit-review {
            height: 42px;
        }
        .review-heading {
            height: 46px;
            background-color: @systemBackgroundColor;
            .heading-text {
                color: @profileTextColor;
            }
        }
        .review-texts {

        }
        .review-title {
            font-size: 18px;
            color: @profileTextColor;
        }
    }

</style>