<template>
    <div class="registration">
        <step-template>
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Login</h1>
                <form @submit.prevent="submit">
                    <input required class="email-field input-field" name="email" v-model="userEmail" type="email"
                           placeholder="Enter your email address">
                    <input required class="password-field input-field" name="password" v-model="password" type="password"
                           placeholder="Password (Account key)">
                    <div class="checkbox-wrap">
                        <input id="keep-logged-in" type="checkbox" v-model="keepLogedIn">
                        <label for="keep-logged-in">keep me logged in</label>
                    </div>
                    <vue-recaptcha class="recaptcha-wrapper"
                                   sitekey="6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1"
                                   @verify="onVerify" @expired="onExpired"></vue-recaptcha>
                    <input v-bind:disabled="!(userEmail && password)" class="continue-btn" type="submit" value="Login">
                </form>
                <div class="signin-strip">Need an account?
                    <button type="button">Sign up</button>
                </div>
            </div>
            <img slot="step-image" :src="require(`./img/registerEmail.png`)"/>
        </step-template>
    </div>
</template>

<script>
    import stepTemplate from './steps/stepTemplate.vue';
    import VueRecaptcha from 'vue-recaptcha';
    import registrationService from '../../services/registrationService';

    export default {
        components: {stepTemplate, VueRecaptcha},
        data() {
            return {
                userEmail: '',
                password: '',
                keepLogedIn: false,
                recaptcha: '',
            }
        },
        methods: {
            submit() {
                self = this;
                registrationService.signIn(this.userEmail, this.password, this.recaptcha)
                    .then(function () {
                        debugger;
                        //   self.emailSent = true
                    }, function (reason) {
                        debugger;
                        console.error(reason);
                    });
            },
            onVerify(response) {
                this.recaptcha = response;
            },
            onExpired() {
                this.recaptcha = "";
            }
        },
    }
</script>
<style src="./registration.less" lang="less"></style>
<style src="./signin.less" lang="less"></style>
