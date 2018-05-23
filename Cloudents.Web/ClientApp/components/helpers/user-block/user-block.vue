<template>  
         <div class="user-block">
            <img :class="classType" class="avatar"  :src="user.img" />
            <div class="text">
                <span :class="classType"><a href="/user">{{user.name}}</a> · 1 hour ago </span>
                <p class="last-msg" :class="classType" v-if="text">{{text}}</p>
            </div>
        </div> 
</template>

<style src="./user-block.less" lang="less"></style>
<script src="./user-block.js"></script>