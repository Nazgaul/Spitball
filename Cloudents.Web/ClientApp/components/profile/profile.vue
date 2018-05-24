<template>
    <div class="profile-page" v-if="profileData">
        <v-layout :class="{'column': isMobile}" wrap justify-center>
            <v-flex>
                <div class="main-block">

                    <div class="user-block" v-if="profileData.user">
                        <img class="avatar" src="../chat/img/user.png"/>
                        <span class="name">{{profileData.user.name}}</span>
                        <p>{{profileData.user.universityName}}</p>
                    </div>

                    <div class="menu">
                        <ul class="tabs" v-if="!isMobile" xs3>
                            <li :class="{'active': activeTab === 1}" @click="activeTab = 1">Selling</li>
                            <li :class="{'active': activeTab === 2}" @click="activeTab = 2">Sold</li>
                            <li :class="{'active': activeTab === 3}" @click="activeTab = 3">Upvoted</li>
                        </ul>

                        <v-tabs v-else grow class="tab-padding" xs12>
                            <v-tabs-bar>
                                <v-tabs-slider color="blue"></v-tabs-slider>
                                <v-tabs-item @click="activeTab = 1"></v-tabs-item>
                                <v-tabs-item @click="activeTab = 2"></v-tabs-item>
                                <v-tabs-item @click="activeTab = 3"></v-tabs-item>
                            </v-tabs-bar>
                        </v-tabs>
                    </div>
                </div>
            </v-flex>

            <v-flex class="web-content">
                <div v-if="activeTab === 1">
                    <question-card v-for="questionData in profileData.ask" :cardData="questionData"
                                   :myQuestion="true"></question-card>
                </div>
                <div v-else-if="activeTab === 2">
                    <question-card v-for="answerData in profileData.answer" :cardData="answerData"
                                   :myQuestion="true"></question-card>
                </div>
                <div v-else-if="activeTab === 3">UPVOTED CONTENT</div>
            </v-flex>
        </v-layout>


    </div>
</template>

<style src="./profile.less" lang="less"></style>
<script src="./profile.js"></script>
