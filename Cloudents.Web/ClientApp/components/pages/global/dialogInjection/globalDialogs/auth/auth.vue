<template>
    <v-dialog :value="true" max-width="410px" height="510" content-class="authDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">
        <component
            :is="tempComponent || params.component"
            :params="params"
            :teacher="teacher"
            @goTo="goTo"
            @updateRegisterType="updateRegisterType"
            class="wrapper"
        >
        </component>
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
        }
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
    .wrapper {
        height: 510px !important;
    }
</style>