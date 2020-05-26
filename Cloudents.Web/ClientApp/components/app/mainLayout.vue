<template>
    <component :is="layout">
    </component>
</template>

<script>

const appDefault = () => import('./appDefault.vue');
const appSession = () => import('./../studyroom/appSession.vue');

//This should not be async since we can loose events if the components not loading in time

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