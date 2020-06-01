<template>
<transition name="fade">
    <div class="answer-question" v-if="questionData">
        <!-- Web version -->
        <div class="pt-4 question-wrap" v-if="!$vuetify.breakpoint.smAndDown"
             :class="{'my-question': cardOwner}">
            <v-flex xs12 class="breadcrumbs">
                <a style="cursor:pointer;" @click="resetSearch()" v-language:inner>questionDetails_Ask_question</a>
                <span class="question-category"> <span v-language:inner>questionDetails_question_dash</span>{{questionData.subject}}</span>
            </v-flex>
            <v-layout>
                <v-flex style="width:inherit;" class="question-data">
                    <question-thread 
                        v-if="questionData" 
                        :questionData="questionData">
                    </question-thread>
                    <div slot="answer-form" class="mb-4" style="width:inherit;" v-if="!cardOwner && !userAnswered">
                            <div style="position:relative;width:inherit;" v-if="(accountUser&&!questionData.answers.length) || (questionData.answers.length && showForm)" key="one">
                                <extended-text-area 
                                    v-model="textAreaValue"
                                    :error="errorTextArea">
                                </extended-text-area>
                                <div class="has-answer-error-wrapper">
                                    <span v-if="errorDuplicatedAnswer.length" class="error-message  has-answer-error">{{errorDuplicatedAnswer}}</span>
                                </div>
                                <v-btn block color="primary"
                                       @click="submitAnswer()"
                                       :loading="submitLoader"
                                       class="add_answer"><span v-language:inner>questionDetails_Add_answer</span> 
                                </v-btn>
                            </div>

                            <div v-else class="show-form-trigger" @click="showAnswerField()"  key="two">
                                <div><b v-language:inner>questionDetails_Know_the_answer</b>&nbsp; <span v-language:inner>questionDetails_Add_it_here</span></div>
                            </div>
                        </div>
                </v-flex>
            </v-layout>
        </div>
        <div v-else>
            <v-tabs grow>
                    <v-tabs-slider color="blue"></v-tabs-slider>
                    <v-tab :href="'#tab-1'" :key="'1'"><span v-language:inner>questionDetails_Question</span></v-tab>
                <v-tab-item :key="'1'" :id="'tab-1'" class="tab-padding">
                        <v-flex xs12 class="question-data" >
                            <question-thread v-if="questionData" :questionData="questionData"></question-thread>
                            <div slot="answer-form" class="answer-form mb-4" v-if="!cardOwner && !userAnswered">
                                    <div v-if="(accountUser && !questionData.answers.length) || (questionData.answers.length && showForm)">
                                        <extended-text-area 
                                            v-model="textAreaValue"
                                            :error="errorTextArea">
                                        </extended-text-area>
                                        <v-btn color="primary" @click="submitAnswer()"
                                               :loading="submitLoader"
                                               class="add_answer"><span  v-language:inner>questionDetails_Add_answer</span> 
                                        </v-btn>
                                    </div>
                                    <div v-else class="show-form-trigger" @click="showAnswerField()">
                                        <div><b v-language:inner>questionDetails_Know_the_answer</b>&nbsp;<span v-language:inner>questionDetails_Add_it_here</span></div>
                                    </div>
                                </div>
                        </v-flex>
                </v-tab-item>
            </v-tabs>
        </div>
    </div>
</transition>
</template>

<style src="./questionDetails.less" lang="less"></style>
<script>
    import { mapGetters, mapActions } from 'vuex';
    import questionThread from "./questionThread.vue";
    import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
    import questionService from "../../../services/questionService";
    import disableForm from "../../mixins/submitDisableMixin.js";
    import analyticsService from '../../../services/analytics.service';
    import { LanguageService } from "../../../services/language/languageService";

    export default {
        mixins: [disableForm],
        components: {questionThread,extendedTextArea},
        props: {
            id: {Number}, // got it from route
        },
        data() {
            return {
                textAreaValue: "",
                errorDuplicatedAnswer:'',
                errorLength:{},
                showForm: false,
                submitLoader: false,
                hasData: false
            };
        },
        beforeRouteLeave(to, from, next) {
            this.resetQuestion();
            next();
        },
        methods: {
            ...mapActions([
                "resetQuestion",
                'setQuestion'
            ]),
            ...mapGetters(["getQuestion"]),
            resetSearch(){
                this.$router.push({name: "feed"});
            },
            submitAnswer() {
                
                if (!this.textAreaValue || this.textAreaValue.trim().length < 15) {
                    this.errorLength= {
                        errorText: LanguageService.getValueByKey("questionDetails_error_minChar"),
                        errorClass: true
                    };
                    return;
                }
                if (!this.textAreaValue || this.textAreaValue.trim().length > 540) {
                    this.errorLength= {
                        errorText: LanguageService.getValueByKey("questionDetails_error_maxChar"),
                        errorClass: true
                    };
                    return;
                }
                var self = this;
                if(this.hasDuplicatiedAnswer(self.textAreaValue, self.questionData.answers)) {
                    this.errorDuplicatedAnswer = LanguageService.getValueByKey("questionDetails_error_duplicated");
                    return;
                }else{
                    this.errorDuplicatedAnswer = '';
                }
                if (self.submitForm()) {
                    self.textAreaValue = self.textAreaValue.trim();
                    this.submitLoader = true;
                    questionService.answerQuestion(self.id, self.textAreaValue)
                        .then(function () {                       
                            analyticsService.sb_unitedEvent("Submit_answer", "Homwork help");
                            self.textAreaValue = "";
                            self.getData(); //TODO: remove this line when doing the client side data rendering (make sure to handle delete as well)
                        }, (error) => {
                            console.log(error);
                            // self. = error.response.data["Text"] ? error.response.data["Text"][0] : '';
                            self.submitForm(false);
                        
                        }).finally(()=>{
                            this.submitLoader = false;
                        });
                }
            },
            hasDuplicatiedAnswer(currentText, answers){  
                let duplicated = answers.filter(answer=>{
                    return answer.text.indexOf(currentText) > -1;
                });
                return duplicated.length > 0;
            },

            getData() {
                //enable submit btn
                this.$data.submitted = false;
                this.setQuestion(this.id).then(()=>{
                }).finally(() => {
                    this.hasData = true
                })
            },
            showAnswerField() {            
                if (this.accountUser) {
                    this.showForm = true;
                }
                else {
                    this.$store.commit('setComponent', 'register')
                }
            },
            goToAnswer(hash) {
                this.$vuetify.goTo(hash)
            }
        },
        watch: {
            textAreaValue(){
                this.errorLength = {};
            },
            hasData(val) {
                let hash = this.$route.hash
                if(hash && val) {
                    this.goToAnswer(hash);
                }
            },
            //watch route(url query) update, and het question data from server
            '$route': 'getData'
        },
        computed: {
            ...mapGetters(["accountUser", "isCardOwner"]),
            questionData(){
                return this.getQuestion();
            },
            errorTextArea(){
                    return this.errorLength;
            },
            cardOwner(){
                return this.isCardOwner;
            },
            userAnswered() {
                if(this.accountUser) {
                    return this.questionData.answers.length && this.questionData.answers.filter(i => i.user.id === this.accountUser.id).length;
                }
                return null;
            },
        },
        created() {               
            this.getData();
        },
    }
</script>