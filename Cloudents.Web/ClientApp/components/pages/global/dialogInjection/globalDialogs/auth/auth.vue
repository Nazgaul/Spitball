<template>
    <v-dialog :value="true" max-width="620px" content-class="authDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">
        <component :is="params.show"></component>
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
    components: {
        login,
        register
    },
    computed: {
        componentState: {
            get() {
                return this.component
            },
            set(name) {
                this.$store.commit('setAuthDialogState', name)
            }
        }
    }
};
</script>