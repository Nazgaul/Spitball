<template>
    <div class="answer-question" v-if="questionData">

        <!-- Web version -->
        <div class="pt-3 question-wrap" v-if="!$vuetify.breakpoint.smAndDown" :class="{'my-question': questionData.cardOwner}">
            <v-flex xs12 class="breadcrumbs">
                <router-link class="ask-question" to="/ask">
                    Ask a question</router-link>

                <!--<a class="ask-question" href="/ask">-->
                <!--<v-icon>sbf-ask-q</v-icon>-->
                <!--Ask a question</a>-->
                <span class="question-category">/  Questions  / {{questionData.subject}}</span>
            </v-flex>
            <v-layout row>


                <v-flex class="question-data">
                    <question-thread v-if="questionData" :questionData="questionData">
                        <div v-if="enableAnswer" slot="answer-form" class="mb-3">
                            <div v-if="(accountUser&&!questionData.answers.length) || (questionData.answers.length && showForm)">
                                <extended-text-area uploadUrl="/api/upload/ask"
                                        v-model="textAreaValue"
                                        :isFocused="showForm" @addFile="addFile" @removeFile="removeFile"></extended-text-area>
                                <v-btn block color="primary" @click="submitAnswer()" :disabled="!this.textAreaValue.length||submitted"
                                       class="add_answer">Add your answer
                                </v-btn>
                            </div>
                            <div v-else class="show-form-trigger" @click="showAnswerField()">
                                <div><b>Know the answer?</b> Add it here!</div>
                            </div>
                        </div>
                    </question-thread>
                </v-flex>

                <v-flex class="chat-wrapper">
                    <div class="chat-title pa-2">Discussion Board</div>
                    <div ref="chat-area"></div>

                </v-flex>

            </v-layout>
        </div>

        <!-- Mobile version with tabs hfgfh -->
        <div v-else>
            <v-tabs grow>

                <v-tabs-bar>
                    <v-tabs-slider color="blue"></v-tabs-slider>
                    <v-tabs-item :href="'#tab-1'" :key="'1'">Question</v-tabs-item>
                    <v-tabs-item :href="'#tab-2'" :key="'2'">Chat</v-tabs-item>
                </v-tabs-bar>

                <v-tabs-items>

                    <v-tabs-content :key="'1'" :id="'tab-1'" class="tab-padding">
                        <v-flex xs12>
                            <question-thread v-if="questionData" :questionData="questionData" :hasCorrectAnswer="getCorrectAnswer">
                                <div slot="answer-form" class="answer-form mb-3 mt-3" v-if="enableAnswer">
                                    <div v-if="(accountUser&&!questionData.answers.length) || (questionData.answers.length && showForm)">
                                        <extended-text-area uploadUrl="/api/upload/ask"
                                                v-model="textAreaValue"
                                                :isFocused="showForm" @addFile="addFile" @removeFile="removeFile"></extended-text-area>
                                        <v-btn  color="primary" @click="submitAnswer()" :disabled="!this.textAreaValue.length"
                                               class="add_answer">Add your answer
                                        </v-btn>
                                    </div>
                                    <div v-else class="show-form-trigger" @click="showAnswerField()">
                                        <div><b>Know the answer?</b> Add it here!</div>
                                    </div>
                                </div>
                            </question-thread>
                        </v-flex>
                    </v-tabs-content>

                    <v-tabs-content :key="'2'" :id="'tab-2'">
                        <v-flex xs12>
                            <div ref="chat-area" class="chat-iframe"></div>
                        </v-flex>
                    </v-tabs-content>

                </v-tabs-items>

            </v-tabs>
        </div>
    </div>
</template>

<style src="./questionDetails.less" lang="less"></style>
<script src="./questionDetails.js"></script>
