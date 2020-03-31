<template>
    <v-dialog :value="true" max-width="410px" height="510" content-class="authDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">
        <component :is="tempComponent || params.component" @goTo="goTo" class="wrapper"></component>
    </v-dialog>
</template>

<script>

const login = () => import('./login/login2.vue')
const register = () => import('./register/register.vue')

export default {
    props: {
        params: {
            type: Object,
            required: false,
        }
    },
    data() {
        return {
            tempComponent: ''
        }
    },
    components: {
        login,
        register
    },
    methods: {
        goTo(name) {
            this.tempComponent = name
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
        height: 510px;
    }
</style>