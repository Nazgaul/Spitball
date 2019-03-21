<template>
    <v-container class="profile-page-container">
        <v-layout align-start justify-center v-bind="xsColumn" class="bio-wrap" >
                <v-flex xs12 sm10 md10 class="mr-2">
                    <profile-bio></profile-bio>
                </v-flex>
            <v-flex  xs12 sm2 md2>
                <tutorInfoBlock></tutorInfoBlock>
                <userInfoBlock></userInfoBlock>
            </v-flex>
        </v-layout>
                <v-layout v-bind="xsColumn" align-start justify-start>
                    <v-flex xs12 md10 sm10 class="mr-5">
                        <v-flex xs12 sm12 md12 class="mt-3 mb-4" >
                            <v-tabs :dir="isRtl ? `ltr` : ''" class="tab-padding" hide-slider xs12>
                                <v-tab @click="activeTab = 1" :href="'#tab-1'" :key="1"><span
                                        v-language:inner>About</span>
                                </v-tab>


                                <v-tab @click="activeTab = 2" :href="'#tab-2'" :key="2"><span
                                        v-language:inner>profile_Questions</span>
                                </v-tab>
                                <v-tab @click="activeTab = 3" :href="'#tab-3'" :key="3"><span
                                        v-language:inner>profile_Answers</span>
                                </v-tab>
                                <v-tab @click="activeTab = 4" :href="'#tab-4'" :key="4"><span
                                        v-language:inner>profile_documents</span>
                                </v-tab>
                                <v-tab @click="activeTab = 5" :href="'#tab-5'" :key="5"><span v-language:inner>profile_purchased_documents</span>
                                </v-tab>
                            </v-tabs>
                            <v-divider style="height:2px; color: rgba(163, 160, 251, 0.32);"></v-divider>

                        </v-flex>
                        <v-flex class="web-content">
                            <div class="empty-state"
                                 v-if="activeTab === 2 && !profileData.questions.length && !loadingContent">
                                <div class="text-block">
                                    <p v-html="emptyStateData.text"></p>
                                    <b>{{emptyStateData.boldText}}</b>
                                </div>
                                <a class="ask-question" @click="emptyStateData.btnUrl()">{{emptyStateData.btnText}}</a>
                            </div>
                            <div class="empty-state"
                                 v-else-if="activeTab === 3 && !profileData.answers.length && !loadingContent">
                                <div class="text-block">
                                    <p v-html="emptyStateData.text"></p>
                                    <b>{{emptyStateData.boldText}}</b>
                                </div>
                                <router-link class="ask-question" :to="{name: emptyStateData.btnUrl}">
                                    {{emptyStateData.btnText}}
                                </router-link>
                            </div>
                            <div class="empty-state doc-empty-state"
                                 v-if="activeTab === 4 && !profileData.documents.length && !loadingContent">
                                <div class="text-block">
                                    <p v-html="emptyStateData.text"></p>
                                    <b>{{emptyStateData.boldText}}</b>
                                </div>
                                <div class="upload-btn-wrap">
                                    <upload-document-btn></upload-document-btn>
                                </div>

                            </div>
                            <div v-if="activeTab === 1" style="max-width: 760px;">
                                <tutorAboutMe></tutorAboutMe>
                                <coursesCard></coursesCard>
                                <subjectsCard></subjectsCard>
                                <ctaBlock></ctaBlock>
                                <reviewsList></reviewsList>
                            </div>
                            <scroll-list v-if="activeTab === 2" :scrollFunc="loadQuestions" :isLoading="questions.isLoading"
                                         :isComplete="questions.isComplete">
                                <router-link class="question-card-wrapper"
                                             :to="{name:'question',params:{id:questionData.id}}"
                                             v-for="(questionData,index) in profileData.questions" :key="index">
                                    <question-card :cardData="questionData"></question-card>
                                </router-link>
                            </scroll-list>
                            <scroll-list v-if="activeTab === 3" :scrollFunc="loadAnswers" :isLoading="answers.isLoading"
                                         :isComplete="answers.isComplete">
                                <router-link :to="{name:'question',params:{id:answerData.id}}"
                                             v-for="(answerData,index) in profileData.answers"
                                             :key="index" class="mb-3">
                                    <question-card :cardData="answerData" class="mb-3"></question-card>
                                </router-link>
                            </scroll-list>
                            <scroll-list v-if="activeTab === 4" :scrollFunc="loadDocuments" :isLoading="documents.isLoading"
                                         :isComplete="documents.isComplete">
                                <router-link :to="{name:'document',params:{id:document.id}}"
                                             v-for="(document ,index) in profileData.documents"
                                             :key="index" class="mb-3">
                                    <result-note style="padding: 16px;" :item="document" class="mb-3"></result-note>
                                </router-link>
                            </scroll-list>
                            <scroll-list v-if="activeTab === 5" :scrollFunc="loadPurchasedDocuments"
                                         :isLoading="purchasedDocuments.isLoading"
                                         :isComplete="purchasedDocuments.isComplete">
                                <router-link :to="{name:'document',params:{id:document.id}}"
                                             v-for="(document ,index) in profileData.purchasedDocuments"
                                             :key="index" class="mb-3">
                                    <result-note style="padding: 16px;" :item="document" class="mb-3"></result-note>
                                </router-link>
                            </scroll-list>
                        </v-flex>
                    </v-flex>

                    <v-flex sm2 md2 xs12>
                        <v-spacer></v-spacer>
                    </v-flex>
                </v-layout>
    </v-container>
</template>

<script src="./new_profile.js"></script>

<style lang="less" src="./new_profile.less">


</style>