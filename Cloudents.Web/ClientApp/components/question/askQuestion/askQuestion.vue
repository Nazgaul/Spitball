<template>
    <transition name="fade">
        <div class="add-question-container">
            <v-form ref="questionForm">
                <v-layout class="question-header">
                    <div class="mx-auto" v-language:inner="'addQuestion_title'"></div>
                </v-layout>

                <div class="question-textarea-container">
                    <div class="question-textarea-upper-part">
                        <div class="question-profile-data-container">
                            <user-avatar :userImageUrl="userImageUrl" :user-name="userName"></user-avatar>
                        </div>
                        <v-textarea
                                solo
                                no-resize
                                name="add-question-textarea"
                                :label="topicPlaceholder"
                                class="question-textarea elevation-0"
                                :rows="7"
                                :rules="[rules.required, rules.maximumChars, rules.minimumChars]"
                                v-model="questionText" :value="questionText"
                        ></v-textarea>
                    </div>
                </div>

                <div class="question-textarea-courses">
                    <fieldset class="question-fieldset px-2">
                        <legend v-language:inner="'addQuestion_class_placeholder'"/>
                        <v-combobox 
                            class="text-truncate pa-0"
                            flat
                            hide-no-data
                            :append-icon="''"
                            v-model="questionCourse"
                            :items="suggestsCourses"
                            :rules="[rules.required]">
                        </v-combobox>
                    </fieldset>
                </div>
                
                <v-layout class="question-add-button-container pt-6 pb-4" :class="{'question-mobile': $vuetify.breakpoint.xsOnly}" align-center justify-center column>
                    <v-flex xs12 class="text-center">
                        <v-btn sel="cancel_ask"
                            class="question-add-button font-weight-bold px-3 button_cancel" 
                            text 
                            @click="closeAddQuestionDialog">
                                <span v-language:inner="'addQuestion_cancel_button'"></span>
                        </v-btn>
                        <v-btn sel="post"
                            :loading="btnQuestionLoading"
                            text
                            class="question-add-button font-weight-bold px-3"
                            @click="submitQuestion">
                                <span v-language:inner="'addQuestion_add_button'"></span>
                        </v-btn>
                    </v-flex>
                    <v-flex xs12 v-if="errorMessage" class="error--text">{{errorMessage}}</v-flex>
                </v-layout>
            </v-form>
        </div>
    </transition>
</template>



<style lang="less" src="./askQuestion.less"></style>
<script>
import { mapActions, mapGetters } from 'vuex';
import { validationRules } from "../../../services/utilities/formValidationRules";
import questionService from "../../../services/questionService";
import analyticsService from "../../../services/analytics.service";

export default {
    data() {
        return {
            questionCourse: '',
            questionText: '',
            suggestsCourses: [],
            btnQuestionLoading: false,
            errorMessage: '',
            rules: {
                required: (value) => validationRules.required(value),
                maximumChars: (value) => validationRules.maximumChars(value, 255),
                minimumChars: (value) => validationRules.minimumChars(value, 15),
            },
            topicPlaceholder: this.$t('addQuestion_ask_your_question_placeholder'),
        };
    },
    computed: {
        ...mapGetters(['accountUser']),
        userImageUrl() {
            if(this.accountUser && this.accountUser.image.length > 1) {
                return `${this.accountUser.image}`;
            }
            return '';
        },
        userName() {
            if(this.accountUser && this.accountUser.name) {
                return this.accountUser.name;
            }
            return '';
        }
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState']),
        requestAskClose() {
            this.updateNewQuestionDialogState(false);
        },
        submitQuestion() {
            let self = this;
            if(self.$refs.questionForm.validate()) {
                self.btnQuestionLoading =true;
                let serverQuestionObj = {
                    text: self.questionText,
                    course: self.questionCourse.text ? self.questionCourse.text : self.questionCourse,
                };
                questionService.postQuestion(serverQuestionObj).then(() => {
                    analyticsService.sb_unitedEvent("Submit_question", "Homework help");
                    self.btnQuestionLoading =false;
                    //close dialog after question submitted
                    self.requestAskClose(false);
                    self.$router.push({path: '/'});
                }, (error) => {                    
                    let errorMessage = self.$t('addQuestion_error_general');
                    if (error && error.response && error.response.data && error.response.data[""] && error.response.data[""][0]) {
                        errorMessage = error.response.data[""][0];
                    }
                    self.errorMessage = errorMessage;
                }).finally(()=>{
                    self.btnQuestionLoading =false;
                });
            }
        },
        closeAddQuestionDialog() {
            this.updateNewQuestionDialogState(false);
        },
    },
    created() {
        this.$store.dispatch('updateFeedCourses').then(({data}) => {
            this.suggestsCourses = data
        })
        if(this.$route.query && this.$route.query.term){
            this.questionCourse = this.$route.query.term;
        }
        if(this.$route.query && this.$route.query.Course){
            this.questionCourse = this.$route.query.Course;
        }
    }
};
</script>