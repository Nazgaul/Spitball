<template>
    <v-card>
        <div class="dialog-wrapp">
            <button class="close-btn text-md-right" @click.prevent="requestDialogClose()">
                <v-icon>sbf-close</v-icon>
            </button>
            <div class="ml-1 wrap-text">
                <h2 class="text-md-center" v-language:inner="'profileData_generalProfile_modal_heading'"></h2>
                <div class="wrapper-body-text">
                    <p class="dialog-body-text" v-language:inner="'profileData_generalProfile_modal_text'"></p>
                </div>
            </div>
            <div class="btn-container">
                <a  class="login-btns body-1" @click="goToSignIn()" v-language:inner>loginAnswer_login</a>
                <a  class="login-btns body-1" @click="goToRegister()" v-language:inner>loginAnswer_Sign_Up</a>
            </div>
        </div>
    </v-card>
</template>

<script src="./login-answer.js"></script>

<style src="./login-answer.less" lang="less"></style>
