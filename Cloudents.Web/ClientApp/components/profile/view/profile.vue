<template>
    <div class="profile-page">
        <v-container v-if="!isMobile">
            <v-layout row wrap>    

                <v-flex xs3>
                    <div class="main-block">
                        <img class="avatar" src="" />
                        <span class="name">User Name</span>
                        <p>univrsity Name</p>

                        <div class="menu">
                            
                        </div>
                    </div>  

                </v-flex>  

                <v-flex xs6 offset-xs1>
                        <div class="user-question">
                            <question-card :myQuestion="true"></question-card>    
                        </div>                        
                        <market-item :displayRow="3" :items="marketItem"></market-item>                        
                </v-flex> 

            </v-layout>
        </v-container> 
    </div>
</template>

<style src="./profile.less" lang="less"></style>
<script src="./profile.js"></script>
