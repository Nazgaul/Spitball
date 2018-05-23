<template>
    <v-flex xs12  class="conversations-list">
        
         <ul v-if="rooms.length">
            <li v-for="(room, index) in rooms" :class="{'active': activeRoom == room}" @click="selectRoom(room)">

                <div class="conversation">                   
                    <user-block :user="user" :classType="'chat'" :text="'sadasbdb'"></user-block>
                    <img class="some-icon" src="./../../img/user.png" />
                </div>
                              
            </li>                
        </ul>

    </v-flex>
</template>

<style src="./conversations-list.less" lang="less"></style>
<script src="./conversations-list.js"></script>