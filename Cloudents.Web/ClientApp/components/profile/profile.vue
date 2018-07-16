<template>
    <div class="profile-page">
        <div>
            <v-layout class="data-wrapper" :class="{'column': isMobile}" wrap justify-start>

                <div>
                    <div class="main-block">
                        <button class="back" @click="$router.go(-1)">
                            <v-icon>sbf-arrow-right</v-icon>
                        </button>
                        <user-block v-if="profileData.user" :user="profileData.user"
                                    :classType="'university'"></user-block>

                        <div class="menu">
                            <ul class="tabs" v-if="!isMobile" xs3>
                                <li :class="{'active': activeTab === 1}" @click="activeTab = 1">
                                    <span v-if="isMyProfile">My&nbsp;</span>Questions
                                </li>
                                <li :class="{'active': activeTab === 2}" @click="activeTab = 2">
                                    <span v-if="isMyProfile">My&nbsp;</span>Answers
                                </li>
                            </ul>

                            <v-tabs v-else grow class="tab-padding" xs12>
                                <!--<v-tabs-bar>-->
                                    <v-tabs-slider color="blue"></v-tabs-slider>
                                    <v-tab @click="activeTab = 1" :href="'#tab-1'" :key="1"><span
                                            v-if="isMyProfile">My&nbsp;</span>Questions
                                    </v-tab>
                                    <v-tab @click="activeTab = 2" :href="'#tab-2'" :key="2"><span
                                            v-if="isMyProfile">My&nbsp;</span>Answers
                                    </v-tab>
                                <!--</v-tabs-bar>-->
                            </v-tabs>
                        </div>
                    </div>
                </div>

                <v-flex class="web-content">
                    <div class="empty-state" v-if="!questions.length">
                        <div class="text-block">
                            <p v-html="emptyStateData.text"></p>
                            <b>{{emptyStateData.boldText}}</b>
                        </div>
                        <router-link class="ask-question" :to="{name: emptyStateData.btnUrl}">{{emptyStateData.btnText}}</router-link>
                    </div>
                    <div v-if="activeTab === 1">
                        <router-link class="question-card-wrapper" :to="{name:'question',params:{id:questionData.id}}"
                                     v-for="(questionData,index) in questions" :key="index">
                            <question-card :cardData="questionData"></question-card>
                        </router-link>
                    </div>
                    <div v-else-if="activeTab === 2">
                        <router-link :to="{name:'question',params:{id:answerData.id}}" v-for="(answerData,index) in myAnswers"
                                     :key="index" class="mb-3">
                            <question-card :cardData="answerData" class="mb-3"></question-card>
                        </router-link>
                    </div>
                </v-flex>

            </v-layout>
        </div>
    </div>
</template>

<style src="./profile.less" lang="less"></style>
<script src="./profile.js"></script>
