<template>
    <div class="profile-page">
        <v-container v-if="!isMobile">
            <v-layout row wrap>    

                <v-flex xs3>
                    <div class="main-block">

                        <div class="user-block">
                            <img class="avatar" src="./../../chat/img/user.png" />
                            <span class="name">User Name</span>
                            <p>univrsity Name</p>
                        </div>

                        <div class="menu">
                            <ul class="tabs">
                                <li :class="{'active': activeTab == 1}" @click="changeActiveTab(1)">Selling</li>
                                <li :class="{'active': activeTab == 2}" @click="changeActiveTab(2)">Sold</li>
                                <li :class="{'active': activeTab == 3}" @click="changeActiveTab(3)">Upvoted</li>
                            </ul>
                        </div>
                    </div>  

                </v-flex>  

                <v-flex xs6 class="web-content">
                        <div class="user-question">
                            <question-card :myQuestion="true"></question-card>    
                        </div>                        
                        <market-item :displayRow="2" :items="marketItem"></market-item>                        
                </v-flex> 

            </v-layout>

        </v-container> 

        <!-- Mobile version with tabs -->
        <div v-if="isMobile">
            <v-flex xs12>
                <div class="main-block">
                    <div class="user-block">
                            <img class="avatar" src="./../../chat/img/user.png" />
                            <span class="name">User Name</span>
                            <p>univrsity Name</p>
                        </div>
                </div>  

            </v-flex>  

            <v-tabs grow class="tab-padding">
                
                <v-tabs-bar >
                    <v-tabs-slider color="blue"></v-tabs-slider>
                    <v-tabs-item  :href="'#tab-1'" :key="'1'">Selling</v-tabs-item>
                    <v-tabs-item  :href="'#tab-2'" :key="'2'">Sold</v-tabs-item>
                    <v-tabs-item  :href="'#tab-3'" :key="'3'">Upvoted</v-tabs-item>
                </v-tabs-bar>     

                <v-flex xs12 class="mobile-content">
                        <div class="user-question">
                            <question-card :myQuestion="true"></question-card>    
                        </div>                        
                        <market-item :displayRow="2" :items="marketItem"></market-item>                        
                </v-flex> 

            </v-tabs>
        </div>
    </div>
</template>

<style src="./profile.less" lang="less"></style>
<script src="./profile.js"></script>
