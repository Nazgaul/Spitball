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
            <div class="text-center flex-grow-1" v-t="'loginRegister_google_signin_error'"></div>
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
        showToasterError() {
            this.snackbar = true
        }
    }
};
</script>

<style lang="less">
    .wrapper {
        background: #fff;
        height: 510px;
    }
</style>