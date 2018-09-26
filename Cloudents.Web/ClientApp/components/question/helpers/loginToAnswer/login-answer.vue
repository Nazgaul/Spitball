<template>
    <v-card>
        <div class="dialog-wrapp">
            <button class="close-btn text-md-right" @click.prevent="requestDialogClose()">
                <v-icon>sbf-close</v-icon>
            </button>
            <div class="ml-1 wrap-text">
                <h2 class="text-md-left"> {{profileData.modal.heading}}</h2>
                <div class="wrapper-body-text">
                    <p class="dialog-body-text"> {{profileData.modal.text}}</p>
                </div>
            </div>
            <div class="btn-container">
                <router-link  class="login-btns body-1" :to="{ path: '/register', query:{returnUrl : $route.path}}" v-language:inner>loginAnswer_Sign_Up</router-link>
                <router-link  class="login-btns body-1" :to="{ path: '/signin'}" v-language:inner>loginAnswer_login</router-link>
            </div>
        </div>
    </v-card>
</template>

<script src="./login-answer.js"></script>

<style src="./login-answer.less" lang="less"></style>
