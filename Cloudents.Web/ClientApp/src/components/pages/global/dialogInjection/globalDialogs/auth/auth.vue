<template>
    <v-dialog :value="true" max-width="410px" height="510" content-class="authDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">
        <component
            :is="tempComponent || params.component"
            :params="params"
            :teacher="teacherState"
            @goTo="goTo"
            @updateRegisterType="updateRegisterType"
            class="wrapper"
        >
        </component>
    </v-dialog>
</template>

<script>
import authMixin from './authMixin'
const login = () => import('./login/login2.vue')
const register = () => import('./register/register.vue')
const registerType = () => import('./register/registerType.vue')
const setPhone = () => import('./register/setPhone2.vue')

export default {
    mixins: [authMixin],
    components: {
        login,
        register,
        registerType,
        setPhone
    },
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
            teacherState: false,
            snackbar: false
        }
    },
    methods: {
        goTo(name) {
            this.tempComponent = name
        },
        updateRegisterType(val) {
            this.teacherState = val;
        },
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