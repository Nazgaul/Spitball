<template>
    <div>
        <div v-if="showExtended || cardData && cardData.subject">
            <!-- question userBlock -->
            <div class="user-block" :class="classType">
                <slot name="icon">
                    <user-avatar class="avatar-circle" :user-name="user.name"  :userImageUrl="user.image" :user-id="user.id"/>
                </slot>
                <slot name="text">
                    <div class="text">
                <span class="user-date">
                    <router-link v-if="!name" :to="{name:'profile',params:{id:user.id,name:user.name}}">{{user.name}}</router-link>
                    <template v-else>{{name}}</template>
                <slot></slot>
                </span>
                        <span class="name">{{user.name}}</span>
                        <p class="last-msg" v-if="text">{{text}}</p>
                        <p class="university" v-if="user.universityName">{{user.universityName}}</p>
                    </div>
                </slot>
            </div>
        </div>
        <div v-else>
            <!-- answer userBlock -->
            <div class="user-block" :class="classType">
                <div style="margin-top:15px;">
                    <slot name="icon">
                        <user-avatar class="avatar-circle" :userImageUrl="user.image" :user-name="user.name" :user-id="user.id"/>
                    </slot>
                </div>
            </div>
        </div>
    </div>
</template>

<style src="./user-block.less" lang="less"></style>
<script src="./user-block.js"></script>