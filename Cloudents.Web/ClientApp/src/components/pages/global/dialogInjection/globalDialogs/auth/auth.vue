<template>
    <v-dialog :value="true" max-width="410px" height="510" content-class="authDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">
        <component
            :is="tempComponent || params.component"
            :params="params"
            :teacher="teacher"
            @goTo="goTo"
            @updateRegisterType="updateRegisterType"
            @showToasterError="showToasterError"
            class="wrapper"
        >
        </component>

        <v-snackbar
            v-model="snackbar"
            class="error-toaster getStartedToaster"
            :timeout="5000"
            top
        >
            <div class="text-center flex-grow-1">
                {{gmailError}}
            </div>
        </v-snackbar>
    </v-dialog>
</template>

<script>

const login = () => import('./login/login2.vue')
const register = () => import('./register/register.vue')
const registerType = () => import('./register/registerType.vue')

export default {
    props: {
        params: {
            type: Object,
            required: false,
        }
    },
    data() {
        return {
            gmailError: '',
            tempComponent: '',
            teacher: false,
            snackbar: false
        }
    },
    components: {
        login,
        register,
        registerType
    },
    methods: {
        goTo(name) {
            this.tempComponent = name
        },
        updateRegisterType(val) {
            this.teacher = val;
        },
        showToasterError(error) {
            // if(error.error === 'popup_closed_by_user') {
            //     this.gmailError = this.$t('loginRegister_google_signin_error')
            // }else if(error) {
            //     this.gmailError = ('showToasterError', error.response.data["Google"] ? error.response.data["Google"][0] : '');
            // }
            if(error && error.response) {
                this.gmailError = ('showToasterError', error.response.data["Google"] ? error.response.data["Google"][0] : '');
                this.snackbar = true
            }
        }
    },
    created() {
        if(this.params.teacher) this.updateRegisterType(true)
    }
};
</script>

<style lang="less">
// this is idan(1) requirement.
//remove input background when click from autocomplete suggestions
input:-webkit-autofill,
input:-webkit-autofill:hover, 
input:-webkit-autofill:focus, 
input:-webkit-autofill:active  {
    -webkit-box-shadow: 0 0 0 30px white inset !important;
}
    .authDialog {
        background: #fff;
    }
    .wrapper {
        height: 510px !important;
    }
</style>