<template>
    <!--step create password-->
    <div class="step-phone">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-language:inner>login_enter_new_password
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width">
                <h1 v-if="!isMobile" class="step-title"  v-language:inner>login_enter_new_password</h1>
                <sb-input class="phone-field" :errorMessage="errorMessage.password"
                          v-model="password" placeholder="Enter new password" name="password" type="password"
                          :autofocus="true" @keyup.enter.native="" minlength="4"></sb-input>
                <sb-input class="phone-field"   :errorMessage="errorMessage.confirmPassword"
                          v-model="confirmPassword" placeholder="Confirm password" name="confirmPassword" type="password"
                          :autofocus="true" @keyup.enter.native=""></sb-input>
                <v-btn class="continue-btn"
                       value="Password"
                       :loading="loading"
                       :disabled="!(password && confirmPassword)"
                       @click="validatePassword()"
                ><span v-language:inner>login_continue</span></v-btn>

            </div>
            <img slot="step-image" :src="require(`../img/signin.png`)"/>
        </step-template>
    </div>
    <!--step create password-->
</template>

<script>
    export default {
        name: "step_9",
        data() {
            return {
            }
        },
        props: {

        },
        methods: {
            validatePassword() {
                if (this.password !== this.confirmPassword) {
                    this.errorMessage.confirmPassword = 'Password do not match';
                    return
                } else if (this.password || this.confirmPassword) {
                    this.errorMessage.confirmPassword = 'Password should be longer than 6 symbols';
                }
                else {
                    this.updatePassword()
                }
            },
        },
    }
</script>

<style scoped>

</style>