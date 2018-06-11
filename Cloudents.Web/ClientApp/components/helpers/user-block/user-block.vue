<template>  
         <div class="user-block" :class="classType">
             <v-avatar size="32">
                 <span class="white--text">{{user.name.slice(0,2).toUpperCase()}}</span>
             </v-avatar>
            <div class="text">
                <span class="user-date"><router-link :to="{name:'profile',params:{id:user.id}}">{{user.name}}</router-link> · 1 hour ago </span>
                <span class="name">{{user.name}}</span>
                <p class="last-msg" v-if="text">{{text}}</p>
                <p class="university" v-if="user.universityName">{{user.universityName}}</p>
            </div>

        </div> 
</template>

<style src="./user-block.less" lang="less"></style>
<script src="./user-block.js"></script>