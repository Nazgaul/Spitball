<template>
    <!-- ********************** THIS IS THE ANSWER CARD ********************** -->
    <v-flex
            v-if="cardData && !isDeleted "
            class="question-card"
            :class="[{'highlight':flaggedAsCorrect}]"
    >
        <div class="question-card-answer transparent">
            <!-- answer Card -->
            <div class="full-width-flex">
                <div>
                    <user-block class="q-user-block" :cardData="cardData" :user="cardData.user"></user-block>
                </div>

                <div class="full-width-flex" :class="{'column-direction': gallery && gallery.length}">
                    <div class="full-width-flex calc-Margin answer-block">
                        <div class="triangle"></div>
                        <div class="text-container">
                            <div class="text">
                                <div class="answer-header-left-container">
                                    <span class="user-date" v-language:inner>questionCard_Answer</span>
                                    <span class="timeago ml-2" :datetime="cardData.dateTime||cardData.create">{{date}}</span>
                                    <span v-if="typeAnswer" class="q-answer">
                                        <button
                                                class="accept-btn right"
                                                @click="markAsCorrect"
                                                v-if="showApproveButton && !flaggedAsCorrect && !hasAnswer">
                                            <v-icon>sbf-check-circle</v-icon>
                                        <span v-language:inner>questionCard_Accept</span>
                                        </button>
                                    </span>                                    
                                    <span class="choosen-answer right" v-if="flaggedAsCorrect">
                                        <v-icon>sbf-check-circle</v-icon>
                                    </span>
                                </div>
                                <v-spacer></v-spacer>
                                <div class="menu-area">
                                    <v-menu bottom left content-class="card-user-actions">
                                        <v-btn :depressed="true" @click.prevent slot="activator" icon>
                                            <v-icon>sbf-3-dot</v-icon>
                                        </v-btn>
                                        <v-list>
                                            <v-list-tile
                                                    v-show="item.isVisible"
                                                    :disabled="item.isDisabled()"
                                                    v-for="(item, i) in actions"
                                                    :key="i"
                                            >
                                                <v-list-tile-title @click="item.action()">{{ item.title }}
                                                </v-list-tile-title>
                                            </v-list-tile>
                                        </v-list>
                                    </v-menu>
                                </div>
                            </div>

                            <p
                                    class="q-text"
                                    :class="[`align-switch-${cardData.isRtl ? isRtl ? 'l' : 'r' : isRtl ? 'r' : 'l'}`, {'answer': typeAnswer}]"
                            >{{cardData.text}}</p>
                        </div>
                    </div>
                    <div class="gallery fixed-margin" v-if="gallery && gallery.length">
                        <v-carousel
                                :prev-icon="isRtl ? 'sbf-arrow-right rigth' : 'sbf-arrow-right left'"
                                :next-icon="isRtl ?  'sbf-arrow-right left': 'sbf-arrow-right right'"
                                interval="600000"
                                cycle
                                full-screen
                                hide-delimiters
                                :hide-controls="gallery.length===1"
                        >
                            <v-carousel-item
                                    v-for="(item,i) in gallery"
                                    v-bind:src="item"
                                    :key="i"
                                    @click.native="showBigImage(item)"
                            ></v-carousel-item>
                        </v-carousel>
                    </div>
                </div>
            </div>
            <v-dialog
                    v-if="showDialog"
                    v-model="showDialog"
                    max-width="720px"
                    transition="scale-transition"
                    content-class="zoom-image"
            >
                <img :src="selectedImage" alt height="auto" width="100%" class="zoomed-image">
            </v-dialog>
        </div>
        <sb-dialog
                :showDialog="showReport"
                :maxWidth="'438px'"
                :popUpType="'reportDialog'"
                :content-class="`reportDialog ${isRtl? 'rtl': ''}` "
        >
            <report-item
                    :closeReport="closeReportDialog"
                    :itemType="'answer'"
                    :answerDelData="answerToDeletObj"
                    :itemId="itemId"
            ></report-item>
        </sb-dialog>
    </v-flex>
</template>
<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
