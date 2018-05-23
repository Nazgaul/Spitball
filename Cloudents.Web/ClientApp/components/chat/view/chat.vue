<template>
    <div class="chat-page">
        <v-container>

            <!-- Empty chat block -->
            <v-flex xs12 v-if="!rooms.length">                 
                <div class="empty-chat">
                    <img src="./../img/start-chat.png"   width="203"  height="170" />
                    <p>No chats yet!</p>
                    <button>Go Back</button>                        
                </div>
            </v-flex> 

            <v-layout row wrap v-else>                 
                    <v-flex class="list-block"  :class="{'xs4': !isMobile, 'xs12':isMobile}" v-if="(!currentRoom && isMobile) || !isMobile">        
                        <div class="header-block">
                            <button>
                                <v-icon right>sbf-arrow-right</v-icon>
                                <span>Chats</span>
                            </button>
                            
                            <img class="some-icon" src="./../img/user.png" />

                        </div>                
                        <v-tabs grow class="tab-padding">                
                            <v-tabs-bar >
                                <v-tabs-slider color="blue"></v-tabs-slider>
                                <v-tabs-item  :href="'#tab-1'" :key="'1'">All</v-tabs-item>
                                <v-tabs-item  :href="'#tab-2'" :key="'2'">Selling</v-tabs-item>
                                <v-tabs-item  :href="'#tab-3'" :key="'3'">Buying</v-tabs-item>
                                <v-tabs-item  :href="'#tab-4'" :key="'4'">Blocked</v-tabs-item>
                            </v-tabs-bar>    
                        </v-tabs>

                        <conversations-list :rooms="rooms" v-on:selectRoom="selectRoom"></conversations-list>

                    </v-flex>
                    <v-flex :class="{'xs8': !isMobile, 'xs12':isMobile}" v-if="(currentRoom && isMobile) || !isMobile">
                        <div class="empty-chat" v-if="!currentRoom">
                            <img src="./../img/start-chat.png"   width="203"  height="170" />
                            <p class="small">Tap a conversation to start chatting</p>                 
                        </div>

                        <mini-chat v-else></mini-chat>

                    </v-flex>
            </v-layout>

        </v-container> 
    </div>
</template>

<style src="./chat.less" lang="less"></style>
<script src="./chat.js"></script>
