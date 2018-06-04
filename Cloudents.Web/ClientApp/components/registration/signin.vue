<template>
    <div class="registration">
        <step-template>
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Login</h1>
                <form @submit.prevent="submit">
                    <sb-input class="email-field" type="email" name="email" id="input-url" :value="userEmail" placeholder="Enter your email address"></sb-input>
                    <sb-input class="password-field" type="password" name="password" id="input-url" :value="password" placeholder="Password (Account key)"></sb-input>
                    <div class="checkbox-wrap">
                        <input id="keep-logged-in" type="checkbox" v-model="keepLogedIn">
                        <label for="keep-logged-in">keep me logged in</label>
                    </div>
                    <vue-recaptcha class="recaptcha-wrapper"
                                   sitekey="6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1"
                                   @verify="onVerify" @expired="onExpired"></vue-recaptcha>
                    <input v-bind:disabled="disableSubmit" class="continue-btn" type="submit" value="Login">
                </form>
                <div class="signin-strip">Need an account?
                    <router-link to="register">Sign up</router-link>
                </div>
            </div>
            <img slot="step-image" :src="require(`./img/signin.png`)"/>
        </step-template>
    </div>
</template>

<script src="./signin.js"></script>
<style src="./registration.less" lang="less"></style>
<style src="./signin.less" lang="less"></style>
