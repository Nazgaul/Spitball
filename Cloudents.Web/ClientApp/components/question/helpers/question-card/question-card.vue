<template>
    <!-- ********************** THIS IS THE ANSWER CARD ********************** -->
    <v-flex v-if="cardData && !isDeleted" class="question-card">
        <div class="question-card-answer">
            <!-- answer Card -->
            <div class="full-width-flex">
                <div>
                    <user-block class="q-user-block" :cardData="cardData" :user="cardData.user"></user-block>
                </div>

                <div class="full-width-flex">
                    <div class="full-width-flex calc-Margin answer-block">
                        <div class="triangle"></div>
                        <div class="text-container">
                            <div class="text">
                                <div class="answer-header-left-container">
                                    <!-- <span class="user-date" v-language:inner>questionCard_Answer</span> -->
                                    <span class="timeago ml-2" :datetime="cardData.dateTime||cardData.create">{{date}}</span>
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
                </div>
            </div>
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
