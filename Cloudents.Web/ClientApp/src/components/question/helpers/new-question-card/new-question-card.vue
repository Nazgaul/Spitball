<template>
    <div class="question-container">
        <router-link draggable="false" :to="{path:'/question/'+cardData.id}" :style="{'cursor':isQuestionPage?'auto':'pointer'}">
            <div class="question-header-container">
                <div class="question-header-large-sagment">
                    <div class="rank-date-container">
                        <!-- <user-avatar class="mr-1" size="34" :userImageUrl="cardData.user.image" :user-name="cardData.user.name" :user-id="cardData.user.id"/> -->
                        <userAvatarNew 
                            class="mr-1"
                            :userImageUrl="cardData.user.image"
                            :user-name="cardData.user.name"
                            :user-id="cardData.user.id"
                            :width="34"
                            :height="34"
                            :fontSize="14"
                        />
                        <div class="user-question">
                            <div class="user-question-name text-truncate">{{cardData.user.name}}</div>
                            <div class="user-question-date">{{$d(new Date(cardData.dateTime), 'short')}}</div>
                        </div>
                    </div>
                </div>
                <div class="question-header-small-sagment">
                    <div class="price-area" :class="{'sold': isSold}">
                        <v-icon class="has-correct">sbf-check-circle</v-icon>
                    </div>
                    <div class="menu-area">
                        <v-menu class="menu-area" bottom left content-class="card-user-actions">
                            <template v-slot:activator="{ on }">
                                <v-btn :depressed="true" @click.prevent icon v-on="on">
                                    <v-icon>sbf-3-dot</v-icon>
                                </v-btn>
                            </template>
                            <v-list>
                            <v-list-item v-show="item.isVisible" class="report-list-item" :disabled="item.isDisabled()" v-for="(item, i) in actions" :key="i">
                                <v-list-item-title style="cursor:pointer;" @click="item.action()">{{ item.title }}</v-list-item-title>
                            </v-list-item>
                            </v-list>
                        </v-menu>
                    </div>
                </div>
            </div>
            <div class="question-body-container" :class="{'ml-12': !$vuetify.breakpoint.xsOnly}">
                <div class="question-right-body-container">
                    <div class="question-body-content-container mt-2 mb-1" :class="{'question-ellipsis': $route.name === 'feed'}">
                        <div class="question-text">{{cardData.text}}</div>
                    </div>
                    <div class="question-body-course-container" :class="[answers ? 'mb-4' : 'mb-0']">
                        <div class="question-body-course_name text-truncate" v-if="cardData.course">
                            <span class="question-body-course_name_span" v-t="'resultNote_course'"></span>
                            <h2 class="question-body-course_name_h2"> {{cardData.course}}</h2>
                        </div>
                    </div>
                    <div class="gallery" v-if="cardData.files && cardData.files.length">
                        <v-carousel 
                            :prev-icon="isRtl ? 'sbf-arrow-right rigth' : 'sbf-arrow-right left'"
                            :next-icon="isRtl ? 'sbf-arrow-right left': 'sbf-arrow-right right'"
                            interval="600000" 
                            cycle 
                            full-screen
                            hide-delimiters 
                            :hide-controls="cardData.files.length === 1">
                            <v-carousel-item v-for="(item,i) in cardData.files" v-bind:src="item" :key="i" @click.native="showBigImage(item)"></v-carousel-item>
                        </v-carousel>
                    </div>
                </div>
                <v-dialog v-if="showDialog"
                          v-model="showDialog"
                          max-width="720px"
                          transition="scale-transition"
                          content-class="zoom-image">
                    <img :src="selectedImage" alt="" height="auto" width="100%" class="zoomed-image">
                </v-dialog>
            </div>
            <div class="question-footer-container" :class="{'ml-12': !$vuetify.breakpoint.xsOnly}">
                <div class="answer-display-container">
                    <div class="user_answer_wrap" v-if="answers">
                        <div class="user_answer_body mb-1">
                            <div class="d-flex mb-2 user_answer_aligment">
                                <user-avatar
                                    class="avatar-area"
                                    :user-name="answers.user.name"
                                    :userImageUrl="answers.user.image"
                                    :user-id="answers.user.id"
                                />
                                <div class="user_answer_info">
                                    <div class="user_answer_info_name text-truncate">{{answers.user.name}}</div>
                                    <div class="user_answer_info_date text-truncate">{{$d(new Date(cardData.firstAnswer.date), 'short')}}</div>
                                </div>
                            </div>
                        </div>
                        <div class="user_answer">{{answers.text}}</div>
                    </div>
                    <div v-if="cardData.answers > 1" class="more-answers">{{moreAnswersDictionary}}</div>
                    <div v-else class="mt-4"></div>
                </div>
                
            </div>
            <div v-if="!hideAnswerInput" class="question-bottom-section" :class="[{'mx-12': !$vuetify.breakpoint.xsOnly}, answersCount > 1 ? 'mt-0' : 'mt-6']">
                <div class="question-input-container d-flex">
                    <user-avatar class="avatar-area mr-2" :user-name="accountUser.name" :userImageUrl="accountUser.image" :user-id="accountUser.id" v-if="accountUser" />
                    <user-avatar class="avatar-area mr-2" :user-name="'JD'" :userImageUrl="''" v-else />
                    <input class="question-input" :placeholder="$t('questionCard_Answer_placeholder')" type="text">
                    <questionNote class="question-input-icon"/>
                </div>
            </div>

        </router-link>
        <sb-dialog
                :showDialog="showReportReasons"
                :maxWidth="'438px'"
                :popUpType="'reportDialog'"
                :content-class="`reportDialog ` ">
            <report-item :closeReport="closeReportDialog" :itemType="'Question'" :itemId="itemId"></report-item>
        </sb-dialog>
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';

