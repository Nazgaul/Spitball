<template>  
         <div class="user-block" :class="classType">
             <slot name="icon"><user-avatar class="avatar-circle" :user-name="user.name" :user-id="user.id"/></slot>
         <slot name="text"><div class="text">
                <span class="user-date"><router-link v-if="!name" :to="{name:'profile',params:{id:user.id}}">{{user.name}}</router-link><template v-else>{{name}}</template><slot></slot></span>
                <span class="name">{{user.name}}</span>
                <p class="last-msg" v-if="text">{{text}}</p>
                <p class="university" v-if="user.universityName">{{user.universityName}}</p>
            </div></slot>

        </div> 
</template>

<style src="./user-block.less" lang="less"></style>
<script src="./user-block.js"></script>