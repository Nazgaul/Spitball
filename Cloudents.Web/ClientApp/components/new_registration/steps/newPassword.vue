<template>
    <!--step create password-->
    <div class="step-password">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="meta.heading">
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width">
                <h1 v-if="!isMobile" class="step-title" v-html="meta.heading"></h1>
                <sb-input :class="['phone-field', 'pass-field', hintClass]"  :errorMessage="errorMessage.password" :bottomError="true"
                          :hint="passZxcvbn"
                          v-model="password" placeholder="login_placeholder_enter_new_password" name="password"
                          :type="'password'"
                          :autofocus="true" @keyup.enter.native="" minlength="4" v-language:placeholder></sb-input>
                <sb-input class="phone-field " :errorMessage="errorMessage.confirmPassword" :bottomError="true"
                          v-model="confirmPassword" placeholder="login_placeholder_confirm_password"
                          name="confirmPassword"
                          :type="'password'"
                          :autofocus="true" @keyup.enter.native="" v-language:placeholder></sb-input>
                <v-btn class="continue-btn"
                       value="Password"
                       :loading="loading"
                       :disabled="!(password && confirmPassword) || !isValidPass"
                       @click="changePassword()"
                ><span v-language:inner>login_continue</span></v-btn>

            </div>
            <img slot="step-image" :src="require(`../img/signin.png`)"/>
        </step-template>
    </div>
    <!--step create password-->
</template>

<script>
    //import zxcvbn from 'zxcvbn';
    import stepTemplate from '../helpers/stepTemplate.vue'
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    import { mapActions, mapMutations } from 'vuex'
    import registrationService from "../../../services/registrationService";

    const defaultSubmitRoute = {path: '/ask'};

    export default {
        name: "step_9",
        components: {stepTemplate, SbInput},

        data() {
            return {
                password: "",
                confirmPassword: "",
                errorMessage: {
                    code: '',
                    password: '',
                    confirmPassword: ''
                },
                loading: false,
                bottomError: false,
                score: {
                    default: 0,
                    required: false
                },
            }
        },
        props: {
            toUrl: {},
            isMobile: {
                type: Boolean,
                default: false
            },
            ID: {
                type: String,
                default: '',
                required: false
            },
            passResetCode: {
                type: String,
                default: '',
                required: false
            },
            meta: {},
            passScoreObj: {},
        },
        computed: {
            passZxcvbn() {
                if (this.password.length !== 0) {
                    this.score = global.zxcvbn(this.password).score;
                    return this.passScoreObj[this.score].name
                }
            },
            hintClass() {
                if (this.passZxcvbn) {
                    return this.passScoreObj[this.score].className;
                }
            },
            isValidPass(){
                return this.password.length >= 8  && this.confirmPassword.length >= 8
            },
        },
        methods: {
            changePassword() {
                if (this.password && this.ID && this.passResetCode && this.confirmPassword) {
                    // let self = this;
                    this.loading = true;
                    registrationService.updatePassword(this.password, this.confirmPassword, this.ID, this.passResetCode)
                        .then((response) => {
                            analyticsService.sb_unitedEvent('Forgot Password', 'Updated password');
                            global.isAuth = true;
                            this.loading = false;
                            let url = this.toUrl || defaultSubmitRoute;
                            //will be always ask cause he came from email
                            this.$router.push({path: `${url.path }`});
                        }, (error) => {
                            this.loading = false;
                            this.errorMessage.confirmPassword = error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : '';
                            this.errorMessage.password = error.response.data["Password"] ? error.response.data["Password"][0] : '';
                        });

                }
            }

        },

    }
</script>

<style scoped>

</style>