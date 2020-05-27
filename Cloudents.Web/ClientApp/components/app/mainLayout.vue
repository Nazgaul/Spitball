<template>
    <component :is="layout">
    </component>
</template>

<script>

const appDefault = () => import('./appDefault.vue');
const appSession = () => import('./../studyroom/appSession.vue');
export default {
    components: {
        appDefault,
        appSession,
    },
    computed: {
        layout() {
            return this.$route.meta.layout || 'appDefault';
        }
    }
}
</script>