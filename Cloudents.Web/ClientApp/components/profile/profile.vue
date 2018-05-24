<template>
    <div class="profile-page" v-if="profileData">
        <v-layout :class="{'column': isMobile}" wrap justify-center>
            <v-flex>
                <div class="main-block">

                    <div class="user-block">
                        <img class="avatar" src="../chat/img/user.png"/>
                        <span class="name">{{profileData.user.name}}</span>
                        <p>{{profileData.user.name}}</p>
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
                                <v-tabs-item :href="'#tab-1'" :key="'1'">Selling</v-tabs-item>
                                <v-tabs-item :href="'#tab-2'" :key="'2'">Sold</v-tabs-item>
                                <v-tabs-item :href="'#tab-3'" :key="'3'">Upvoted</v-tabs-item>
                            </v-tabs-bar>
                        </v-tabs>
                    </div>
                </div>

            </v-flex>


            <v-flex class="web-content">
                <question-card v-for="questionData in profileData.ask" :cardData="questionData"
                               :myQuestion="true"></question-card>
            </v-flex>
        </v-layout>


    </div>
</template>

<style src="./profile.less" lang="less"></style>
<script src="./profile.js"></script>
