<template>
    <div>

        <div class="loginTitle">{{$t('loginRegister_setemail_title')}}</div>

        <v-form class="setEmail pa-4" @submit.prevent="login" ref="form">
            
            <v-btn
                @click="gmailRegister"
                depressed
                :loading="googleLoading"
                rounded
                sel="gmail"
                color="primary"
                class="google btn-login"
            >
                <img width="40" src="../../../../authenticationPage/images/G icon@2x.png" />
                <span class="btnText" v-t="'loginRegister_getstarted_btn_google_signup'"></span>
            </v-btn>

            <v-text-field 
                v-model="email"
                class="widther input-fields mt-5" 
                color="#304FFE"
                outlined
                height="44" 
                dense
                :label="labels['email']"
                :error-messages="errorMessages.email"
                :rules="[rules.required, rules.email]"
                placeholder=" "
                type="email"
            >
            </v-text-field>

            <v-text-field 
                v-model="password"
                :class="[hintClass,'widther','input-fields','mb-3']"
                color="#304FFE"
                outlined
                height="44"
                dense
                :label="labels['password']"
                :error-messages="errorMessages.password"
                :rules="[rules.required, rules.minimumCharsPass]"
                placeholder=" "
                type="password"
                :hint="passHint"
            >
            </v-text-field>
            
            <v-btn
                type="submit"
                depressed
                large
                :loading="btnLoading && !googleLoading"
                block
                rounded
                class="ctnBtn white--text btn-login"
                color="primary"
            >
                    <span>{{$t('loginRegister_setemail_btn')}}</span>
            </v-btn>

            <div class="text-center mt-4">
                <router-link :to="{name: 'forgotPassword'}" class="bottom">{{$t('loginRegister_setpass_forgot')}}</router-link>
            </div>
        </v-form>

    </div>
</template>

<script>
import authMixin from '../../../../../mixins/authMixin'

import registrationService from '../../../../../../services/registrationService2';
import analyticsService from '../../../../../../services/analytics.service.js';

export default {
    mixins: [authMixin],
    data() {
        return {
            password: '',
        }
    },
    props: {
        email: {
            type: String,
            default: '',
            required: true
        }
    },
    methods:{
        login(){    
            let emailObj = {
                email: this.email,
                password: this.password
            }

            let self = this
            registrationService.signIn(emailObj)
                .then(({data}) => {
                    global.country = data.country;
                    analyticsService.sb_unitedEvent('Login', 'Start');
                    self.$store.commit('setLoginDialog', false)
                    self.$store.dispatch('updateLoginStatus', true)
                }, (error)=> {
                    self.$store.commit('setErrorMessages',{email: error.response.data["Email"] ? error.response.data["Email"][0] : ''});
                });
        },
    }
}
</script>