<template>
    <div class="question-container">
        <router-link :to="{path:'/question/'+cardData.id}"
                     :class="[ cursorDefault ? 'cursor-text cursor-modified': '']">
            <div class="question-header-container">
                <div class="question-header-large-sagment">
                    <div class="avatar-area">
                        <user-avatar
                                :user-name="cardData.user.name"
                                :userImageUrl="userImageUrl"
                                :user-id="cardData.user.id"/>
                    </div>
                    <div class="rank-date-container">
                        <div class="rank-area">
                            <user-rank
                                    :score="cardData && cardData.user && cardData.user.score ? cardData.user.score : ''"></user-rank>
                        </div>
                        <div class="date-area">{{uploadDate}}</div>
                    </div>
                </div>
                <div class="question-header-small-sagment">
                    <!--TODO SOLD IS ACCEPTED-->
                    <div class="price-area" :class="{'sold': isSold}">
                        <v-icon class="has-correct">sbf-check-circle</v-icon>
                    </div>
                    <div class="menu-area">
                        <v-menu lazy bottom left content-class="card-user-actions">
                            <v-btn :depressed="true" @click.prevent slot="activator" icon>
                                <v-icon>sbf-3-dot</v-icon>
                            </v-btn>
                            <v-list class="report-list">
                                <v-list-tile v-show="item.isVisible" :disabled="item.isDisabled()"
                                             class="report-list-item"
                                             v-for="(item, i) in actions" :key="i">
                                    <v-list-tile-content>
                                        <v-list-tile-title @click="item.action()">{{ item.title }}</v-list-tile-title>
                                    </v-list-tile-content>
                                </v-list-tile>
                            </v-list>
                        </v-menu>
                    </div>
                </div>
            </div>
            <div class="question-body-container">
                <div class="question-left-body-container">
          <span class="question-raputation upvote-arrow" @click.prevent="upvoteQuestion()">
            <v-icon :class="{'voted': cardData.upvoted}">sbf-arrow-up</v-icon>
          </span>
                    <span class="question-raputation question-score" :dir="isRtl ? `ltr` : ''">{{cardData.votes}}</span>
                    <span class="question-raputation downvote-arrow" @click.prevent="downvoteQuestion()">
            <v-icon :class="{'voted': cardData.downvoted}">sbf-arrow-down</v-icon>
          </span>
                </div>
                <div class="question-right-body-container">
                    <v-layout align-center justify-start class="question-body-header-container">
                        <div class="question-subject" v-line-clamp:18="'1'">{{cardData.course ? cardData.course : cardData.subject}}
                        </div>
                        <div v-show="!!cardData.course && !!cardData.subject" class="question-course"> 
                            <span class="dot"></span>  
                            <span v-line-clamp:18="'1'">{{cardData.subject}}
                            </span>
                        </div>
                    </v-layout>
                    <div
                            class="question-body-content-container"
                            :class="[`align-switch-${cardData.isRtl ? isRtl ? 'l' : 'r' : isRtl ? 'r' : 'l'}`]"
                    >
                        <span class="question-text" v-line-clamp:18="lineClampValue">{{cardData.text}}</span>
                    </div>
                    <div class="gallery" v-if="cardData.files && cardData.files.length">
                        <v-carousel :prev-icon="isRtl ? 'sbf-arrow-right rigth' : 'sbf-arrow-right left'"
                                    :next-icon="isRtl ?  'sbf-arrow-right left': 'sbf-arrow-right right'"
                                    interval="600000" cycle full-screen
                                    hide-delimiters :hide-controls="cardData.files.length===1">
                            <v-carousel-item v-for="(item,i) in cardData.files" v-bind:src="item" :key="i"
                                             @click.native="showBigImage(item)"></v-carousel-item>
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
            <div class="question-footer-container">
                <div class="answer-display-container">
                    <div>
                        <v-icon
                                v-for="(answer, index) in answersNumber"
                                :key="index"
                                class="answer-icon"
                        >sbf-comment-icon
                        </v-icon>
                    </div>
                    <div v-if="answersDeltaNumber > 0" class="answers-delta-number-container">
                        <span>{{answersDeltaNumber}}</span>
                    </div>
                    <div
                            v-if="cardData.watchingNow > 0 && !isSold"
                            class="answer-currently-watching-container"
                            :class="{'space': answersNumber > 0}"
                    >
                        <span v-language:inner>questionCard_Someone_answering</span>
                    </div>
                </div>
                <div class="answers-info-container">
                    <div v-if="cardData.filesNum > 0" class="answers-attachments-container">
                        <v-icon>sbf-attach</v-icon>
                        <span>{{cardData.filesNum}}</span>
                    </div>
                    <!-- <div class="answers-viewers-container">
                        <v-icon class>sbf-views</v-icon>
                        <span>{{randomViews}}</span>
                    </div> -->
                </div>
            </div>
            <div v-if="!isSold && !hideAnswerInput" class="question-bottom-section">
                <div class="question-input-container">
                    <input class="question-input" placeholder="questionCard_Answer_placeholder" v-language:placeholder
                           type="text">
                </div>
            </div>
        </router-link>
        <sb-dialog
                :showDialog="showReportReasons"
                :maxWidth="'438px'"
                :popUpType="'reportDialog'"
                :content-class="`reportDialog ${isRtl? 'rtl': ''}` ">
            <report-item :closeReport="closeReportDialog" :itemType="cardData.template" :itemId="itemId"></report-item>
        </sb-dialog>
    </div>
</template>

<script src="./new-question-card.js"></script>
<style lang="less" src="./new-question-card.less"></style>
