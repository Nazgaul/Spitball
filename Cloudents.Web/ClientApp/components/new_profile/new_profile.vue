<template>
    <v-container class="profile-page-container">
        <v-layout align-start justify-center>
            <v-flex xs19 sm19 md19>
                <v-flex xs12 sm12 md12>
                    <profile-bio></profile-bio>
                </v-flex>
                <v-layout column>
                    <v-flex>
                        <v-tabs :dir="isRtl ? `ltr` : ''" class="tab-padding" hide-slider xs12>

                            <v-tab @click="activeTab = 1" :href="'#tab-1'" :key="1"><span
                                    v-language:inner>profile_Questions</span>
                            </v-tab>
                            <v-tab @click="activeTab = 2" :href="'#tab-2'" :key="2"><span
                                    v-language:inner>profile_Answers</span>
                            </v-tab>
                            <v-tab @click="activeTab = 3" :href="'#tab-3'" :key="3"><span
                                    v-language:inner>profile_documents</span>
                            </v-tab>
                            <v-tab @click="activeTab = 4" :href="'#tab-4'" :key="4"><span v-language:inner>profile_purchased_documents</span>
                            </v-tab>
                            <v-tab @click="activeTab = 5" :href="'#tab-5'" :key="5"><span
                                    v-language:inner>About</span>
                            </v-tab>
                        </v-tabs>
                        <v-divider style="height:2px; color: rgba(163, 160, 251, 0.32);"></v-divider>

                    </v-flex>
                    <v-flex class="web-content">
                        <div class="empty-state"
                             v-if="activeTab === 1 && !profileData.questions.length && !loadingContent">
                            <div class="text-block">
                                <p v-html="emptyStateData.text"></p>
                                <b>{{emptyStateData.boldText}}</b>
                            </div>
                            <a class="ask-question" @click="emptyStateData.btnUrl()">{{emptyStateData.btnText}}</a>
                        </div>
                        <div class="empty-state"
                             v-else-if="activeTab === 2 && !profileData.answers.length && !loadingContent">
                            <div class="text-block">
                                <p v-html="emptyStateData.text"></p>
                                <b>{{emptyStateData.boldText}}</b>
                            </div>
                            <router-link class="ask-question" :to="{name: emptyStateData.btnUrl}">
                                {{emptyStateData.btnText}}
                            </router-link>
                        </div>
                        <div class="empty-state doc-empty-state"
                             v-if="activeTab === 3 && !profileData.documents.length && !loadingContent">
                            <div class="text-block">
                                <p v-html="emptyStateData.text"></p>
                                <b>{{emptyStateData.boldText}}</b>
                            </div>
                            <div class="upload-btn-wrap">
                                <upload-document-btn></upload-document-btn>
                            </div>

                        </div>
                        <scroll-list v-if="activeTab === 1" :scrollFunc="loadQuestions" :isLoading="questions.isLoading"
                                     :isComplete="questions.isComplete">
                            <router-link class="question-card-wrapper"
                                         :to="{name:'question',params:{id:questionData.id}}"
                                         v-for="(questionData,index) in profileData.questions" :key="index">
                                <question-card :cardData="questionData"></question-card>
                            </router-link>
                        </scroll-list>
                        <scroll-list v-if="activeTab === 2" :scrollFunc="loadAnswers" :isLoading="answers.isLoading"
                                     :isComplete="answers.isComplete">
                            <router-link :to="{name:'question',params:{id:answerData.id}}"
                                         v-for="(answerData,index) in profileData.answers"
                                         :key="index" class="mb-3">
                                <question-card :cardData="answerData" class="mb-3"></question-card>
                            </router-link>
                        </scroll-list>
                        <scroll-list v-if="activeTab === 3" :scrollFunc="loadDocuments" :isLoading="documents.isLoading"
                                     :isComplete="documents.isComplete">
                            <router-link :to="{name:'document',params:{id:document.id}}"
                                         v-for="(document ,index) in profileData.documents"
                                         :key="index" class="mb-3">
                                <result-note style="padding: 16px;" :item="document" class="mb-3"></result-note>
                            </router-link>
                        </scroll-list>
                        <scroll-list v-if="activeTab === 4" :scrollFunc="loadPurchasedDocuments"
                                     :isLoading="purchasedDocuments.isLoading"
                                     :isComplete="purchasedDocuments.isComplete">
                            <router-link :to="{name:'document',params:{id:document.id}}"
                                         v-for="(document ,index) in profileData.purchasedDocuments"
                                         :key="index" class="mb-3">
                                <result-note style="padding: 16px;" :item="document" class="mb-3"></result-note>
                            </router-link>
                        </scroll-list>
                        <div v-if="activeTab === 5">
                            <tutorAboutMe></tutorAboutMe>
                            <coursesCard></coursesCard>
                            <subjectsCard></subjectsCard>
                            <reviewsList></reviewsList>
                        </div>
                    </v-flex>
                </v-layout>
            </v-flex>
            <v-flex xs2 md3 sm3 class="ml-5" style="max-width: 260px;">
                <tutorInfoBlock></tutorInfoBlock>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>

    //old
    import questionCard from "../question/helpers/new-question-card/new-question-card.vue";
    import resultNote from "../results/ResultNote.vue"
    import userBlock from '../helpers/user-block/user-block.vue';
    import { mapActions, mapGetters } from 'vuex'
    import { LanguageService } from "../../services/language/languageService";
    import uploadDocumentBtn from "../results/helpers/uploadFilesBtn/uploadFilesBtn.vue"
    //old
    //new
    import profileBio from './profileHelpers/profileBio/profileBio.vue';
    import tutorAboutMe from './profileHelpers/profileAbout/tutorAboutMe.vue';
    import coursesCard from './profileHelpers/coursesCard/coursesCard.vue';
    import subjectsCard from './profileHelpers/subjectsCard/subjectsCard.vue';
    import reviewsList from './profileHelpers/reviews/reviewsList.vue';
    import tutorInfoBlock from './profileHelpers/tutoringInfo/tutorInfoBlock.vue'


    //new
    export default {
        name: "new_profile",
        components: {
            questionCard,
            userBlock,
            resultNote,
            uploadDocumentBtn,
            profileBio,
            tutorAboutMe,
            coursesCard,
            subjectsCard,
            reviewsList,
            tutorInfoBlock
        },
        props: {
            id: {
                Number
            }
        },
        data() {
            return {
                isRtl: global.isRtl,
                isEdgeRtl: global.isEdgeRtl,
                loadingContent: false,
                activeTab: 1,
                itemsPerTab: 50,
                answers: {
                    isLoading: false,
                    isComplete: false,
                    page: 1,
                },
                questions: {
                    isLoading: false,
                    isComplete: false,
                    page: 1,
                },
                documents: {
                    isLoading: false,
                    isComplete: false,
                    page: 1
                },
                purchasedDocuments: {
                    isLoading: false,
                    isComplete: false,
                    page: 1
                }
            }
        },
        methods: {
            ...mapActions([
                'updateNewQuestionDialogState',
                'syncProfile',
                'getAnswers',
                'getQuestions',
                'getDocuments',
                'resetProfileData',
                'getPurchasedDocuments',
                'setProfileByActiveTab'
            ]),

            changeActiveTab(tabId) {
                this.activeTab = tabId;
            },
            fetchData() {
                let syncObj = {
                    id: this.id,
                    activeTab: this.activeTab
                }
                this.syncProfile(syncObj);
            },
            getInfoByTab() {
                this.loadingContent = true;
                this.setProfileByActiveTab(this.activeTab).then(() => {
                    this.loadingContent = false;
                })
            },
            loadAnswers() {
                if (this.profileData.answers.length < this.itemsPerTab) {
                    this.answers.isComplete = true;
                    return;
                }
                this.answers.isLoading = true;
                let AnswersInfo = {
                    id: this.id,
                    page: this.answers.page
                }
                this.getAnswers(AnswersInfo).then((hasData) => {
                    if (!hasData) {
                        this.answers.isComplete = true;
                    }
                    this.answers.isLoading = false;
                    this.answers.page++;
                }, (err) => {
                    this.answers.isComplete = true;
                })
            },
            loadQuestions() {
                if (this.profileData.questions.length < this.itemsPerTab) {
                    this.questions.isComplete = true;
                    return;
                }
                this.questions.isLoading = true;
                let QuestionsInfo = {
                    id: this.id,
                    page: this.questions.page,
                    user: this.profileData.user
                }
                this.getQuestions(QuestionsInfo).then((hasData) => {
                    if (!hasData) {
                        this.questions.isComplete = true;
                    }
                    this.questions.isLoading = false;
                    this.questions.page++;
                }, (err) => {
                    this.questions.isComplete = true;
                })
            },
            loadDocuments() {
                if (this.profileData.documents.length < this.itemsPerTab) {
                    this.documents.isComplete = true;
                    return;
                }
                this.documents.isLoading = true;
                let DocumentsInfo = {
                    id: this.id,
                    page: this.documents.page,
                    user: this.profileData.user
                }
                this.getDocuments(DocumentsInfo).then((hasData) => {
                    if (!hasData) {
                        this.documents.isComplete = true;
                    }
                    this.documents.isLoading = false;
                    this.documents.page++;
                }, (err) => {
                    this.documents.isComplete = true;
                })
            },
            loadPurchasedDocuments() {
                if (this.profileData.purchasedDocuments.length < this.itemsPerTab) {
                    this.purchasedDocuments.isComplete = true;
                    return;
                }
                this.purchasedDocuments.isLoading = true;
                let DocumentsInfo = {
                    id: this.id,
                    page: this.purchasedDocuments.page,
                    user: this.profileData.user
                }
                this.getPurchasedDocuments(DocumentsInfo).then((hasData) => {
                    if (!hasData) {
                        this.purchasedDocuments.isComplete = true;
                    }
                    this.purchasedDocuments.isLoading = false;
                    this.purchasedDocuments.page++;
                }, (err) => {
                    this.purchasedDocuments.isComplete = true;
                })
            }
        },
        computed: {
            ...mapGetters(["accountUser", "getProfile"]),
            profileData() {
                if (!!this.getProfile) {
                    return this.getProfile
                }
            },
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly;
            },
            isMyProfile() {
                if (!!this.profileData) {
                    return false  // return this.accountUser && this.accountUser.id && this.profileData ? this.profileData.user.id == this.accountUser.id : false;
                }
            },
            emptyStateData() {
                let questions = {
                    text: LanguageService.getValueByKey("profile_emptyState_questions_text"),
                    boldText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                    btnText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                    btnUrl: () => {
                        let Obj = {
                            status: true,
                            from: 5
                        };
                        this.updateNewQuestionDialogState(Obj)
                    }
                };
                let answers = {
                    text: LanguageService.getValueByKey("profile_emptyState_answers_text"),
                    btnText: LanguageService.getValueByKey("profile_emptyState_answers_btnText"),
                    btnUrl: 'ask'
                };
                let documents = {
                    text: LanguageService.getValueByKey("profile_emptyState_documents_text"),
                    //TODO feel free to remove this after redesign, will not be used, using reusable component instead
                    btnText: LanguageService.getValueByKey("profile_emptyState_documents_btnText"),
                    btnUrl: 'note'
                };
                if (this.activeTab === 1) {
                    return questions
                } else if (this.activeTab === 2) {
                    return answers
                } else if (this.activeTab === 3) {
                    return documents
                }
            }
        },
        watch: {
            '$route': 'fetchData',
            activeTab() {
                this.getInfoByTab();
            }
        },
        //reset profile data to prevent glitch in profile loading
        beforeRouteLeave(to, from, next) {
            this.resetProfileData();
            next()
        },
        created() {
            this.fetchData();
        }
    }

</script>

<style lang="less" src="./new_profile.less">


</style>