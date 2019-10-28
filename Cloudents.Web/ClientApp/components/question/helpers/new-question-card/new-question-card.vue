<template>
    <div class="question-container">
        <router-link :to="{path:'/question/'+cardData.id}">
            <div class="question-header-container">
                <div class="question-header-large-sagment">
                    <div class="rank-date-container">
                        <div class="date-area">{{uploadDate}}</div>
                    </div>
                </div>
                <div class="question-header-small-sagment">
                    <div class="price-area" :class="{'sold': isSold}">
                        <v-icon class="has-correct">sbf-check-circle</v-icon>
                    </div>
                    <div class="menu-area">


                        <v-menu class="menu-area" lazy bottom left content-class="card-user-actions">
                            <v-btn :depressed="true" @click.prevent slot="activator" icon>
                            <v-icon>sbf-3-dot</v-icon>
                            </v-btn>
                            <v-list>
                            <v-list-tile v-show="item.isVisible" class="report-list-item" :disabled="item.isDisabled()" v-for="(item, i) in actions" :key="i">
                                <v-list-tile-title style="cursor:pointer;" @click="item.action()">{{ item.title }}</v-list-tile-title>
                            </v-list-tile>
                            </v-list>
                        </v-menu>

                    </div>
                </div>
            </div>
            <div class="question-body-container">
                <div class="question-right-body-container">
                    <v-layout align-center justify-start class="question-body-header-container">
                        <div class="question-subject">{{cardData.course}}</div>
                        <div v-show="!!cardData.course && !!cardData.subject" class="question-course"> 
                            <span class="dot"></span>  
                            <span class="lineClamp">{{cardData.subject}}</span>
                        </div>
                    </v-layout>
                    <div class="question-body-content-container mt-2 mb-3" :class="[`align-switch-${cardData.isRtl ? isRtl ? 'l' : 'r' : isRtl ? 'r' : 'l'}`, {'question-ellipsis': $route.name === 'feed'}]">
                        <div class="question-text">{{cardData.text}}</div>
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
            <div class="question-footer-container">
                <div class="answer-display-container">
                    <div class="user_answer_wrap" v-if="answers">
                        <div class="user_answer_body mb-0">
                            <div class="d-flex mb-2 user_answer_aligment">
                                <user-avatar
                                    class="avatar-area mr-2" 
                                    :user-name="answers.user.name"
                                    :userImageUrl="answers.user.image"
                                    :user-id="answers.user.id"
                                />
                                <div class="user_answer_info">
                                    <div class="user_answer_info_name text-truncate">{{answers.user.name}}</div>
                                    <div class="user_answer_info_date text-truncate">{{uploadDateAnswer}}</div>
                                </div>
                            </div>
                        </div>
                        <div class="user_answer">{{answers.text}}</div>
                    </div>
                    <div v-if="cardData.answers > 1" class="more-answers mt-3" v-html="$Ph(moreAnswersDictionary, answersCount -1)"></div>
                    <div v-else class="more-answers mt-3"></div>
                </div>

                <div class="answers-info-container">
                    <div v-if="cardData.filesNum > 0" class="answers-attachments-container">
                        <v-icon>sbf-attach</v-icon>
                        <span>{{cardData.filesNum}}</span>
                    </div>
                </div>
            </div>

            <div v-if="!hideAnswerInput" class="question-bottom-section">
                <div class="question-input-container d-flex">
                    <user-avatar class="avatar-area mr-2" :user-name="accountUser.name" :userImageUrl="accountUser.image" :user-id="accountUser.id" v-if="accountUser" />
                    <user-avatar class="avatar-area mr-2" :user-name="'JD'" :userImageUrl="''" :user-id="''" v-else />
                    <input class="question-input" placeholder="questionCard_Answer_placeholder" v-language:placeholder type="text">
                </div>
            </div>

        </router-link>
        <sb-dialog
                :showDialog="showReportReasons"
                :maxWidth="'438px'"
                :popUpType="'reportDialog'"
                :content-class="`reportDialog ${isRtl? 'rtl': ''}` ">
            <report-item :closeReport="closeReportDialog" :itemType="'feed'" :itemId="itemId"></report-item>
        </sb-dialog>
    </div>
</template>

<script src="./new-question-card.js"></script>
<style lang="less" src="./new-question-card.less"></style>
