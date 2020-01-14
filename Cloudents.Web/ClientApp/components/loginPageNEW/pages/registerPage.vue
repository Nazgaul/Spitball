<template>
    <section class="registerPage">
        <button class="back-button">
            <v-icon right @click="updateDialog(true)">sbf-close</v-icon>
        </button>
        <div class="leftSection" :class="{'reg_frymo': isFrymo}">
            <p v-language:inner="'loginRegister_main_txt'"></p>
        </div>

        <div class="stepsSections">
            <div class="stepContainer">
                <component :is="`${currentStep}`"/>
            </div>
        </div>

        <v-dialog v-model="showDialog" max-width="600px" :fullscreen="isMobile" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="updateDialog(false)">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width py-8 px-7">
                    <h1 style="font-size: 22px;" v-language:inner="'login_are_you_sure_you_want_to_exit'"/>
                    <p><span class="pre-line" v-language:inner="'login_exiting_information1'"/><br /></p>

                    <v-btn v-if="isMobile" height="48" class="continue-registr" @click="updateDialog(false)">
                        <span v-language:inner="'login_continue_registration'"/>
                    </v-btn>
                    <button class="continue-btn" @click="exit" v-language:inner>login_Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>

    </section>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

// globals
import getStarted from '../components/getStarted.vue'
import EmailConfirmed from '../components/EmailConfirmed.vue'

// register
import setEmailPassword from '../components/setEmailPassword.vue'
import setPhone from '../components/setPhone.vue'
import VerifyPhone from '../components/VerifyPhone.vue'
import congrats from '../components/congrats.vue'

// login
import setEmail from '../components/setEmail.vue'
import setPassword from '../components/setPassword.vue'

// reset password
import forgotPass from '../components/forgotPass.vue'
import resetPassword from '../components/resetPassword.vue'

//STORE
import storeService from '../../../services/store/storeService';
import loginRegister from '../../../store/loginRegister';

export default {
    data() {
        return {
            showDialog: false,
            from: ''
        }
    },
    components:{
        getStarted,
        setEmailPassword,
        EmailConfirmed,
        setPhone,
        VerifyPhone,
        congrats,
        setEmail,
        setPassword,
        forgotPass,
        resetPassword
        },
    computed: {
        ...mapGetters(['getCurrentLoginStep', 'isFrymo']),
        currentStep(){
            return this.getCurrentLoginStep
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }
    },
    methods: {
        ...mapActions(['goBackStep','updateStep','updateToUrl','exit','finishRegister']),
            goExit() {
            this.exit()
        },
        updateDialog(val){
            if(this.currentStep === 'congrats'){
                this.finishRegister()
            } else{
                this.showDialog = val
            }
        }
    },
    beforeDestroy(){
        storeService.unregisterModule(this.$store, 'loginRegister');
    },
    beforeRouteEnter (to, from, next) {
        next((vm) => {
            vm.from = from;
        });
    },
    created() {
        storeService.registerModule(this.$store, 'loginRegister', loginRegister);
        global.onpopstate = () => {
            this.goBackStep()
        }; 
        let path = this.$route.path.toLowerCase();

        this.$nextTick(() => {
            this.updateToUrl({path: this.from.fullPath});
        })       
        
        if (!!this.$route.query.returnUrl) {
            this.updateToUrl({ path: `${this.$route.query.returnUrl}`, query: { term: '' } })
        }
        if (path === '/resetpassword'){
            this.updateStep('resetPassword')
        }
        if (this.$route.query && this.$route.query.step) {
            if(this.$route.query.step === 'EnterPhone'){
                this.updateStep('setPhone')
            }else if (this.$route.query.step === 'VerifyPhone'){
                this.updateStep('VerifyPhone')
            }
        } 
    },
}

</script>
<style lang="less" src="./registerPage.less"/>