<template>
    <v-container class="profile-page-container"> 
        <button class="back-button hidden-sm-and-up" @click="$router.go(-1)">
            <v-icon :class="{'rtl-icon': isRtl}" right>sbf-arrow-back</v-icon>
        </button>
        <v-layout  justify-start v-bind="xsColumn" class="bio-wrap" >
                <v-flex xs12 sm9  >
                    <profile-bio :isMyProfile="isMyProfile"></profile-bio>
                </v-flex>

            <!--TODO HIDDEN FOR NOW-->
            <v-flex  xs12 sm3  :class="{'pl-4': $vuetify.breakpoint.smAndUp}" v-if="isMyProfile || isTutorProfile">
                <tutorInfoBlock v-if="isTutorProfile && !isMyProfile"></tutorInfoBlock>
                <userInfoBlock v-else-if="!isTutorProfile"></userInfoBlock>
            </v-flex>
        </v-layout>
                <v-layout v-bind="xsColumn" align-start  justify-start>
                    <v-flex xs12  sm9 :class="[isMyProfile && isTutorProfile ? '' : ''  ]">
                        <v-flex xs12 sm12  class="mt-3  limited-760" :class="[$vuetify.breakpoint.xsOnly ? 'mb-2' : 'mb-4']">
                            <v-divider v-if="$vuetify.breakpoint.xsOnly" style="height:2px; color: rgba(163, 160, 251, 0.32);"></v-divider>
                            <v-tabs :dir="isRtl && $vuetify.breakpoint.xsOnly ? `ltr` : isRtl? 'rtl' : ''" class="tab-padding" hide-slider xs12>
                                
                                <v-tab @click="activeTab = 1" :id="`tab-${1}`" :href="'#tab-1'" :key="1">
                                    <span v-language:inner="'profile_about'"/>
                                </v-tab>

                                <v-tab @click="activeTab = 2" :id="`tab-${2}`" :href="'#tab-2'" :key="2">
                                    <span v-language:inner="'profile_Questions'"/>
                                </v-tab>

                                <v-tab @click="activeTab = 3" :id="`tab-${3}`" :href="'#tab-3'" :key="3">
                                    <span v-language:inner="'profile_Answers'"/>
                                </v-tab>

                                <v-tab @click="activeTab = 4" :id="`tab-${4}`" :href="'#tab-4'" :key="4">
                                    <span v-language:inner="'profile_documents'"/>
                                </v-tab>

                                <v-tab @click="activeTab = 5" :id="`tab-${5}`" :href="'#tab-5'" :key="5">
                                    <span v-language:inner="'profile_purchased_documents'"/>
                                </v-tab>

                                <v-tab @click="openCalendar" :id="`tab-${6}`" :href="'#tab-6'" :key="6" v-if="showCalendar">
                                    <span v-language:inner="'profile_calendar'"/>
                                </v-tab>


                            </v-tabs>
                            <v-divider style="height:2px; color: rgba(163, 160, 251, 0.32);"></v-divider>

                        </v-flex>
                        <v-flex class="web-content">
                            <div class="empty-state"
                                 v-if="activeTab === 1 && isEmptyCourses && isMyProfile && !isTutorProfile">
                                <courseEmptyState></courseEmptyState>
                            </div>

                            <div class="empty-state"
                                 v-if="activeTab === 2 && !questionDocuments.length && !loadingContent">
                                <div class="text-block">
                                    <p v-html="emptyStateData.text"></p>
                                    <b>{{emptyStateData.boldText}}</b>
                                </div>
                                <a class="ask-question" @click="emptyStateData.btnUrl()">{{emptyStateData.btnText}}</a>
                            </div>
                            <div class="empty-state"
                                 v-else-if="activeTab === 3 && !answerDocuments.length && !loadingContent">
                                <div class="text-block">
                                    <p v-html="emptyStateData.text"></p>
                                    <b>{{emptyStateData.boldText}}</b>
                                </div>
                                <router-link class="ask-question" :to="{name: emptyStateData.btnUrl}">
                                    {{emptyStateData.btnText}}
                                </router-link>
                            </div>
                            <div class="empty-state doc-empty-state"
                                 v-if="activeTab === 4 && !uploadedDocuments.length && !loadingContent">
                                <div class="text-block">
                                    <p v-html="emptyStateData.text"></p>
                                    <b>{{emptyStateData.boldText}}</b>
                                </div>
                                <div class="upload-btn-wrap">
                                    <upload-document-btn></upload-document-btn>
                                </div>

                            </div>
                            <div v-if="activeTab === 1" style="max-width: 760px;">
                                <tutorAboutMe v-if="isTutorProfile" :isMyProfile="isMyProfile"></tutorAboutMe>
                                <coursesCard :isMyProfile="isMyProfile" v-if="!isEmptyCourses"></coursesCard>
                               
                                <!--TODO HIDDEN FOR NOW-->
                                <ctaBlock v-if="$vuetify.breakpoint.smAndUp"></ctaBlock>
                                <reviewsList v-if="isTutorProfile"></reviewsList>
                            </div>
                            <scroll-list v-if="activeTab === 2" :scrollFunc="loadQuestions" :isLoading="questions.isLoading"
                                         :isComplete="questions.isComplete">
                                <!-- <router-link class="question-card-wrapper"
                                             :to="{name:'question',params:{id:questionData.id}}"
                                             v-for="(questionData,index) in profileData.questions" :key="index"> -->
                                             <div class="mb-3"  v-for="(questionData,index) in questionDocuments" :key="index">
                                    <question-card :cardData="questionData"></question-card>
                                    </div>

                                <!-- </router-link> -->
                            </scroll-list>
                            <scroll-list v-if="activeTab === 3" :scrollFunc="loadAnswers" :isLoading="answers.isLoading"
                                         :isComplete="answers.isComplete">
                                <div 
                                             v-for="(answerData,index) in answerDocuments"
                                             :key="index" class="mb-3">
                                    <question-card :cardData="answerData"></question-card>
                                </div>
                            </scroll-list>
                            <scroll-list v-if="activeTab === 4" :scrollFunc="loadDocuments" :isLoading="documents.isLoading"
                                         :isComplete="documents.isComplete">
                                <div 
                                             v-for="(document ,index) in uploadedDocuments"
                                             :key="index" class="mb-3">
                                    <result-note :item="document" class="pa-3 "></result-note>
                                </div>
                            </scroll-list>
                            <scroll-list v-if="activeTab === 5" :scrollFunc="loadPurchasedDocuments"
                                         :isLoading="purchasedDocuments.isLoading"
                                         :isComplete="purchasedDocuments.isComplete">
                                <div 
                                             v-for="(document ,index) in purchasedsDocuments"
                                             :key="index" class="mb-3">
                                    <result-note :item="document" class="pa-3 "></result-note>
                                </div>
                            </scroll-list>
                            <scroll-list v-if="activeTab === 6" :scrollFunc="(()=>{})"
                                         :isLoading="calendar.isLoading"
                                         :isComplete="calendar.isComplete">
                                <div class="mb-3">
                                    <calendarTab></calendarTab>
                                </div>
                            </scroll-list>                            
                        </v-flex>
                    </v-flex>
                    <!--TODO HIDDEN FOR NOW-->
                    <v-flex sm3 xs12 v-if="isMyProfile || isTutorProfile">
                        <v-spacer></v-spacer>
                    </v-flex>
                </v-layout>
    </v-container>
</template>

<script src="./new_profile.js"></script>

<style lang="less" src="./new_profile.less">


</style>