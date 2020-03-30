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
    .wrapper {
        height: 510px;
    }
</style>