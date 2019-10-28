<template>
<transition name="fade">
    <div class="answer-question" v-if="questionData">
        <!-- Web version -->
        <div class="pt-3 question-wrap" v-if="!$vuetify.breakpoint.smAndDown"
             :class="{'my-question': cardOwner}">
            <v-flex xs12 class="breadcrumbs">
                <a style="cursor:pointer;" @click="resetSearch()" v-language:inner>questionDetails_Ask_question</a>
                <span class="question-category"> <span v-language:inner>questionDetails_question_dash</span>{{questionData.subject}}</span>
            </v-flex>
            <v-layout row>
                <v-flex style="width:inherit;" class="question-data">
                    <question-thread 
                        v-if="questionData" 
                        :questionData="questionData"
                        :cardOwner="cardOwner"
                        :showDialogSuggestQuestion="showDialogSuggestQuestion"
                        :hasCorrectAnswer="getCorrectAnswer">
                            <div slot="currently-watching"></div>
                    </question-thread>
                    <div slot="answer-form" class="mb-3" style="width:inherit;">
                            <div style="position:relative;width:inherit;" v-if="(accountUser&&!questionData.answers.length) || (questionData.answers.length && showForm)" key="one">
                                <extended-text-area 
                                    uploadUrl="/api/question/ask"
                                    v-model="textAreaValue"
                                    :error="errorTextArea"
                                    @addFile="addFile"
                                    @removeFile="removeFile">
                                </extended-text-area>
                                <div class="has-answer-error-wrapper">
                                    <span v-if="errorHasAnswer.length" class="error-message  has-answer-error">{{errorHasAnswer}}</span>
                                    <span v-if="errorDuplicatedAnswer.length" class="error-message  has-answer-error">{{errorDuplicatedAnswer}}</span>
                                </div>
                                <v-btn block color="primary"
                                       @click="submitAnswer()"
                                       :disabled="submitted"
                                       class="add_answer"><span v-language:inner>questionDetails_Add_answer</span> 
                                </v-btn>
                            </div>

                            <div v-else class="show-form-trigger" @click="showAnswerField()"  key="two">
                                <div><b v-language:inner>questionDetails_Know_the_answer</b>&nbsp; <span v-language:inner>questionDetails_Add_it_here</span></div>
                            </div>
                        </div>
                </v-flex>
                    <!--TODO V 20  SPITBALL-851 Remove the discussion board from question pages -->
                <!--<v-flex v-if ="accountUser" class="chat-wrapper" :class="{'position-static': isEdgeRtl}">-->
                    <!--<div class="chat-title pa-2" v-language:inner>questionDetails_Discussion_Board</div>-->
                    <!--<div ref="chat-area" class="chat-container"></div>-->

                <!--</v-flex>-->

            </v-layout>
        </div>

        <!-- Mobile version with tabs hfgfh -->
        <div v-else>
            <v-tabs grow>
                    <v-tabs-slider color="blue"></v-tabs-slider>
                    <v-tab :href="'#tab-1'" :key="'1'"><span v-language:inner>questionDetails_Question</span></v-tab>
                      <!--TODO V 20  SPITBALL-851 Remove the discussion board from question pages -->
                    <!--<v-tab :href="'#tab-2'" :key="'2'" v-if="accountUser"><span v-language:inner>questionDetails_Chat</span></v-tab>-->

                <v-tab-item :key="'1'" :id="'tab-1'" class="tab-padding">
                        <v-flex xs12 class="question-data" >
                            <question-thread v-if="questionData" :questionData="questionData" :cardOwner="cardOwner"
                                             :hasCorrectAnswer="getCorrectAnswer">
                                
                            </question-thread>
                            <div slot="answer-form" class="answer-form mb-3">
                                    <div v-if="(accountUser&&!questionData.answers.length) || (questionData.answers.length && showForm)">
                                        <extended-text-area 
                                            uploadUrl="/api/question/ask"
                                            v-model="textAreaValue"
                                            :error="errorTextArea"
                                            @addFile="addFile"
                                            @removeFile="removeFile">
                                        </extended-text-area>
                                        <div class="has-answer-error-wrapper">
                                            <span v-if="errorHasAnswer.length" class="error-message  has-answer-error">{{errorHasAnswer}}</span>
                                        </div>
                                        <v-btn color="primary" @click="submitAnswer()"
                                               :disabled="submitted"
                                               class="add_answer"><span  v-language:inner>questionDetails_Add_answer</span> 
                                        </v-btn>
                                    </div>
                                    <div v-else class="show-form-trigger" @click="showAnswerField()">
                                        <div><b v-language:inner>questionDetails_Know_the_answer</b>&nbsp;<span v-language:inner>questionDetails_Add_it_here</span></div>
                                    </div>
                                </div>
                        </v-flex>
                </v-tab-item>
                <!--TODO V 20  SPITBALL-851 Remove the discussion board from question pages -->
                <!--<v-tab-item :key="'2'" :id="'tab-2'">-->
                        <!--<v-flex xs12>-->
                        <!--<div ref="chat-area" class="chat-iframe"></div>-->
                        <!--</v-flex>-->
                <!--</v-tab-item>-->
            </v-tabs>
        </div>
        <!-- <sb-dialog :showDialog="showDialogSuggestQuestion" :popUpType="'suggestions'" :content-class="'question-suggest'">
                <question-suggest-pop-up  :user="questionData.user" :cardList="cardList"></question-suggest-pop-up>
        </sb-dialog> -->

        <sb-dialog :showDialog="loginDialogState" :popUpType="'loginPop'"  :content-class="'login-popup'">
                <login-to-answer></login-to-answer>
        </sb-dialog>

    </div>
</transition>
</template>

<style src="./questionDetails.less" lang="less"></style>
<script src="./questionDetails.js"></script>
