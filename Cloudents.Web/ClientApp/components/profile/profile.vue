<template>
    <div class="profile-page" v-if="profileData">
        <v-container>
            <v-layout :class="{'column': isMobile}" wrap justify-start>

                <div>
                    <div class="main-block">
                        <button class="back">
                            <v-icon>sbf-arrow-right</v-icon>
                        </button>
                        <user-block v-if="profileData.user" :user="profileData.user"
                                    :classType="'university'"></user-block>

                        <div class="menu">
                            <ul class="tabs" v-if="!isMobile" xs3>
                                <li :class="{'active': activeTab === 1}" @click="activeTab = 1">Selling</li>
                                <li :class="{'active': activeTab === 2}" @click="activeTab = 2">Sold</li>
                                <li :class="{'active': activeTab === 3}" @click="activeTab = 3">Upvoted</li>
                            </ul>

                            <v-tabs v-else grow class="tab-padding" xs12>
                                <v-tabs-bar>
                                    <v-tabs-slider color="blue"></v-tabs-slider>
                                    <v-tabs-item @click="activeTab = 1" :href="'#tab-1'" :key="1">Selling</v-tabs-item>
                                    <v-tabs-item @click="activeTab = 2" :href="'#tab-2'" :key="2">Sold</v-tabs-item>
                                    <v-tabs-item @click="activeTab = 3" :href="'#tab-3'" :key="3">Upvoted</v-tabs-item>
                                </v-tabs-bar>
                            </v-tabs>
                        </div>
                    </div>
                </div>

                <v-flex class="web-content">
                    <div v-if="activeTab === 1">
                        <router-link :to="{path:'/question/'+questionData.id}" v-for="questionData in profileData.ask">
                            <question-card :cardData="questionData" answer-btn click-card></question-card>
                        </router-link>
                    </div>
                    <div v-else-if="activeTab === 2">
                        <question-card v-for="answerData in profileData.answer" :cardData="answerData" answer-btn
                                       click-card></question-card>
                    </div>
                    <div v-else-if="activeTab === 3">
                        <p>UPVOTED CONTENT</p>
                    </div>
                </v-flex>

            </v-layout>
        </v-container>
    </div>
</template>

<style src="./profile.less" lang="less"></style>
<script src="./profile.js"></script>
