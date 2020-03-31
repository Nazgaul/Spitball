<template>
    <v-dialog :value="true" max-width="410px" height="510" content-class="authDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">
        <component
            :is="tempComponent || params.component"
            :params="params"
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
        updateRegisterType(type) {
            // this.
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