<template>
    <div class="question-container">
        <router-link draggable="false" :to="{path:'/question/'+cardData.id}" :style="{'cursor':isQuestionPage?'auto':'pointer'}">
            <div class="question-header-container">
                <div class="question-header-large-sagment">
                    <div class="rank-date-container">
                        <user-avatar class="mr-1" size="34" :userImageUrl="cardData.user.image" :user-name="cardData.user.name" :user-id="cardData.user.id"/>
                        <div class="user-question">
                            <div class="user-question-name body-2 text-truncate">{{cardData.user.name}}</div>
                            <div class="user-question-date">{{uploadDate}}</div>
                        </div>
                    </div>
                </div>
                <div class="question-header-small-sagment">
                    <div class="price-area" :class="{'sold': isSold}">
                        <v-icon class="has-correct">sbf-check-circle</v-icon>
                    </div>
                    <div class="menu-area">
                        <v-menu class="menu-area" bottom left content-class="card-user-actions">
                            <template v-slot:activator="{ on }">
                                <v-btn :depressed="true" @click.prevent icon v-on="on">
                                    <v-icon>sbf-3-dot</v-icon>
                                </v-btn>
                            </template>
                            <v-list>
                            <v-list-item v-show="item.isVisible" class="report-list-item" :disabled="item.isDisabled()" v-for="(item, i) in actions" :key="i">
                                <v-list-item-title style="cursor:pointer;" @click="item.action()">{{ item.title }}</v-list-item-title>
                            </v-list-item>
                            </v-list>
                        </v-menu>
                    </div>
                </div>
            </div>
            <div class="question-body-container" :class="{'ml-12': !$vuetify.breakpoint.xsOnly}">
                <div class="question-right-body-container">
                    <!-- <v-layout align-center justify-start class="question-body-header-container">
                        <div class="question-subject">{{cardData.course}}</div>
                        <div v-show="!!cardData.course && !!cardData.subject" class="question-course"> 
                            <span class="dot"></span>  
                            <span class="lineClamp">{{cardData.subject}}</span>
                        </div>
                    </v-layout> -->
                    <div class="question-body-content-container mt-2 mb-1" :class="{'question-ellipsis': $route.name === 'feed'}">
                        <div class="question-text body-2">{{cardData.text}}</div>
                    </div>
                    <div class="question-body-course-container" :class="[answers ? 'mb-4' : 'mb-0']">
                        <div class="question-body-course body-2 text-truncate" v-html="$Ph('resultNote_course',[cardData.course])"></div>
                        <!-- <div class="mr-1" v-language:inner="'resultTutor_courses'"></div> -->
                        <!-- <div class="question-subject text-truncate">{{cardData.course}}</div> -->
                    </div>
                    <div class="gallery" v-if="cardData.files && cardData.files.length">
                        <v-carousel 
                            :prev-icon="isRtl ? 'sbf-arrow-right rigth' : 'sbf-arrow-right left'"
                            :next-icon="isRtl ? 'sbf-arrow-right left': 'sbf-arrow-right right'"
                            interval="600000" 
                            cycle 
                            full-screen
                            hide-delimiters 
                            :hide-controls="cardData.files.length === 1">
                            <v-carousel-item v-for="(item,i) in cardData.files" v-bind:src="item" :key="i" @click.native="showBigImage(item)"></v-carousel-item>
                        </v-carousel>
                    </div>
                </div>
                <v-dialog v-if="showDialog"
                          v-model="showDialog"
                          max-width="720px"
                          transition="scale-transition"
                          content-class="zoom-image">
                    <img :src="selectedImage" alt="" height="auto" width="100%" class="zoomed-image">
                </v-dialog>
            </div>
            <div class="question-footer-container" :class="{'ml-12': !$vuetify.breakpoint.xsOnly}">
                <div class="answer-display-container">
                    <div class="user_answer_wrap" v-if="answers">
                        <div class="user_answer_body mb-1">
                            <div class="d-flex mb-2 user_answer_aligment">
                                <user-avatar
                                    class="avatar-area"
                                    :user-name="answers.user.name"
                                    :userImageUrl="answers.user.image"
                                    :user-id="answers.user.id"
                                />
                                <div class="user_answer_info">
                                    <div class="user_answer_info_name body-2 text-truncate">{{answers.user.name}}</div>
                                    <div class="user_answer_info_date text-truncate">{{uploadDateAnswer}}</div>
                                </div>
                            </div>
                        </div>
                        <div class="user_answer body-2">{{answers.text}}</div>
                    </div>
                    <div v-if="cardData.answers > 1" class="more-answers" v-html="$Ph(moreAnswersDictionary, answersCount -1)"></div>
                    <div v-else class="mt-4"></div>
                </div>

                <!-- <div class="answers-info-container">
                    <div v-if="cardData.filesNum > 0" class="answers-attachments-container">
                        <v-icon>sbf-attach</v-icon>
                        <span>{{cardData.filesNum}}</span>
                    </div>
                </div> -->
            </div>
            <div v-if="!hideAnswerInput" class="question-bottom-section" :class="[{'mx-12': !$vuetify.breakpoint.xsOnly}, answersCount > 1 ? 'mt-0' : 'mt-6']">
                <div class="question-input-container d-flex">
                    <user-avatar class="avatar-area mr-2" :user-name="accountUser.name" :userImageUrl="accountUser.image" :user-id="accountUser.id" v-if="accountUser" />
                    <user-avatar class="avatar-area mr-2" :user-name="'JD'" :userImageUrl="''" v-else />
                    <input class="question-input" placeholder="questionCard_Answer_placeholder" v-language:placeholder type="text">
                    <questionNote class="question-input-icon"/>
                </div>
            </div>

        </router-link>
        <sb-dialog
                :showDialog="showReportReasons"
                :maxWidth="'438px'"
                :popUpType="'reportDialog'"
                :content-class="`reportDialog ` ">
            <report-item :closeReport="closeReportDialog" :itemType="'Question'" :itemId="itemId"></report-item>
        </sb-dialog>
    </div>
</template>

<script src="./new-question-card.js"></script>
<style lang="less" src="./new-question-card.less"></style>
