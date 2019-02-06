<template>
    <div class="profile-page" id="profilePageContainer" v-if="profileData">
        <div>
            <v-layout class="data-wrapper" :class="{'column': isMobile}" wrap justify-start>
                <div>
                    <div :class="['main-block',  {'position-static': isEdgeRtl}]">
                        <button class="back" :class="{'heb': isRtl}" @click="$router.go(-1)">
                            <v-icon>sbf-arrow-right</v-icon>
                        </button>
                        <user-block v-if="profileData && profileData.user" :user="profileData.user"
                                    :classType="'university'" :showExtended="true"></user-block>
                        <div class="menu">
                            <ul class="tabs" v-if="!isMobile" xs3>
                                <li :class="{'active': activeTab === 1}" @click="changeActiveTab(1)">
                                    <!-- <span v-if="isMyProfile"></span> -->
                                    <span v-language:inner>profile_Questions</span> 
                                </li>
                                <li :class="{'active': activeTab === 2}" @click="changeActiveTab(2)">
                                    <!-- <span v-if="isMyProfile"></span> -->
                                    <span v-language:inner>profile_Answers</span> 
                                </li>
                                <li :class="{'active': activeTab === 3}" @click="changeActiveTab(3)">
                                    <!-- <span v-if="isMyProfile"></span> -->
                                    <span v-language:inner>profile_documents</span>
                                </li>
                                <li :class="{'active': activeTab === 4}" @click="changeActiveTab(4)">
                                    <!-- <span v-if="isMyProfile"></span> -->
                                    <span v-language:inner>profile_purchased_documents</span>
                                </li>
                            </ul>
                            <v-tabs v-else :dir="isRtl ? `ltr` : ''" grow class="tab-padding" xs12>
                                <v-tabs-slider color="blue"></v-tabs-slider>
                                <v-tab @click="activeTab = 1" :href="'#tab-1'" :key="1"><span v-language:inner>profile_Questions</span>
                                </v-tab>
                                <v-tab @click="activeTab = 2" :href="'#tab-2'" :key="2"><span v-language:inner>profile_Answers</span>
                                </v-tab>
                                <v-tab @click="activeTab = 3" :href="'#tab-3'" :key="3"><span v-language:inner>profile_documents</span>
                                </v-tab>
                                <v-tab @click="activeTab = 4" :href="'#tab-4'" :key="4"><span v-language:inner>profile_purchased_documents</span>
                                </v-tab>
                            </v-tabs>
                        </div>
                    </div>
                </div>
                <v-flex class="web-content">
                    <div class="empty-state" v-if="activeTab === 1 && !profileData.questions.length && !loadingContent">
                        <div class="text-block">
                            <p v-html="emptyStateData.text"></p>
                            <b>{{emptyStateData.boldText}}</b>
                        </div>
                        <a class="ask-question" @click="emptyStateData.btnUrl()">{{emptyStateData.btnText}}</a>
                    </div>
                    <div class="empty-state" v-else-if="activeTab === 2 && !profileData.answers.length && !loadingContent">
                        <div class="text-block">
                            <p v-html="emptyStateData.text"></p>
                            <b>{{emptyStateData.boldText}}</b>
                        </div>
                        <router-link class="ask-question" :to="{name: emptyStateData.btnUrl}">{{emptyStateData.btnText}}</router-link>
                    </div>
                    <div class="empty-state doc-empty-state" v-if="activeTab === 3 && !profileData.documents.length && !loadingContent">
                        <div class="text-block">
                            <p v-html="emptyStateData.text"></p>
                            <b>{{emptyStateData.boldText}}</b>
                        </div> 
                        <div class="upload-btn-wrap">
                              <upload-document-btn></upload-document-btn>  
                        </div>               

                    </div>
                        <scroll-list v-if="activeTab === 1" :scrollFunc="loadQuestions" :isLoading="questions.isLoading" :isComplete="questions.isComplete">
                            <router-link class="question-card-wrapper" :to="{name:'question',params:{id:questionData.id}}"
                                        v-for="(questionData,index) in profileData.questions" :key="index">
                                <question-card :cardData="questionData"></question-card>
                            </router-link>
                        </scroll-list>
                        <scroll-list v-if="activeTab === 2" :scrollFunc="loadAnswers" :isLoading="answers.isLoading" :isComplete="answers.isComplete">
                            <router-link :to="{name:'question',params:{id:answerData.id}}" v-for="(answerData,index) in profileData.answers"
                                        :key="index" class="mb-3">
                                <question-card :cardData="answerData" class="mb-3"></question-card>
                            </router-link>
                        </scroll-list>
                    <scroll-list v-if="activeTab === 3" :scrollFunc="loadDocuments" :isLoading="documents.isLoading" :isComplete="documents.isComplete">
                        <router-link :to="{name:'document',params:{id:document.id}}" v-for="(document ,index) in profileData.documents"
                                     :key="index" class="mb-3">
                            <result-note style="padding: 16px;" :item="document" class="mb-3"></result-note>
                        </router-link>
                    </scroll-list>
                    <scroll-list v-if="activeTab === 4" :scrollFunc="loadPurchasedDocuments" :isLoading="purchasedDocuments.isLoading" :isComplete="purchasedDocuments.isComplete">
                        <router-link :to="{name:'document',params:{id:document.id}}" v-for="(document ,index) in profileData.purchasedDocuments"
                                     :key="index" class="mb-3">
                            <result-note style="padding: 16px;" :item="document" class="mb-3"></result-note>
                        </router-link>
                    </scroll-list>
                </v-flex>

            </v-layout>
        </div>
    </div>
</template>

<style src="./profile.less" lang="less"></style>
<script src="./profile.js"></script>
