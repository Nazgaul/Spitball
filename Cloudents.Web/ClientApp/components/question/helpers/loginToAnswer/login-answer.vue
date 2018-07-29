<template>
    <v-card>
        <div class="dialog-wrapp">
            <button class="close-btn text-md-right" @click.prevent="requestDialogClose()">
                <v-icon>sbf-close</v-icon>
            </button>
            <div class="ml-1 wrap-text">
                <h2 class="text-md-left"> Please <br/><b>sign up</b> or <b>login</b><br/> to continue.</h2>
                <div class="wrapper-body-text">
                    <p class="dialog-body-text">You are a couple of mouse clicks away from FREE access to making serious money.</p>
                </div>


            </div>
            <div class="btn-container">
                <a class="login-btns body-1" href="/register">Sign Up</a>
                <a class="login-btns body-1" href="/signin">Login</a>
            </div>

        </div>
    </v-card>


</template>

<script src="./login-answer.js"></script>

<style src="./login-answer.less" lang="less"></style>
