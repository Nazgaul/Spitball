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
                                    <span class="choosen-answer right" v-if="flaggedAsCorrect">
                                        <v-icon>sbf-check-circle</v-icon>
                                    </span>
                                </div>
                                <v-spacer></v-spacer>
                                <div class="menu-area">
                                    <v-menu bottom left content-class="card-user-actions">
                                        <template v-slot:activator="{ on }">
                                            <v-btn :depressed="true" @click.prevent v-on="on" icon>
                                                <v-icon small>sbf-3-dot</v-icon>
                                            </v-btn>
                                        </template>
                                        <v-list>
                                            <v-list-item
                                                    v-show="item.isVisible"
                                                    :disabled="item.isDisabled()"
                                                    v-for="(item, i) in actions"
                                                    :key="i"
                                            >
                                                <v-list-item-title @click="item.action()">{{ item.title }}
                                                </v-list-item-title>
                                            </v-list-item>
                                        </v-list>
                                    </v-menu>
                                </div>
                            </div>
                            
                            <p :class="['q-text',{'answer': typeAnswer}]">{{cardData.text}}</p>
                        </div>
                    </div>
                    <!-- TODO CLEAN IT! -->
                    <div class="gallery fixed-margin" v-if="gallery && gallery.length">
                        <v-carousel
                                :prev-icon="'sbf-arrow-right'"
                                :next-icon="'sbf-arrow-right'"
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
                    <!-- TODO CLEAN IT! -->
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
                :content-class="`reportDialog`"
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
