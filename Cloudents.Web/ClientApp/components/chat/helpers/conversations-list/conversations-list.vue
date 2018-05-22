<template>
    <v-flex xs12  class="conversations-list">
        
         <ul v-if="rooms.length">
            <li v-for="(room, index) in rooms" :class="{'active': activeRoom == room}" @click="selectRoom(room)">

                <div class="conversation">
                    
                    <div class="user-block">
                        <img class="avatar" src="./../../img/user.png" />
                        <div class="text">
                            <span><a href="/user">{{room.user}}</a> · 1 hour ago </span>
                            <p class="last-msg">{{room.lastMsg}}</p>
                        </div>
                    </div> 
                    <img class="some-icon" src="./../../img/user.png" />

                </div>
                
                
                              
            </li>                
        </ul>

    </v-flex>
</template>

<style src="./conversations-list.less" lang="less"></style>
<script src="./conversations-list.js"></script>