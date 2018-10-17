<template>
    <!--step login-->
    <div class="step-login" >
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="meta.heading"></p>
                </div>
            </div>
            <div slot="step-data" class="limited-width">
                <h1 v-if="!isMobile" class="step-title" v-html="meta.heading"></h1>
                <form @submit.prevent="submit">
                    <sb-input :errorMessage="errorMessage.email" :required="true" class="email-field" type="email"
                              name="email" id="input-url" v-model="userEmail" :bottomError="true"
                              placeholder="login_placeholder_email" v-language:placeholder></sb-input>
                    <sb-input :errorMessage="errorMessage.password" :required="true" class="email-field mt-3"
                              :type="'password'" :bottomError="true"
                              name="user password"  v-model="password"
                              placeholder="login_placeholder_enter_password" v-language:placeholder></sb-input>

                    <v-btn class="continue-btn loginBtn"
                           value="Login"
                           :loading="loading"
                           :disabled="!userEmail || !password "
                           type="submit"
                    > <span v-language:inner>login_login</span>
                    </v-btn>
                </form>
                <div class="signin-strip">
                    <a @click="forgotPassword()" v-language:inner>login_forgot_password_link</a>
                </div>
            </div>
            <img slot="step-image" :src="require(`../img/signin.png`)"/>
        </step-template>
    </div>
    <!--step login end-->
</template>

<script>
    const defaultSubmitRoute = {path: '/ask'};
    import stepTemplate from '../helpers/stepTemplate.vue'
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    import { mapActions, mapMutations } from 'vuex'
    import registrationService from "../../../services/registrationService";
    export default {
        components: {stepTemplate, SbInput},
        name: "step_7",
        data() {
            return {
                siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
                errorMessage: {
                    phone: '',
                    code: '',
                    password: '',
                    confirmPassword: ''
                },
                password: '',
                loading: false,
                bottomError: false
            }
        },
        props: {
            isMobile: {
                type: Boolean,
                default: false
            },
            isCampaignOn: {
                type: Boolean,
                default: false
            },
            userEmail: {
                type: String,
                default : ""
            },
            meta: {},
            toUrl: {},
            lastActiveRoute: '',
        },
        methods: {
            submit() {
                let self = this;
                self.loading = true;
                registrationService.signIn(this.userEmail, this.password)
                    .then((response) => {
                        self.loading = false;
                        analyticsService.sb_unitedEvent('Login', 'Start');
                        // self.$parent.$emit('updateEmail', self.userEmail);
                        global.isAuth = true;
                        let url = self.toUrl || defaultSubmitRoute;
                        //will be always ask cause he came from email
                        self.$router.push({path: `${url.path }`});
                    }, function (error) {
                        self.loading = false;
                        self.errorMessage.email = error.response.data["Password"] ? error.response.data["Password"][0] : '';

                    });
            },
            forgotPassword() {
                this.$parent.$emit('fromCreate', 'forgot');

                this.$parent.$emit('changeStep', 'emailpassword');

            },
        },
        created(){
            this.password = '';
        }
    }
</script>

<style scoped>

</style>