<template>  
         <div class="user-block" :class="classType">
            <img  class="avatar" :src="user.img" />
            <div class="text">
                <span class="user-date"><router-link :to="{path:'/profile/'+user.id}">{{user.name}}</router-link> · 1 hour ago </span>
                <span class="name">{{user.name}}</span>
                <p class="last-msg" v-if="text">{{text}}</p>
                <p class="university" v-if="user.universityName">{{user.universityName}}</p>
            </div>

        </div> 
</template>

<style src="./user-block.less" lang="less"></style>
<script src="./user-block.js"></script>