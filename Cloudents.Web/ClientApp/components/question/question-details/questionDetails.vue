<template>
    <div class="answer-question" v-if="questionData">

        <!-- Web version -->
        <div class="pt-3 question-wrap" v-if="!$vuetify.breakpoint.smAndDown"
             :class="{'my-question': questionData.cardOwner}">
            <v-flex xs12 class="breadcrumbs">
                <router-link class="ask-question" to="/ask">
                    Ask a question
                </router-link>
                <span class="question-category">/  Questions  / {{questionData.subject}}</span>
            </v-flex>
            <v-layout row>
                <v-flex class="question-data">
                    <question-thread v-if="questionData" :questionData="questionData"
                                     :showDialogSuggestQuestion="showDialogSuggestQuestion"
                                     :hasCorrectAnswer="getCorrectAnswer">

                        <div v-if="enableAnswer" slot="answer-form" class="mb-3">
                            <transition name="fade">
                            <div v-if="(accountUser&&!questionData.answers.length) || (questionData.answers.length && showForm)" key="one" class="smoothAnimateFast">
                                <extended-text-area uploadUrl="/api/upload/ask"
                                                    v-model="textAreaValue"
                                                    :error="errorTextArea"
                                                    :isFocused="showForm" @addFile="addFile"
                                                    @removeFile="removeFile"></extended-text-area>
                                <v-btn block color="primary"
                                       @click="submitAnswer()"
                                       :disabled="submitted"
                                       class="add_answer">Add your answer
                                </v-btn>
                            </div>

                            <div v-else class="show-form-trigger smoothAnimateSlow" @click="showAnswerField()"  key="two">
                                <div><b>Know the answer?</b> Add it here!</div>
                            </div>
                            </transition>
                        </div>
                    </question-thread>
                </v-flex>

                <v-flex v-if ="accountUser" class="chat-wrapper">
                    <div class="chat-title pa-2">Discussion Board</div>
                    <div ref="chat-area" class="chat-container"></div>

                </v-flex>

            </v-layout>
        </div>

        <!-- Mobile version with tabs hfgfh -->
        <div v-else>
            <v-tabs grow>
                    <v-tabs-slider color="blue"></v-tabs-slider>
                    <v-tab :href="'#tab-1'" :key="'1'">Question</v-tab>
                    <v-tab :href="'#tab-2'" :key="'2'" v-if="accountUser">Chat</v-tab>

                <v-tab-item :key="'1'" :id="'tab-1'" class="tab-padding">

                        <v-flex xs12>
                            <question-thread v-if="questionData" :questionData="questionData"
                                             :hasCorrectAnswer="getCorrectAnswer">
                                <div slot="answer-form" class="answer-form mb-3" v-if="enableAnswer">
                                    <div v-if="(accountUser&&!questionData.answers.length) || (questionData.answers.length && showForm)">
                                        <extended-text-area uploadUrl="/api/upload/ask"
                                                            v-model="textAreaValue"
                                                            :error="errorTextArea"
                                                            :isFocused="showForm" @addFile="addFile"
                                                            @removeFile="removeFile"></extended-text-area>
                                        <v-btn color="primary" @click="submitAnswer()"
                                               :disabled="submitted"
                                               class="add_answer">Add your answer
                                        </v-btn>
                                    </div>
                                    <div v-else class="show-form-trigger" @click="showAnswerField()">
                                        <div><b>Know the answer?</b> Add it here!</div>
                                    </div>
                                </div>
                            </question-thread>
                        </v-flex>
                </v-tab-item>
                <v-tab-item :key="'2'" :id="'tab-2'">
                        <v-flex xs12>
                            <div ref="chat-area" class="chat-iframe"></div>
                        </v-flex>
                </v-tab-item>
            </v-tabs>
        </div>
        <sb-dialog :showDialog="showDialogSuggestQuestion" :popUpType="'suggestions'" :content-class="'question-suggest'">
                <question-suggest-pop-up  :user="questionData.user" :cardList="cardList.nextQuestions"></question-suggest-pop-up>

        </sb-dialog>

        <sb-dialog :showDialog="showDialogLogin" :popUpType="'loginPop'"  :content-class="'login-popup'">
                <login-to-answer></login-to-answer>
        </sb-dialog>

    </div>
</template>

<style src="./questionDetails.less" lang="less"></style>
<script src="./questionDetails.js"></script>
