<template>
    <div class="leave_review_wrap">
        <closeIcon class="body-2 review_close_icon d-flex d-sm-none" @click="closeReviewDialog"></closeIcon>
        <!-- <v-icon class="body-2 review_close_icon d-flex d-sm-none" @click="closeReviewDialog">sbf-close</v-icon> -->
        <div class="d-flex">
            <div class="review_title text-center font-weight-bold mx-auto" v-language:inner="reviewsTitle"></div>
        </div>
        <div class="review_sub_title text-center mt-1" :class="{'mb-6': !showNextStep}">
            <span v-if="!showNextStep" v-html="$Ph('leaveReview_sub_title_step1', tutorName)"></span>
            <span class="review_sub_title_step2" v-else v-language:inner="'leaveReview_sub_title_step2'"></span>
        </div>
        <div class="review_user_rate" v-if="!showNextStep">
            <user-avatar class="tutor-img-wrap mr-2" :size="imgSize" :userImageUrl="tutorImg" :user-name="tutorName" :user-id="tutorId"/>
            <v-rating
                v-model="rating"
                ref="hahahahau"
                full-icon="sbf-star-rating-full"
                empty-icon="sbf-star-rating-empty"
                :hover="true"
                :size="28"
                :dense="true"
                :color="'#ffca54'"
                :background-color="'#ffca54'"
            >
            </v-rating>
            <span class="review_start_rate ml-2">{{ratingRate}}</span>
        </div>
        <div class="review_textarea" :class="{'review_textarea--noPadding':showNextStep}">
            <template>
                <div class="mb-1 review_error" v-if="reviewsError">
                    <span>{{errorText}}</span>
                </div>
                <div v-else class="review_no_error"></div>
            </template>  
            <v-textarea v-if="!showNextStep"
                rows="4"
                outlined
                autofocus
                v-model="reviewText"
                class="review_textarea_field"
                name="input-review"
                no-resize
                :placeholder="reviewPlaceholder"
            ></v-textarea>
            <v-form v-if="showNextStep" v-model="validReviewForm" ref="validReviewForm">        
                <v-textarea
                    rows="4"
                    outlined
                    autofocus
                    v-model="reviewText"
                    class="review_textarea_field"
                    name="input-review"
                    no-resize
                    :placeholder="reviewPlaceholder"
                    :rules="[rules.required]"
                ></v-textarea>
            </v-form>
                <!-- hide-details -->
        </div>
        
        <div class="text-center mt-4">
            <v-btn :loading="btnLoadingNoThx" @click="!showNextStep ? closeReviewDialog() : noThanks()" class="review_btn review_btn-back" outlined depressed rounded>
                <span v-language:inner="btnText"/>
            </v-btn>
            <v-btn :loading="btnLoading" @click="showNextStep ? sendPost() : goNextStep()" class="review_btn review_btn-next white--text" depressed rounded color="#4452fc" sel="submit_tutor_request">
                <span v-language:inner="'leaveReview_send'"/>
            </v-btn>
        </div>
    </div>
</template>