import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
import reportItem from "../../../results/helpers/reportItem/reportItem.vue"

import questionNote from './question-note.svg';

export default {
    components: {
        sbDialog,
        reportItem,
        questionNote,
    },
    data() {
        return {
            actions: [
                {
                    title: this.$t("questionCard_Report"),
                    action: this.reportItem,
                    isDisabled: this.isDisabled,
                    isVisible: true,
                    icon: 'sbf-flag'
                },
                {
                    title: this.$t("questionCard_Delete"),
                    action: this.removeQuestion,
                    isDisabled: this.canDelete,
                    isVisible: true,
                    icon: 'sbf-delete'
                }
            ],
            showReportReasons: false,
            itemId: 0,
            maximumAnswersToDisplay: 3,
            isRtl: global.isRtl,
            showDialog: false,
            selectedImage: '',
            isQuestionPage: false,
        };
    },
    props: {
        cardData: {
            required: true
        },
        detailedView: {
            type: Boolean,
            default: false
        },
        suggestion:{
            type: Boolean,
            default: false
        }
    },
    computed: {
        ...mapGetters(['accountUser']),

        userImageUrl(){
            if( this.cardData && this.cardData.user &&  this.cardData.user.image && this.cardData.user.image.length > 1){
                return `${this.cardData.user.image}`;
            }
            return '';
        },
        hideAnswerInput() {           
            return this.detailedView;
        },
        isSold() {            
            return this.cardData.hasCorrectAnswer || this.cardData.correctAnswerId;
        },
        answersCount() {
            return this.cardData.answers;
        },
        answers() {           
            return this.cardData.firstAnswer;
        },
        answersNumber() {
            let answersNum = this.cardData.answers;
            let numericValue;
            if (typeof answersNum !== 'number') {
                numericValue = answersNum.length;
            } else {
                numericValue = answersNum;
            }
            if (numericValue > this.maximumAnswersToDisplay) {
                return this.maximumAnswersToDisplay;
            }
            return numericValue;
        },
        answersDeltaNumber() {
            let answersNum = this.cardData.answers || 1;
            let numericValue;
            if (typeof answersNum !== 'number') {
                numericValue = answersNum.length;
            } else {
                numericValue = answersNum;
            }
            let delta = 0;
            if (numericValue > this.maximumAnswersToDisplay) {
                delta = numericValue - this.maximumAnswersToDisplay;
            }
            return delta;
        },
        moreAnswersDictionary() {
            let answerCount = this.answersCount - 1
            return this.cardData.answers > 2 ? this.$t('questionCard_Answers', [answerCount]) : this.$t('questionCard_Answer_one', [answerCount]);
        }
        
    },
    methods: {
        ...mapActions([
            'deleteQuestion',
            'updateToasterParams',
            'removeQuestionItemAction',
        ]),

        isDisabled() {
            let isOwner = this.cardOwner();
            let account = this.accountUser;
            if (isOwner || !account ) {
                return true;
            }
            return false;

        },
        cardOwner() {
            let userAccount = this.accountUser;
            if (userAccount && this.cardData.userId) {
                return userAccount.id === this.cardData.userId; // will work once API call will also return userId
            }
            return false;
        },
        canDelete() {
            let isOwner = this.cardOwner();
            if (!isOwner) {
                return true;
            } else{
                return false;
            }
        },
        showBigImage(src) {
            this.showDialog = true;
            this.selectedImage = src;
        },
        removeQuestion() {
            let questionId = this.cardData.id;
            this.deleteQuestion({id: questionId, type: 'Question'}).then(() => {
                this.updateToasterParams({
                    toasterText: this.$t("helpers_questionCard_toasterDeleted_question"),
                    showToaster: true
                });
                let objToDelete = {
                    id: parseInt(questionId)
                };
                this.$ga.event("Delete_question", "Homework help");
                this.removeQuestionItemAction(objToDelete);
                if (this.$route.name === 'question') {
                    //redirect only if question got deleted from the question page
                    this.$router.push('/');
                }
            },
            (error) => {
                console.error(error);
                this.updateToasterParams({
                    toasterText: this.$t("questionCard_error_delete"),
                    showToaster: true
                });
            });
        },
        reportItem() {
            this.itemId = this.cardData.id;
            this.showReportReasons = !this.showReportReasons;
        },
        closeReportDialog() {
            this.showReportReasons = false;
        }
    },
    created() {
        this.isQuestionPage = (this.$route.name === 'question');
    },
};
</script>
<style lang="less" src="./new-question-card.less"></style>
