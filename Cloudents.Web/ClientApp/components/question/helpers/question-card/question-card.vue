<template>
    <!-- ********************** THIS IS THE ANSWER CARD ********************** -->
    <v-flex v-if="cardData && !isDeleted " class="question-card">
        <div class="question-card-answer transparent">
            <div class="full-width-flex">
                <div>
                    <user-block class="q-user-block" :cardData="cardData" :user="cardData.user"></user-block>
                </div>

                <div class="full-width-flex">
                    <div class="full-width-flex calc-Margin answer-block">
                        <div class="triangle"></div>
                        <div class="text-container">
                            <div class="text">
                                <div class="answer-header-left-container">
                                    <span class="timeago ml-2">{{date}}</span>
                                </div>
                                <v-spacer></v-spacer>
                                <div class="menu-area">
                                    <v-menu bottom left content-class="card-user-actions">
                                        <template v-slot:activator="{ on }">
                                            <v-btn :depressed="true" @click.prevent v-on="on" icon>
                                                <v-icon small>sbf-3-dot</v-icon>
                                            </v-btn>
                                        </template>
                                        <v-list>
                                            <v-list-item
                                                    v-show="item.isVisible"
                                                    :disabled="item.isDisabled()"
                                                    v-for="(item, i) in actions"
                                                    :key="i"
                                            >
                                                <v-list-item-title @click="item.action()">{{ item.title }}
                                                </v-list-item-title>
                                            </v-list-item>
                                        </v-list>
                                    </v-menu>
                                </div>
                            </div>
                            
                            <p :class="['q-text',{'answer': typeAnswer}]">{{cardData.text}}</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <sb-dialog :showDialog="showReport" :maxWidth="'438px'" :popUpType="'reportDialog'" :content-class="`reportDialog`">
            <report-item
                :closeReport="closeReportDialog"
                :itemType="'answer'"
                :answerDelData="answerToDeletObj"
                :itemId="itemId" 
            />
        </sb-dialog>
    </v-flex>
</template>

<style src="./question-card.less" lang="less"></style>
<script>
import { mapGetters, mapActions } from 'vuex'

import userBlock from "./../../../helpers/user-block/user-block.vue";
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import reportItem from "../../../results/helpers/reportItem/reportItem.vue"
import timeAgoService from '../../../../services/language/timeAgoService';

export default {
    components: {
        userBlock,
        sbDialog,
        reportItem
    },
    props: {
        cardData: {},
        typeAnswer: {
            type: Boolean,
            required: false,
            default: false
        },
    },
    data() {
        return {
            actions: [
                {
                    title: this.$t("questionCard_Report"),
                    action: this.reportItem,
                    isDisabled: this.isDisabled,
                    isVisible: true
                },
                {
                    title: this.$t("questionCard_Delete"),
                    action: this.deleteQuestion,
                    isDisabled: this.isDeleteDisabled,
                    isVisible: true
                }
            ],
            showReport: false, // ok
            itemId: 0, // ok
            answerToDeletObj: {}, // ok
            isDeleted: false, // ok
            toasterText: '',
        };
    },
    computed: {
        date() {           
            return timeAgoService.timeAgoFormat(this.cardData.create);
        }
    },
    methods: {
        ...mapActions({
            'delete': 'deleteQuestion',
            updateToasterParams: 'updateToasterParams',
            removeQuestionItemAction: 'removeQuestionItemAction',
            manualAnswerRemove: 'answerRemoved',
        }),
        ...mapGetters(['accountUser']),

        cardOwner() {
            let user = this.accountUser();
            if (user && this.cardData.user) {
                return user.id === this.cardData.user.id; // will work once API call will also return userId
            }
            return false;
        },
        isDeleteDisabled(){
            let isDeleteble = this.canDelete();
            return !isDeleteble;

        },
        isDisabled() {
            let isOwner, account;
            isOwner = this.cardOwner();
            account = this.accountUser();
            if (isOwner || !account) {
                return true;
            }
        },
        reportItem() {
            this.itemId = this.cardData.id;
            let answerToHide = {
                questionId: parseInt(this.$route.params.id),
                answer: {
                    id: this.itemId
                }
            };
            //assign to obj passed as prop to report component
            this.answerToDeletObj =  Object.assign(answerToHide);
            this.showReport = !this.showReport;
        },
        closeReportDialog() {
            this.showReport = false;
        },
        canDelete() {
           let isOwner = this.cardOwner();
           return isOwner;
        },
        deleteQuestion() {
            let self = this
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')})
                .then(() => {
                        let text = self.typeAnswer ? 'helpers_questionCard_toasterDeleted_answer' : 'helpers_questionCard_toasterDeleted_question'
                        self.updateToasterParams({
                            toasterText: self.$t(text),
                            showToaster: true
                        });
                        if (!self.typeAnswer) {
                            let objToDelete = {
                                id: parseInt(self.$route.params.id)
                            };
                            self.$ga.event("Delete_question", "Homework help");
                            //ToDO change to router link use and not text URL
                            self.removeQuestionItemAction(objToDelete);
                            self.$router.push('/ask');
                        } else {
                            //emit to root to update array of answers
                            self.$ga.event("Delete_answer", "Homework help");

                            //delete object Manually
                            let answerToRemove = {
                                questionId: parseInt(self.$route.params.id),
                                answer: {
                                    id: self.cardData.id
                                }
                            };
                            self.manualAnswerRemove(answerToRemove);
                            self.isDeleted = true;
                        }
                    }
                );
        },
    }
}
</script>