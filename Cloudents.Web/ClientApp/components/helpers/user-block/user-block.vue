<template>  
         <div class="user-block" :class="classType">
             <user-avatar :user-name="user.name" :user-id="user.id"/>
            <div class="text">
                <span class="user-date"><router-link v-if="!name" :to="{name:'profile',params:{id:user.id}}">{{user.name}}</router-link><template v-else>{{name}}</template> · 1 hour ago </span>
                <span class="name">{{user.name}}</span>
                <p class="last-msg" v-if="text">{{text}}</p>
                <p class="university" v-if="user.universityName">{{user.universityName}}</p>
            </div>

        </div> 
</template>

<style src="./user-block.less" lang="less"></style>
<script src="./user-block.js"></script>