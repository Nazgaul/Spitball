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
                <h1 v-if="!isMobile" class="step-title" v-language:inner>login_enter_new_password</h1>
                <sb-input class="phone-field" :errorMessage="errorMessage.password"
                          v-model="password" placeholder="Enter new password" name="password" type="password"
                          :autofocus="true" @keyup.enter.native="" minlength="4"></sb-input>
                <sb-input class="phone-field" :errorMessage="errorMessage.confirmPassword"
                          v-model="confirmPassword" placeholder="Confirm password" name="confirmPassword"
                          type="password"
                          :autofocus="true" @keyup.enter.native=""></sb-input>
                <v-btn class="continue-btn"
                       value="Password"
                       :loading="loading"
                       :disabled="!(password && confirmPassword)"
                       @click="changePassword()"
                ><span v-language:inner>login_continue</span></v-btn>

            </div>
            <img slot="step-image" :src="require(`../img/signin.png`)"/>
        </step-template>
    </div>
    <!--step create password-->
</template>

<script>
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
                loading: false
            }
        },
        props: {
            toUrl: {
                type: String,
                default: 'ask'
            },
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
            }
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
                        },(reason)=> {
                            this.loading = false;
                            this.errorMessage.confirmPassword = reason.response.data ? Object.values(reason.response.data)[0][0] : reason.message;
                            this.errorMessage.password = reason.response.data ? Object.values(reason.response.data)[0][0] : reason.message;
                        });

                }
            }

        },
        created(){
        }
    }
</script>

<style scoped>

</style>