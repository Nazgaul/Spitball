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
                                <li :class="{'active': activeTab === 1}" @click="activeTab = 1">
                                    <span v-if="isMyProfile">My&nbsp;</span>Questions</li>
                                <li :class="{'active': activeTab === 2}" @click="activeTab = 2">
                                    <span v-if="isMyProfile">My&nbsp;</span>Answers</li>
                            </ul>

                            <v-tabs v-else grow class="tab-padding" xs12>
                                <v-tabs-bar>
                                    <v-tabs-slider color="blue"></v-tabs-slider>
                                    <v-tabs-item @click="activeTab = 1" :href="'#tab-1'" :key="1"> <span v-if="isMyProfile">My&nbsp;</span>Questions</v-tabs-item>
                                    <v-tabs-item @click="activeTab = 2" :href="'#tab-2'" :key="2"><span v-if="isMyProfile">My&nbsp;</span>Answers</v-tabs-item>
                                </v-tabs-bar>
                            </v-tabs>
                        </div>
                    </div>
                </div>

                <v-flex class="web-content">
                    <div v-if="activeTab === 1">
                        <router-link :to="{name:'question',params:{id:questionData.id}}" v-for="questionData in profileData.ask" :key="questionData.id">
                            <question-card :cardData="questionData"></question-card>
                        </router-link>
                    </div>
                    <div v-else-if="activeTab === 2">
                        <router-link :to="{name:'question',params:{id:answerData.id}}" v-for="answerData in profileData.answer" :key="answerData.id">
                            <question-card :cardData="answerData"></question-card>
                        </router-link>
                    </div>
                </v-flex>

            </v-layout>
        </v-container>
    </div>
</template>

<style src="./profile.less" lang="less"></style>
<script src="./profile.js"></script>