<script>
    import { mapActions, mapGetters, mapState } from 'vuex';

    import { LanguageService } from "../../../../services/language/languageService";
    import utilitiesService from "../../../../services/utilities/utilitiesService";
    import closeIcon from '../../../../font-icon/close.svg'
    import { validationRules } from '../../../../services/utilities/formValidationRules';


    export default {
        components: {closeIcon},
        name: "leaveReview",
        data() {
            return {
                errorText: '',
                reviewText: '',
                reviewsError: false,
                btnLoading: false,
                btnLoadingNoThx: false,
                showNextStep: false,
                roomId: 0,
                rating: 0,
                reviewPlaceholder: LanguageService.getValueByKey("leaveReview_review_placeholder"),
                imgSize: '42',
                starRate: [
                    '',
                    LanguageService.getValueByKey("leaveReview_star_1"),
                    LanguageService.getValueByKey("leaveReview_star_2"),
                    LanguageService.getValueByKey("leaveReview_star_3"),
                    LanguageService.getValueByKey("leaveReview_star_4"),
                    LanguageService.getValueByKey("leaveReview_star_5"),
                ],
                ratingScore: 0,
                rules: {
                    required: (value) => validationRules.required(value),
                },
                validReviewForm:false,
            };
        },
        computed: {
            ...mapState(['tutoringMain']),
            ...mapGetters(['getReview', 'getStudyRoomData']),

            tutorImg() {
                let size = [this.imgSize, this.imgSize];
                if(this.getStudyRoomData && this.getStudyRoomData.tutorImage){
                    return utilitiesService.proccessImageURL(this.getStudyRoomData.tutorImage, ...size);
                }
                return '';
            },
            tutorId() {
                return this.getStudyRoomData ?  this.getStudyRoomData.tutorId : '';
            },
            tutorName() {
                return this.getStudyRoomData ?  this.getStudyRoomData.tutorName : '';
            },
            ratingRate() {
                if(this.ratingScore === -1 && this.rating !== 0){
                    return this.starRate[this.rating];
                }else{
                    return this.starRate[this.ratingScore];
                }
            },
            reviewsTitle() {
                return this.showNextStep ? 'leaveReview_title_step2' : 'leaveReview_title_step1';
            },
            btnText() {
                return this.showNextStep ? 'leaveReview_noThanks' : 'leaveReview_back';
            },
        },
        watch: {
            rating(val) {
                this.rating = val;
                if(val > 0) {
                    this.reviewsError = false;
                    this.updateReviewStars(val);
                }
            },
        },
        methods: {
            ...mapActions(['submitReview', 'updateReviewDialog', 'updateReviewStars', 'updateReview', 'updateStudentStartDialog', 'setStudentDialogState']),

            noThanks() {
                this.btnLoadingNoThx = true;
                this.sendReview();
            },
            sendPost() {
                this.btnLoading = true;
                if(!this.$refs.validReviewForm.validate()) {
                    // this.setReviewError('leaveReview_emptyStarError');
                    this.btnLoading = false;
                } else {
                    this.sendReview();
                }
            },
            goNextStep() {
                if(this.rating > 0) {
                    if(this.reviewText.length) {
                        this.btnLoading = true;
                        this.sendReview();
                    } else {
                        this.showNextStep = true;
                        this.reviewsError = false;
                    }
                } else {
                    this.setReviewError('leaveReview_emptyStarError');
                }
            },
            sendReview() {
                this.submitReview({
                    roomId: this.getStudyRoomData.roomId,
                    review: this.reviewText,
                    rate: this.getReview.rate,
                    tutor: this.tutorId
                })
                .then(() => {
                    this.updateReview(null);
                    this.closeReviewDialog();
                },
                (error) => {
                    console.log('error sending review', error);
                    this.setReviewError('leaveReview_sendReviewError')
                }
                ).finally(() => {
                    this.btnLoading = false;
                    this.btnLoadingNoThx = false;
                });
            },
            setReviewError(err) {
                this.reviewsError = true;
                this.errorText = LanguageService.getValueByKey(err)
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
        mounted(){
            let self = this;
            this.$nextTick(function(){
                this.$refs.hahahahau.onMouseEnter = function(e, i){
                    this.runDelay('open', () => {
                    this.hoverIndex = this.genHoverIndex(e, i);
                    self.ratingScore = this.hoverIndex;
                });
                }
            })
            this.$nextTick(function(){
                this.$refs.hahahahau.onMouseLeave = function(){
                    self.$refs.hahahahau.runDelay('close', () => {
                        self.$refs.hahahahau.hoverIndex = -1
                        self.ratingScore = self.$refs.hahahahau.hoverIndex;
                    });
                }
            })   
        }
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .leave_review_wrap {
        padding: 14px 20px;
        background-color: @color-white;
        .review_close_icon {
            color: #a4a3be;
            display: flex;
            margin: 0 0 0 auto;
        }
        .review_title {
            font-size: 20px;
            color: @global-purple;
        }
        .review_sub_title {
            color: @global-purple;
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
            padding-left: 60px;
            &--noPadding {
                padding-left: unset;
            }
            @media (max-width: @screen-xs) {
                padding-left: unset;
            }
            .v-input__slot {
                fieldset {
                    border: 1px solid #c4c3d1;
                }
            }
            .review_textarea_field {
                border-radius: 6px;
                textarea {
                    font-size: 14px !important;
                    color: #6a697f !important;
                }
            }
            .review_no_error {
                min-height: 21px;
            }
        }
        .review_btn {
            text-transform: initial;
            min-width: @btnDialog !important; //vuetify
            height: 40px !important; //vuetify
            margin: 0 8px;
            font-weight: 600;
            &.review_btn-back {
                color: #4452fc;
            }
            .v-btn__content {
                font-size: 14px;
            }
        }
    }

</style>